using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    private void Start() {
    }
 
    void Update ()
    {
        Vector3 desiredPosition = target.position + offset;
        transform.position = desiredPosition;
    }

}
