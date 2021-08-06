using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target = null;
    Vector3 offset = Vector3.zero;
    private bool initialized = false;
    private float ratio = 2.0f;
    private bool rotationFollow = false;
   
    public void Initialize(Transform target,bool rotationFollow = false)
    {
        this.target = target;
        this.rotationFollow = rotationFollow;

        offset = transform.position - target.position;
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (initialized == false) return;

        Vector3 cameraPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, cameraPosition, Time.deltaTime * ratio);
        if (rotationFollow == true) transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * ratio);
    }
}
