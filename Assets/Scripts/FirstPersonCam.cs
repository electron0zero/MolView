using UnityEngine;
using System.Collections;

public class FirstPersonCam : MonoBehaviour {

    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    void Update () {
         if (Input.GetMouseButton(0)){
            yaw += speedH * Input.GetAxis("Mouse X");
            pitch -= speedV * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
         } else {
            transform.Rotate(new Vector3(0, 1, 0));
         }
    }
}