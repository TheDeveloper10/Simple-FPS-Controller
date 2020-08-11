/*
     ██████╗ ██████╗  █████╗ ██████╗ ██████╗ ██╗     ██╗███╗  ██╗ ██████╗   ██╗  ██╗ █████╗  █████╗ ██╗  ██╗
    ██╔════╝ ██╔══██╗██╔══██╗██╔══██╗██╔══██╗██║     ██║████╗ ██║██╔════╝   ██║  ██║██╔══██╗██╔══██╗██║ ██╔╝
    ██║  ██╗ ██████╔╝███████║██████╔╝██████╔╝██║     ██║██╔██╗██║██║  ██╗   ███████║██║  ██║██║  ██║█████═╝ 
    ██║  ╚██╗██╔══██╗██╔══██║██╔═══╝ ██╔═══╝ ██║     ██║██║╚████║██║  ╚██╗  ██╔══██║██║  ██║██║  ██║██╔═██╗ 
    ╚██████╔╝██║  ██║██║  ██║██║     ██║     ███████╗██║██║ ╚███║╚██████╔╝  ██║  ██║╚█████╔╝╚█████╔╝██║ ╚██╗
     ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝     ╚═╝     ╚══════╝╚═╝╚═╝  ╚══╝ ╚═════╝   ╚═╝  ╚═╝ ╚════╝  ╚════╝ ╚═╝  ╚═╝

    █▄▄ █▄█   ▀█▀ █ █ █▀▀   █▀▄ █▀▀ █ █ █▀▀ █   █▀█ █▀█ █▀▀ █▀█
    █▄█  █     █  █▀█ ██▄   █▄▀ ██▄ ▀▄▀ ██▄ █▄▄ █▄█ █▀▀ ██▄ █▀▄
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SFPSC_PlayerMovement))] // PlayerMovement also requires Rigidbody
public class SFPSC_GrapplingHook : MonoBehaviour
{
    private SFPSC_PlayerMovement pm;
    private Rigidbody rb;
    private int segments;
    private void Start()
    {
        segments = rope.segments;
        pm = this.GetComponent<SFPSC_PlayerMovement>();
        rb = this.GetComponent<Rigidbody>();
    }

    private bool isGrappling = false;
    private void Update()
    {
        if (crossHairSpinningPart != null)
        {
            // we need 2 raycasts bc w/ 1 you can grapple through colliders which isn't good
            if (Physics.Raycast(SFPSC_FPSCamera.cam.transform.position, SFPSC_FPSCamera.cam.transform.forward, out hitInfo, maxGrappleDistance, layerMask))
            {
                hitName = hitInfo.collider.name;
                if (Physics.Raycast(SFPSC_FPSCamera.cam.transform.position, SFPSC_FPSCamera.cam.transform.forward, out hitInfo, maxGrappleDistance))
                {
                    if (hitName != hitInfo.collider.name)
                        goto _else;
                    crossHairSpinningPart.gameObject.SetActive(true);
                    crossHairSpinningPart.Rotate(Vector3.forward * crossHairSpinSpeed * Time.deltaTime);
                    goto _out;
                }
            }

            _else:
            crossHairSpinningPart.gameObject.SetActive(false);
        }
        _out:

        if (!isGrappling)
        {
            if (Input.GetKeyDown(SFPSC_KeyManager.Grapple))
                Grapple();
            
            return;
        }
        else
        {
            if (!Input.GetKey(SFPSC_KeyManager.Grapple))
                UnGrapple();
            GrappleUpdate();

            return;
        }
    }

    [Header("Properties")]
    public float maxGrappleDistance = 100.0f;
    public SFPSC_Rope rope;
    public float maximumSpeed = 100.0f;
    public float deceleration = 2500.0f; // This is how much the player is going to decelerate after stopped grappling
    public float deceleratingTime = 1.4f; // This is the time the decelerating is going to act on the player after stopped grappling
    public RectTransform crossHairSpinningPart;
    public float crossHairSpinSpeed = 200.0f;
    public float distanceToStop = 2.0f;
    public LayerMask layerMask;
    public float grappleCooldown = 1.0f;
    private bool isBlocked = false;

    private Transform location; // the grappled location
    private RaycastHit hitInfo;
    private string hitName;
    public void Grapple()
    {
        if (isBlocked)
            return;

        // we need 2 raycasts bc w/ 1 you can grapple through colliders which isn't good
        if (Physics.Raycast(SFPSC_FPSCamera.cam.transform.position, SFPSC_FPSCamera.cam.transform.forward, out hitInfo, maxGrappleDistance, layerMask))
        {
            hitName = hitInfo.collider.name;
            if (Physics.Raycast(SFPSC_FPSCamera.cam.transform.position, SFPSC_FPSCamera.cam.transform.forward, out hitInfo, maxGrappleDistance))
            {
                if (hitName != hitInfo.collider.name)
                    return;
                // We create a GameObject and we parent it to the grappled object. 
                // If we don't parent it to the object and the object moves the player is stuck only on one location instead of the moving object.
                location = new GameObject().transform;//Instantiate(new GameObject(), hitInfo.point, Quaternion.identity).transform;
                location.position = hitInfo.point;
                location.parent = hitInfo.collider.transform;

                if (decelerateTimer != 0.0f)
                    StopCoroutine(Decelerate());
                pm.DisableMovement();
                // Rope attaching
                rope.segments = (int)((hitInfo.distance / maxGrappleDistance) * segments);
                rope.Grapple(transform.position, hitInfo.point);

                rb.useGravity = false;
                isGrappling = true;
            }
        }
    }

    private Vector3 grappleForce;
    public void UnGrapple()
    {
        if (!isGrappling)
            return;
        if (location != null)
            Destroy(location.gameObject);
        if (decelerateTimer == 0.0f)
            StartCoroutine(Decelerate());
        else
            decelerateTimer = 0.0f;

        pm.EnableMovement();
        // Rope detaching
        rope.UnGrapple();

        Invoke("UnblockGrapple", grappleCooldown);
        
        rb.useGravity = true;
        isGrappling = false;
    }

    private void UnblockGrapple()
    {
        isBlocked = false;
    }

    private float decelerateTimer = 0.0f, max;
    private IEnumerator Decelerate()
    {
        WaitForEndOfFrame wfeof = new WaitForEndOfFrame();
        max = deceleratingTime * Mathf.Clamp01(targetDistance / 10.0f) * Mathf.Clamp01(rb.velocity.magnitude / 30.0f);
        for (; decelerateTimer < max; decelerateTimer += Time.deltaTime)
        {
            rb.AddForce(-rb.velocity.normalized * deceleration * (1.0f - decelerateTimer / max) * Mathf.Clamp01(rb.velocity.sqrMagnitude / 400.0f) * Time.deltaTime, ForceMode.Acceleration);
            yield return wfeof;
        }
        decelerateTimer = 0.0f;
    }

    private Vector3 dir;

    private float speed = 0.0f, targetDistance;
    private void GrappleUpdate()
    {
        if (location == null)
            return;
        
        targetDistance = Vector3.Distance(transform.position, location.position);
        rope.segments = (int)((targetDistance / maxGrappleDistance) * segments);
        dir = (location.position - transform.position).normalized;
        
        rb.velocity = Vector3.Lerp(rb.velocity, dir * maximumSpeed * Mathf.Clamp01(targetDistance / (4.0f * distanceToStop)), Time.deltaTime);

        // Rope updating
        rope.UpdateStart(transform.position);
        rope.UpdateGrapple();
    }

    private Vector3 ClampMag(Vector3 vec, float maxMag)
    {
        if (vec.sqrMagnitude > maxMag * maxMag)
            vec = vec.normalized * maxMag;
        return vec;
    }
}
