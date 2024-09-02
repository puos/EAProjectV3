using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public enum ProtoType {  TYPE_NONE, TYPE_STRING, TYPE_PROTOBUF }

public interface IWebProtocol
{
    string protocolId { get; }

    void Create();

    void RequestToServer();

    bool Process(EANetManager.SendInfo s);

    void Release();
}

public class WebProtocolBase<RequestT,ResultT> : IWebProtocol
    where RequestT : class , new()
    where ResultT : class, new()
{
    public RequestT request { get; protected set; }
    public ResultT response { get; protected set; }

    public string protocolId { get; protected set; }

    public byte[] payload;

    private UnityWebRequest webRequest = null;

    private int timeout = 10000;
    //private int timeout = 10;

    private Action<ResultT> resultCallback;

    private string cookie;

    public ProtoType protoType { get; protected set; }

    public string contentType { get; protected set; }

    public void Initialize(string protocolId, ProtoType protoType)
    {
        this.protocolId = protocolId;
        this.protoType = protoType;
        this.cookie = EANetManager.instance.Cookie;

        this.request = new RequestT();
        this.response = new ResultT();
    }

    public virtual void PostRequest(RequestT request, Action<ResultT> resultCallback)
    {
        this.request = request;
        this.resultCallback = resultCallback;
        EANetManager.instance.Send(this);
    }

    public void Create()
    {
        string json = JsonUtility.ToJson(request);
        payload = Encoding.UTF8.GetBytes(json);
        contentType = "application/json";
    }

    public void RequestToServer()
    {
        if (webRequest != null) webRequest.Dispose();
        webRequest = null;

        try
        {
            webRequest = UnityWebRequest.Put(EANetManager.WEB_SERVER_URL + protocolId, payload);
            webRequest.method = "POST";
            webRequest.timeout = timeout;
            webRequest.SetRequestHeader("Content-Type", contentType);
            webRequest.SetRequestHeader("Content-length", payload.Length.ToString());
            webRequest.SetRequestHeader("rkey", Guid.NewGuid().ToString());
            if (!string.IsNullOrEmpty(cookie)) webRequest.SetRequestHeader("Cookie", cookie);
            webRequest.SendWebRequest();

            Debug.Log($"RequestOk Url : {webRequest.url} rKey : {webRequest.GetRequestHeader("rkey")}");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogError($"RequestError Url : {EANetManager.WEB_SERVER_URL + protocolId} payload Length : {payload.Length}");
        }
    }

    public bool Process(EANetManager.SendInfo s)
    {
        if (webRequest == null) return true;
        if (s.reconnect == true) return false;

        if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
            webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error : {webRequest.error} , ReachState : {Application.internetReachability}");
            s.reconnect = true;
            return false;
        }

        if (webRequest.isDone == false) return false;
        if (webRequest.downloadProgress < 1.0f) return false;

        if (string.IsNullOrEmpty(cookie)) EANetManager.instance.SetCookie(webRequest.GetResponseHeader("Set-Cookie"));

        response = FromObj<ResultT>(webRequest.downloadHandler);

        if (resultCallback != null) resultCallback(response);

        return true;
    }

    public void Release()
    {
        if (webRequest == null) return;

        webRequest.Dispose();
        webRequest = null;
        payload = null;
    }

    public T FromObj<T>(DownloadHandler handler) where T : new()
    {
        T obj = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
        return obj;
    }
}
