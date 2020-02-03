using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public static FPSCamera fpsCam;

    private Vector3 AimedPosition, InitialPosition;

    public float maxSwayAmount;

    public float SmoothAmount = 3.0f, CameraSmoothAmount = 10;
    private float movementX, movementY;

    private void Start()
    {
        InitialPosition = transform.localPosition;
        AimedPosition = InitialPosition;
    }

    private void Update()
    {
        movementX = Mathf.Clamp(fpsCam.mouseX, -maxSwayAmount, maxSwayAmount);
        movementY = Mathf.Clamp(fpsCam.mouseY, -maxSwayAmount, maxSwayAmount);

        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(movementX, movementY, 0) + InitialPosition, Time.deltaTime * CameraSmoothAmount);
        transform.localPosition = Vector3.Lerp(transform.localPosition, AimedPosition, SmoothAmount * 10 * Time.deltaTime);
        if (RecoilTimer != 0)
        {
            RecoilTimer -= Time.deltaTime;

            if (RecoilTimer <= 0)
            {
                AimedPosition = InitialPosition;
                RecoilTimer = 0;
            }
        }
    }

    private float RecoilTimer = 0f;
    public void Recoil()
    {
        AimedPosition = transform.localPosition + Vector3.back * .07f;
        RecoilTimer = .05f;
    }
}
