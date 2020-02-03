using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FPSCamera))]
public class GrapplingHook : MonoBehaviour
{
    private Vector3 grapplingLocation;
    private readonly Vector3 zeroVector = Vector3.zero;
    private RaycastHit hitInfo;
    private float distance;

    public float maxGrapplingDistance = 100;
    public float hookSpeed = 10;
    public float dampSpeed = 3; // Momentum(Vector3) slows down using damp speed
    public CharacterController PlayerCC;
    public static PlayerMovement pvm;
    [HideInInspector] public Vector3 momentum, dir;

    public static WallRun wr;
    public LineRenderer lr;

    private void Start()
    {
        WallRun.gHook = this;
    }

    public static float DistanceSquared(Vector3 P1, Vector3 P2){
        return (P1.x - P2.x) * (P1.x - P2.x) + (P1.y - P2.y) * (P1.y - P2.y) + (P1.z - P2.z) * (P1.z - P2.z);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(transform.position, transform.forward, out hitInfo, maxGrapplingDistance))
            {
                grapplingLocation = hitInfo.point;
                pvm.DisableMovement();
                momentum = zeroVector;
                wr.EndWallRun();
                goto _decreaseMagnitude;
            }
            goto _decreaseMagnitude;
        }

        if (Input.GetKey(KeyCode.E))
        {
            if (grapplingLocation != zeroVector)
            {
                distance = DistanceSquared(grapplingLocation, transform.position);

                dir = (grapplingLocation - transform.position).normalized;
                momentum = dir * hookSpeed * Mathf.Clamp01(distance);
                lr.SetPosition(0, transform.position - Vector3.up);
                lr.SetPosition(1, grapplingLocation);

                goto _decreaseMagnitude;
            }
            //return;
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            if(grapplingLocation != zeroVector)
                Unhook();
            goto _decreaseMagnitude;
        }

        _decreaseMagnitude:
        if (momentum.sqrMagnitude >= 0.0f)
        {
            momentum -= momentum * dampSpeed * Time.deltaTime * TimeManager.currentTimeScale;
            if (momentum.sqrMagnitude < 0.0f)
                momentum = zeroVector;
            pvm.velocity += momentum;
            //PlayerCC.Move(momentum * Time.deltaTime);
        }
    }

    public void Unhook()
    {
        grapplingLocation = zeroVector;
        pvm.EnableMovement();
        lr.SetPosition(0, zeroVector);
        lr.SetPosition(1, zeroVector);
    }
}
