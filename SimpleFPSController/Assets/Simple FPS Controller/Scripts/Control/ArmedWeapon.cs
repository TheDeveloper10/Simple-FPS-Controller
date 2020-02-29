using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmedWeapon : MonoBehaviour
{
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
        movementX = Mathf.Clamp(FPSCamera.mainFPSCamera.mouseX, -maxSwayAmount, maxSwayAmount);
        movementY = Mathf.Clamp(FPSCamera.mainFPSCamera.mouseY, -maxSwayAmount, maxSwayAmount);

        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(movementX, movementY, 0) + InitialPosition, Time.deltaTime * CameraSmoothAmount);
        transform.localPosition = Vector3.Lerp(transform.localPosition, AimedPosition, SmoothAmount * 10 * Time.deltaTime);
    }

    public void Recoil()
    {
        AimedPosition = transform.localPosition + Vector3.back * .07f;
        StartCoroutine(IERecoil(.05f));
    }

    private IEnumerator IERecoil(float RecoilTime){
        WaitForEndOfFrame wfeof = new WaitForEndOfFrame();
        do{
            RecoilTime -= Time.deltaTime;
            yield return wfeof;
        } while(RecoilTime > 0.0f);
        AimedPosition = InitialPosition;
    }

    /// When the review of the weapon ends
    public void OnReviewEnd(){
        Weapons.mainWeapons.OnReviewEnd();
    }

    public void OnHide(){
        Weapons.mainWeapons.OnWeaponHide();
        this.gameObject.active = false;
    }

    public void OnShow(){
        Weapons.mainWeapons.OnWeaponShow();
    }
}
