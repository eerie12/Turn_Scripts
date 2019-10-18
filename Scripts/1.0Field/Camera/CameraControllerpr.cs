using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerpr : MonoBehaviour
{
    #region MainCameraの変数
    public VirtualJoystick camerajoystick;

    public Transform target;
    public Transform player;
    public Vector3 offset;
    public Transform pivot;
    public bool useOffsetValues;
    public bool invertY;
    public float rotateSpeed;   
    public float maxViewAngle;
    public float minViewAngle;
    float pitch = 0;
    float yaw = 0;
    float distance;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if (!useOffsetValues)//offsetを使わなかった場合
        {
            offset = target.position - transform.position;
        }
        pivot.transform.position = target.transform.position;
        distance = offset.magnitude;
        pivot.transform.parent = null;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        pivot.transform.position = target.transform.position;
        float horizontal = camerajoystick.Horizontal() * rotateSpeed;
        float vertical = camerajoystick.Vertical() * rotateSpeed;
        yaw += horizontal;
        pitch = Mathf.Clamp(pitch + vertical, 0f, 45f);
        var q = Quaternion.Euler(pitch, yaw, 0f);
        transform.position = pivot.transform.position - q * Vector3.forward * distance;
        transform.LookAt(pivot);

    }

    
}
