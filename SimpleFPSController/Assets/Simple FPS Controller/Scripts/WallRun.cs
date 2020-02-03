using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))] // this also requires Character Controller
public class WallRun : MonoBehaviour
{
    public static FPSCamera fpsCam;
    public static GrapplingHook gHook;
    
    private RaycastHit hitInfo;
    private PlayerMovement pvm;
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
        pvm = GetComponent<PlayerMovement>();
        GrapplingHook.wr = this;

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
                        if (pvm.v <= 0)
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
                        if (pvm.v <= 0)
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
        pvm.velocity += velocity + inputVelocity;
    }
    
    /// <param name="zRotation"> The Z rotation of the camera </param>
    private void BeginWallRun(float zRotation)
    {
        attachedOnWall = true;
        pvm.useGravity = false;
        pvm.DisableMovement();
        fpsCam.aimedZRotation = zRotation;
        velocity = zeroVector;

        gHook.Unhook();
        gHook.momentum = zeroVector;
    }

    public void EndWallRun()
    {
        attachedOnWall = false;
        pvm.useGravity = true;
        pvm.EnableMovement();
        fpsCam.aimedZRotation = 0;
        timerToAttachToNextWall = .2f;
        inputVelocity = zeroVector;
    }

    private void WallRunning()
    {
        if (pvm.v <= 0)
        {
            EndWallRun();
            return;
        }

        // Checking if still attached to the wall
        if (directionToCheckForWall)
        {
            if(!Physics.Raycast(transform.position, transform.right, out hitInfo, maxDistanceToAttachToWall))
            {
                EndWallRun();
                return;
            }
        }
        else
        {
            if (!Physics.Raycast(transform.position, -transform.right, out hitInfo, maxDistanceToAttachToWall))
            {
                EndWallRun();
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (directionToCheckForWall)
            {
                //PlayerMovement.cc.Move((-transform.right * runningSpeed + transform.forward * runningSpeed) * runningSpeed * Time.deltaTime);
                velocity = -transform.right * sideForce + transform.forward * sideForce * .5f + Vector3.up * heightForce;
                EndWallRun();
            }
            else
            {
                //PlayerMovement.cc.Move((transform.right * runningSpeed + transform.forward * runningSpeed) * runningSpeed * Time.deltaTime);
                velocity = transform.right * sideForce + transform.forward * sideForce * .5f + Vector3.up * heightForce;
                EndWallRun();
            }
            return;
        }

        inputVelocity = transform.forward * runningSpeed - Vector3.up * 4;
    }
    private Vector3 inputVelocity;
}
