using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FPSCamera))]
public class GrapplingHook : MonoBehaviour
{
    public static GrapplingHook mainGrapplingHook;

    [HideInInspector] public Transform grapplingLocation;
    private readonly Vector3 zeroVector = Vector3.zero;
    private RaycastHit hitInfo;
    private float distance;

    public float maxGrapplingDistance = 100;
    public float hookSpeed = 10;
    public float dampSpeed = 3; // Momentum(Vector3) slows down using damp speed
    public CharacterController PlayerCC;
    [HideInInspector] public Vector3 momentum, dir;

    public LineRenderer lr;

    private void Start()
    {
        mainGrapplingHook = this;
    }

    public static float DistanceSquared(Vector3 P1, Vector3 P2){
        return (P1.x - P2.x) * (P1.x - P2.x) + (P1.y - P2.y) * (P1.y - P2.y) + (P1.z - P2.z) * (P1.z - P2.z);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(transform.position, transform.forward, out hitInfo, maxGrapplingDistance))
            {
                grapplingLocation = Instantiate(new GameObject(), hitInfo.point, Quaternion.identity).transform;
                grapplingLocation.parent = hitInfo.transform;
                PlayerMovement.mainPlayerMovement.DisableMovement();
                momentum = zeroVector;
                if(WallRun.mainWallRun != null)
                    WallRun.mainWallRun.EndWallRun();
                goto _decreaseMagnitude;
            }
            goto _decreaseMagnitude;
        }

        if (Input.GetKey(KeyCode.E))
        {
            if (grapplingLocation != null)
            {
                distance = DistanceSquared(grapplingLocation.position, transform.position);

                dir = (grapplingLocation.position - transform.position).normalized;
                momentum = dir * hookSpeed * Mathf.Clamp01(distance);
                lr.SetPosition(0, transform.position - Vector3.up);
                lr.SetPosition(1, grapplingLocation.position);

                goto _decreaseMagnitude;
            }
            //return;
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            if(grapplingLocation != null)
                Unhook();
            goto _decreaseMagnitude;
        }

        _decreaseMagnitude:
        if (momentum.sqrMagnitude >= 0.0f)
        {
            momentum -= momentum * dampSpeed * Time.deltaTime * TimeManager.currentTimeScale;
            if (momentum.sqrMagnitude < 0.0f)
                momentum = zeroVector;
            PlayerMovement.mainPlayerMovement.velocity += momentum;
            //PlayerCC.Move(momentum * Time.deltaTime);
        }
    }

    public void Unhook()
    {
        if(grapplingLocation != null)
            Destroy(grapplingLocation.gameObject);
        PlayerMovement.mainPlayerMovement.EnableMovement();
        lr.SetPosition(0, zeroVector);
        lr.SetPosition(1, zeroVector);
    }
}
