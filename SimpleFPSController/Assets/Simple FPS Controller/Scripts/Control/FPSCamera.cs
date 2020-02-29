using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FPSCamera : MonoBehaviour
{
    public static FPSCamera mainFPSCamera;
    public static Camera mainCamera;

    public bool lockCursor = false;

    public float Sensitivity = 50;
    [HideInInspector] public float mouseX, mouseY;
    private float rotationX, rotationY;
    [HideInInspector] public float aimedZRotation = 0, rotationZ; // the rotation in z axis(mainly used for shake effect)

    public Transform Player;
    public Transform CameraPosition;

    void Awake()
    {
        Application.targetFrameRate = 90;
    }

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        mainFPSCamera = this;
        mainCamera = this.GetComponent<Camera>();

        Player.GetComponent<PlayerMovement>().SetupFOV(mainCamera.fieldOfView);
    }
    
    private void Update()
    {
        // Input from the mouse
        mouseX = Input.GetAxis("Mouse X") * Sensitivity * TimeManager.currentTimeScale;
        mouseY = Input.GetAxis("Mouse Y") * Sensitivity * TimeManager.currentTimeScale;
        
        // Calculating the rotation
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90, 90);
        rotationY += mouseX;

        // Rotating
        rotationZ = Mathf.Lerp(rotationZ, aimedZRotation, Time.deltaTime * 8);
        transform.localRotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
        Player.Rotate(Vector3.up * mouseX);

        transform.position = CameraPosition.position;
    }
}
