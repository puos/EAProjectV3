using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class UserCam : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cam;
    
    static List<GameObject> cams = new List<GameObject>();
    private GameObject cachedObject = null;

    public void Initialize()
    {
        cachedObject = gameObject;
        cams.Add(cachedObject);
        cachedObject.SetActive(false);
    }
    public void SetFollow(Transform tr) => cam.Follow = tr;

    public void SwitchCam()
    {
        for (int i = 0; i < cams.Count; ++i) cams[i].SetActive(false);
        cachedObject.SetActive(true);
    }
}
