using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public static CharacterController cc;
    public static PlayerMovement mainPlayerMovement;


    [Header("Player Movement")]
    public float Speed = 2.0f;
    private bool enableMovement = true;
    [HideInInspector] public float v, h;
    [HideInInspector] public Vector3 velocity, inputVelocity, gravityVector, slidingMomentum;

    [Header("Gravity")]
    public bool useGravity = true;
    public Transform GroundChecker;
    public float maxGroundDistance = 0.2f;
    private RaycastHit hitInfo;
    [HideInInspector]
    public bool isGrounded = false;
    private float slopeAngle, slidingTime;

    [Header("Jump")]
    public float jumpHeight = 2.0f;
    private float jumpForce;

    [Header("Effects")]
    public ParticleSystem HyperDrive;

    [HideInInspector]
    public float FOVAim = 0.0f, DefaultFOV;
    public float maxFOV = 140;

    private void Start()
    {
        cc = this.GetComponent<CharacterController>();

        mainPlayerMovement = this;

        jumpForce = Mathf.Sqrt(-Physics.gravity.y * jumpHeight);
    }

    public void SetupFOV(float DefFOV)
    {
        DefaultFOV = DefFOV;
        FOVAim = DefaultFOV;
    }

    void Update()
    {
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");

        if (enableMovement)
        {
            inputVelocity = (transform.forward * v + transform.right * h) * Speed;

            if (Physics.Raycast(GroundChecker.position, Vector3.down, out hitInfo, maxGroundDistance))
            {
                isGrounded = true;

                gravityVector = zeroVector;
                //gravityVector *= dampingVelocity;

                // Jumping
                if (Input.GetKeyDown(KeyCode.Space))
                    gravityVector.y += jumpForce;
            }
            else
            {
                isGrounded = false;

                if (useGravity){
                    if(GrapplingHook.mainGrapplingHook != null){
                        if(GrapplingHook.mainGrapplingHook.grapplingLocation == null){
                            // Calculating the gravity
                            // It is multiplied by two, because we want the gravity to be stronger
                            gravityVector += Physics.gravity * 2.0f * Time.deltaTime * TimeManager.currentTimeScale;
                        }
                    }
                    else{
                        // Calculating the gravity
                        // It is multiplied by two, because we want the gravity to be stronger
                        gravityVector += Physics.gravity * 2.0f * Time.deltaTime * TimeManager.currentTimeScale;
                    }
                }
            }

            if (Physics.Raycast(GroundChecker.position, Vector3.down, out hitInfo))
            {
                if (hitInfo.distance < maxGroundDistance + maxGroundDistance)
                {
                    // Sliding
                    slopeAngle = Vector3.Angle(transform.up, hitInfo.normal);
                    if (slopeAngle > cc.slopeLimit)
                    {
                        slidingTime += Time.deltaTime * TimeManager.currentTimeScale;
                        slidingMomentum += (hitInfo.normal - transform.up) * Mathf.Clamp01(slidingTime * 1.1f) * slopeAngle * 2;
                        inputVelocity *= .1f;

                        if (Input.GetKeyDown(KeyCode.Space))
                            gravityVector += (transform.up * slopeAngle * .7f + transform.forward * slopeAngle * .3f);// * Mathf.Clamp01(slidingTime / 1.2f);// * Mathf.Clamp01(slidingTime / 2);//hitInfo.normal * jumpForce * Mathf.Clamp01(slidingTime) * slopeAngle;
                    }
                }
            }
            if (slidingMomentum != zeroVector)
            {
                slidingTime = 0.0f;
                slidingMomentum -= slidingMomentum.normalized  * 20 * Time.deltaTime * TimeManager.currentTimeScale;
                if (slidingMomentum.sqrMagnitude < 0.02f)
                    slidingMomentum = zeroVector;
            }
        }
        
        t += Time.deltaTime * TimeManager.currentTimeScale;
        if (t > .1f)
        {
            dSqr = GrapplingHook.DistanceSquared(lastFramePosition, transform.position);
            if (dSqr > 10 * TimeManager.currentTimeScale)
            {
                if (!HyperDrive.isPlaying)
                    HyperDrive.Play();
            }
            else
            {
                FOVAim = DefaultFOV;
                if (HyperDrive.isPlaying)
                    HyperDrive.Stop();
            }

            if(dSqr > 30)
                FOVAim = Mathf.Clamp(90 + dSqr / 8, DefaultFOV, maxFOV);
            else
                FOVAim = DefaultFOV;

            t = 0.0f;
            lastFramePosition = transform.position;
        }
        
        cc.Move((velocity + inputVelocity + gravityVector + slidingMomentum) * Time.deltaTime * TimeManager.currentTimeScale);
        velocity = zeroVector;

        FPSCamera.mainCamera.fieldOfView = Mathf.Lerp(
            FPSCamera.mainCamera.fieldOfView, FOVAim, Time.deltaTime * TimeManager.currentTimeScale * 8);
    }
    private Vector3 lastFramePosition;
    private float t = 0.0f, dSqr;
    
    private Vector3 zeroVector = Vector3.zero;
    public void EnableMovement()
    {
        enableMovement = true;
        gravityVector = inputVelocity = slidingMomentum = zeroVector;
    }
    
    public void DisableMovement()
    {
        enableMovement = false;
        gravityVector = inputVelocity = slidingMomentum = zeroVector;
    }
}
