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
    bool isAiming;

    public Vector3 defaultOffset = new Vector3(0, 0, -3f);

    public Vector3 aimOffset = new Vector3(1, 0, -2f);

    Vector3 targetOffset;

    public float offsetSpeed = 2f;
    void Start()
    {
        targetOffset = defaultOffset;
        cameraPivot = transform.GetChild(0).gameObject.transform;
    }

    void Update()
    {
        isAiming = Input.GetButton("Fire2");
        if(isAiming)
        {
            targetOffset = aimOffset;
        }
        else
            targetOffset = defaultOffset;
    }
    void LateUpdate()
    {
        RotateCamera();
        AimOffset();
    }

    void RotateCamera()
    {

        cameraXAngle += Input.GetAxis("Mouse X") * cameraXSpeed * Time.deltaTime;
        cameraYAngle -= Input.GetAxis("Mouse Y") * cameraYSpeed * Time.deltaTime;

        cameraYAngle = Mathf.Clamp(cameraYAngle, minLookAngle, maxLookAngle);

        transform.localRotation = Quaternion.Euler(0, cameraXAngle, 0);

        cameraPivot.localRotation = Quaternion.Euler(cameraYAngle, 0, 0);
    }

    void AimOffset()
    {
        if(Camera.main.transform.localPosition != targetOffset)
        {
            Vector3 newPosition = Vector3.Lerp(Camera.main.transform.localPosition, targetOffset, Time.deltaTime * offsetSpeed);

            Camera.main.transform.localPosition = newPosition;
        }
    }
}
