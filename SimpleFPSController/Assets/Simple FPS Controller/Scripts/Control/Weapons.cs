using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapons : MonoBehaviour
{
    public static byte weaponIndex = 0;
    public Weapon[] weapons;
    private Animation weaponReviewAnimations;
    
    private bool reloading = false;
    private float reloadingTimer = 0.0f, maxDistanceToTravel;
    private Vector3 lastPosition;
    private Rigidbody fillerRb;

    public AudioSource noAmmo, shot;
    
    private RaycastHit hitInfo;
    public static Weapons mainWeapons;
    
    private void Start()
    {
        weaponIndex = 0;
        for (byte i = 0; i < weapons.Length; ++i)
            weapons[i].WeaponObj.active = false;

        ChangeWeapon(0);
        weapons[weaponIndex].AmmoUI.text = weapons[weaponIndex].leftAmmoInFiller + "/" + weapons[weaponIndex].ammo;

        mainWeapons = this;
    }

    private uint DifferenceInAmmo;
    private void Update()
    {
        if (!reloading)
        {
            if(!reviewingWeapon){
                if(Input.GetKeyDown(KeyCode.Alpha1)){
                    ChangeWeapon(0);
                }
                if(Input.GetKeyDown(KeyCode.Alpha2)){
                    ChangeWeapon(1);
                }

                if (Input.GetMouseButton(0))
                {
                    Shoot();
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    if (weapons[weaponIndex].ammo > 0)
                    {
                        DifferenceInAmmo = weapons[weaponIndex].maxAmmoInAFiller - weapons[weaponIndex].leftAmmoInFiller;
                        if (DifferenceInAmmo < weapons[weaponIndex].ammo)
                        {
                            weapons[weaponIndex].ammo -= DifferenceInAmmo;
                            weapons[weaponIndex].leftAmmoInFiller += DifferenceInAmmo;
                        }
                        else
                        {
                            weapons[weaponIndex].leftAmmoInFiller += weapons[weaponIndex].ammo;
                            weapons[weaponIndex].ammo = 0;
                        }

                        weapons[weaponIndex].AmmoUI.text = weapons[weaponIndex].leftAmmoInFiller + "/" + weapons[weaponIndex].ammo;
                        fillerRb.isKinematic = false;

                        reloading = true;
                        reloadingTimer = 0.0f;
                        lastPosition = weapons[weaponIndex].WeaponFiller.localPosition;
                    }
                }
            }

            // Weapon Review
            if(Input.GetKeyDown(KeyCode.F)){
                ReviewWeapon();
            }
        }
        else
        {
            reloadingTimer += Time.deltaTime;
            if (reloadingTimer >= weapons[weaponIndex].reloadTime / 2.0f)
            {
                if (reloadingTimer <= weapons[weaponIndex].reloadTime)
                {
                    fillerRb.isKinematic = true;
                    weapons[weaponIndex].WeaponFiller.localPosition = Vector3.Lerp(weapons[weaponIndex].WeaponFiller.localPosition, lastPosition, Time.deltaTime * 25);
                }
                else
                    reloading = false;
            }
        }
    }

    #region WeaponReview

    private bool reviewingWeapon = false;
    public void ReviewWeapon(){
        reviewingWeapon = true;
        weaponReviewAnimations.Play();
    }
    
    public void OnReviewEnd(){
        reviewingWeapon = false;
        weaponReviewAnimations.Stop();  
    }

    #endregion

    private float shootingTime = 0.0f;
    public void Shoot()
    {
        shootingTime += Time.deltaTime;
        if (shootingTime < weapons[weaponIndex].shootTime)
            return;
        shootingTime = 0;

        if(weapons[weaponIndex].leftAmmoInFiller > 0)
        {
            shot.Play();
            weapons[weaponIndex].muzzleFlash.Play();

            --weapons[weaponIndex].leftAmmoInFiller;
            weapons[weaponIndex].AmmoUI.text = weapons[weaponIndex].leftAmmoInFiller + "/" + weapons[weaponIndex].ammo;

            weapons[weaponIndex].weapon.Recoil();

            if(Physics.Raycast(transform.position, transform.forward, out hitInfo))
            {
                ReleaseNextTrucks rnt;
                if(hitInfo.transform.TryGetComponent<ReleaseNextTrucks>(out rnt))
                    rnt.ReleaseTrucks();
            }
        }
        else
        {
            // No ammo sound
            noAmmo.Play();
        }
    }

    private bool changingWeapon = false;
    public void ChangeWeapon(byte _weaponIndex_)
    {
        if(_weaponIndex_ < weapons.Length)
        {
            if(!changingWeapon){
                if(_weaponIndex_ != weaponIndex){
                    weapons[weaponIndex].WeaponObj.GetComponent<Animation>().Play("Hide");
                    weaponIndex = _weaponIndex_;
                    changingWeapon = true;
                    return;
                } else {
                    weapons[weaponIndex].WeaponObj.active = true;
                    weapons[weaponIndex].AmmoUI.text = weapons[weaponIndex].leftAmmoInFiller + "/" + weapons[weaponIndex].ammo;
                    
                    fillerRb = weapons[weaponIndex].WeaponFiller.GetComponent<Rigidbody>();
                    weaponReviewAnimations = weapons[weaponIndex].WeaponObj.GetComponent<Animation>();
                    return;
                }
            }
        }
    }

    public void OnWeaponHide(){
        // Showing the new weapon
        weapons[weaponIndex].WeaponObj.active = true;
        weapons[weaponIndex].WeaponObj.GetComponent<Animation>().Play("Show");
        weapons[weaponIndex].AmmoUI.text = weapons[weaponIndex].leftAmmoInFiller + "/" + weapons[weaponIndex].ammo;

        fillerRb = weapons[weaponIndex].WeaponFiller.GetComponent<Rigidbody>();
        weaponReviewAnimations = weapons[weaponIndex].WeaponObj.GetComponent<Animation>();
    }

    public void OnWeaponShow(){
        changingWeapon = false;
    }

    public void AddAmmo(uint newAmmo)
    {
        weapons[weaponIndex].ammo += newAmmo;
        weapons[weaponIndex].AmmoUI.text = weapons[weaponIndex].leftAmmoInFiller + "/" + weapons[weaponIndex].ammo;
    }
}

[System.Serializable]
public class Weapon
{
    [Header("Objects")]
    public GameObject WeaponObj;
    public Transform WeaponFiller;
    public ArmedWeapon weapon;

    [Header("Specification")]
    public uint maxAmmoInAFiller = 30;
    public uint ammo = 600;
    public uint leftAmmoInFiller = 30;
    public float reloadTime = 1;
    public float shootTime = 0.0f;

    [Header("Effects")]
    public Text AmmoUI;
    public ParticleSystem muzzleFlash;
}
