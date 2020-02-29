using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    public static WallRun mainWallRun;

    private RaycastHit hitInfo;
    private bool attachedOnWall;
    private bool directionToCheckForWall = false; // False is left and True is right
    private Vector3 velocity, zeroVector;
    private float timerToAttachToNextWall;

    [Header("Specifications to attach")]
    public float maxDistanceToAttachToWall; // The maximum distance needed for the player to attach to a wall

    [Header("Specifications for run")]
    public float runningSpeed = 2.5f;
    public float dampingVelocity = 4f;
    public float heightForce = 8f;
    public float sideForce = 5f;

    private void Start()
    {
        mainWallRun = this;

        zeroVector = Vector3.zero;
    }

    private void Update()
    {
        if (!attachedOnWall)
        {
            timerToAttachToNextWall -= Time.deltaTime;
            if (timerToAttachToNextWall <= 0)
            {
                timerToAttachToNextWall = 0.0f;
                if (Physics.Raycast(transform.position, transform.right, out hitInfo, maxDistanceToAttachToWall))
                {
                    if (hitInfo.transform.CompareTag("WallRun"))
                    {
                        if (PlayerMovement.mainPlayerMovement.v <= 0)
                            return;
                        directionToCheckForWall = true;
                        BeginWallRun(10);

                        goto _WallRunning_Method_;
                    }
                }

                if (Physics.Raycast(transform.position, -transform.right, out hitInfo, maxDistanceToAttachToWall))
                {
                    if (hitInfo.transform.CompareTag("WallRun"))
                    {
                        if (PlayerMovement.mainPlayerMovement.v <= 0)
                            return;
                        directionToCheckForWall = false;
                        BeginWallRun(-10);

                        goto _WallRunning_Method_;
                    }
                }
            }
        }
        _WallRunning_Method_:
        if (attachedOnWall)
            WallRunning();
        
        if(velocity != zeroVector)
        {
            velocity -= Vector3.Normalize(velocity) * dampingVelocity * Time.deltaTime;
            if (velocity.sqrMagnitude <= .2f)
                velocity = zeroVector;
        }
        PlayerMovement.mainPlayerMovement.velocity += velocity + inputVelocity;
    }
    
    /// <param name="zRotation"> The Z rotation of the camera </param>
    private void BeginWallRun(float zRotation)
    {
        attachedOnWall = true;
        PlayerMovement.mainPlayerMovement.useGravity = false;
        PlayerMovement.mainPlayerMovement.DisableMovement();
        FPSCamera.mainFPSCamera.aimedZRotation = zRotation;
        velocity = zeroVector;

        if(GrapplingHook.mainGrapplingHook != null){
            GrapplingHook.mainGrapplingHook.Unhook();
            GrapplingHook.mainGrapplingHook.momentum = zeroVector;
        }
    }

    public void EndWallRun()
    {
        attachedOnWall = false;
        PlayerMovement.mainPlayerMovement.useGravity = true;
        PlayerMovement.mainPlayerMovement.EnableMovement();
        FPSCamera.mainFPSCamera.aimedZRotation = 0;
        timerToAttachToNextWall = .2f;
        inputVelocity = zeroVector;
    }

    private void WallRunning()
    {
        if (PlayerMovement.mainPlayerMovement.v <= 0)
        {
            EndWallRun();
            return;
        }

        // Checking if still attached to the wall
        if(!Physics.Raycast(transform.position, 
        directionToCheckForWall ? transform.right : -transform.right, // Direction to the wall
        out hitInfo, maxDistanceToAttachToWall))
        {
            EndWallRun();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            velocity = (directionToCheckForWall ? -transform.right : transform.right) * sideForce + transform.forward * sideForce * .5f + Vector3.up * heightForce;
            EndWallRun();
            return;
        }

        inputVelocity = transform.forward * runningSpeed - Vector3.up * 4;
    }
    private Vector3 inputVelocity;
}
