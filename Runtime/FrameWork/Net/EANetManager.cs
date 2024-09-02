using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HybridWebSocket;

public class EANetManager : Singleton<EANetManager>
{
    private ConcurrentQueue<SendInfo> sendQue = new ConcurrentQueue<SendInfo>();
    
    private bool initialize = false;

    public bool IsLoginEnd { get; set; }

    public string Cookie { get; private set; }

    public WebSocket WSocket { get { return ws; } }

    private WebSocket ws = null;

    public System.Action<WebSocket> onOpenCallback = null;
    public System.Action<byte[]> onMessageCallback = null;
    public System.Action<WebSocket, WebSocketCloseCode> onCloseCallback = null;
    public System.Action<string> onErrorMsgCallback = null;

    public System.Action<int,System.Action> onCloseAction = null;
    public System.Func<UICtrl> onAlarmPopupAction = null;

    private static string serverURL = string.Empty;
    private static string webSyncServerURL = string.Empty;

    public static string WEB_SERVER_URL { get { return serverURL; } }
    public static string WEB_SyncSERVER_URL { get { return webSyncServerURL; } }

    public override GameObject GetSingletonParent()
    {
        return EAMainFrame.instance.gameObject;
    }

    public class SendInfo
    {
        public string protocolId;
        public IWebProtocol webProtocol;
        public bool reconnect;
        public int reqCount;
    }

    protected override void Initialize()
    {
        Cookie = string.Empty;

        while (sendQue.Count > 0)
        {
            bool checkOk = sendQue.TryDequeue(out SendInfo sendInfo);
            if (checkOk)
            {
                Debug.Log("NetManager initialize WebProtocol release : " + sendInfo.protocolId);
                sendInfo.webProtocol.Release();
            }
        }

        initialize = true;
    }

    protected override void Close()
    {
        base.Close();

        if(ws != null) ws.Close();
        ws = null;
    }

    public void SetCookie(string cookie) => this.Cookie = cookie;

    private void Update()
    {
        if (!initialize) return;

        if (sendQue.Count > 0)
        {
            sendQue.TryPeek(out SendInfo sendInfo);
            bool receiveOk = sendInfo.webProtocol.Process(sendInfo);
            if (receiveOk == true)
            {
                sendInfo.webProtocol.Release();
                sendQue.TryDequeue(out sendInfo);
            }
            else if (sendInfo.reconnect)
            {
               var uiPopup = GetAlertPopup();

                if (sendInfo.reqCount >= 3)
                {
                    uiPopup?.Close();
                    sendInfo.webProtocol.Release();
                    sendQue.TryDequeue(out sendInfo);

                    CloseAction(0);
                    return;
                }

                if (uiPopup != null) 
                {
                    if (uiPopup.IsActivate) return;
                }
                
                if (sendInfo.reqCount == 0)
                {
                    sendInfo.reconnect = false;
                    sendInfo.webProtocol.RequestToServer();
                    sendInfo.reqCount = sendInfo.reqCount + 1;
                    Debug.Log($"first reconnect  : {sendInfo.protocolId} reqCount {sendInfo.reqCount}");
                    return;
                }

                string msg = $"reconnect ½Ãµµ ({sendInfo.reqCount + 1})";

                CloseAction(1,()=>
                {
                    sendInfo.reconnect = false;
                    sendInfo.webProtocol.RequestToServer();
                    sendInfo.reqCount = sendInfo.reqCount + 1;
                    Debug.Log($"reconnect  : {sendInfo.protocolId} reqCount {sendInfo.reqCount}");
                });
            }
        }
    }

    public void SetServerURL(string serverURL) => EANetManager.serverURL = serverURL;
    public void SetSyncServerURL(string syncserverURL) => EANetManager.webSyncServerURL = syncserverURL;

    public void CloseAction(int type , System.Action callBack = null)
    {
        if (onCloseAction != null) onCloseAction.Invoke(type ,callBack);
    }

    public UICtrl GetAlertPopup()
    {
        if (onAlarmPopupAction != null)
            return onAlarmPopupAction();

        return null;
    }

    public void Send(IWebProtocol webProtocol)
    {
        if (sendQue.Count > 0)
        {
            int count = sendQue.Count(x => x.protocolId == webProtocol.protocolId);
            if (count > 0) return;
        }

        webProtocol.Create();

        sendQue.Enqueue(new SendInfo()
        {
            protocolId = webProtocol.protocolId,
            webProtocol = webProtocol
        });

        webProtocol.RequestToServer();
    }

    public void PostRequest<RequestT, ResultT>(string protocolId, RequestT request, Action<ResultT> resultCallback)
    where RequestT : class, new()
    where ResultT : class,  new()
    {
        var protocol = new WebProtocolBase<RequestT, ResultT>();
        protocol.Initialize(protocolId, ProtoType.TYPE_STRING);
        protocol.PostRequest(request, resultCallback);
    }

    public void OpenSocket()
    {
        if (ws == null) ws = WebSocketFactory.CreateInstance(EANetManager.WEB_SyncSERVER_URL);

        ws.OnOpen += () => 
        {
            Debug.Log("WS connected!");
            Debug.Log("WS state: " + ws.GetState().ToString());
            if(onOpenCallback != null) onOpenCallback.Invoke(ws);           
        };

        ws.OnMessage += (byte[] msg) =>
        {
            if (onMessageCallback != null) onMessageCallback.Invoke(msg);
        };

        ws.OnError += (string errMsg) =>
        {
            Debug.Log("WS error: " + errMsg);
            if(onErrorMsgCallback != null) onErrorMsgCallback.Invoke(errMsg);
        };

        ws.OnClose += (WebSocketCloseCode code) =>
        {
            Debug.Log("WS Closed with code: " + code.ToString());
            if (onCloseCallback != null) onCloseCallback.Invoke(ws, code);
        };

        ws.Connect();
    }
}
