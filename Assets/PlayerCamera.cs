using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    Transform cameraPivot;
    float maxLookAngle = 60f;
    float minLookAngle = -60f;
    public float cameraXAngle;
    float cameraYAngle;
    [SerializeField]
    float cameraXSpeed = 220f;
    [SerializeField]
    float cameraYSpeed = 220f;
    void Start()
    {
        cameraPivot = transform.GetChild(0).gameObject.transform;
    }
    void LateUpdate()
    {
        RotateCamera();
    }

    void RotateCamera()
    {

        cameraXAngle += Input.GetAxis("Mouse X") * cameraXSpeed * Time.deltaTime;
        cameraYAngle -= Input.GetAxis("Mouse Y") * cameraYSpeed * Time.deltaTime;

        cameraYAngle = Mathf.Clamp(cameraYAngle, minLookAngle, maxLookAngle);

        transform.localRotation = Quaternion.Euler(0, cameraXAngle, 0);

        cameraPivot.localRotation = Quaternion.Euler(cameraYAngle, 0, 0);
    }
}
