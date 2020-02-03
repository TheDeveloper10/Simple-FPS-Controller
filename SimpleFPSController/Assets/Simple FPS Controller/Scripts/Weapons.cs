using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapons : MonoBehaviour
{
    public static byte weaponIndex = 0;
    public Weapon[] weapons;
    
    private bool reloading = false;
    private float reloadingTimer = 0.0f, maxDistanceToTravel;
    private Vector3 lastPosition;
    private Rigidbody fillerRb;

    public AudioSource noAmmo, shot;
    
    private RaycastHit hitInfo;

    [Header("Effects")]
    public GameObject Explosion;
    public static GameObject ExplosionFX;

    private void Awake()
    {
        ExplosionFX = Explosion;
    }

    void Start()
    {
        for (byte i = 0; i < weapons.Length; ++i)
            weapons[i].WeaponObj.active = false;

        ChangeWeapon(0);
        weapons[weaponIndex].AmmoUI.text = weapons[weaponIndex].leftAmmoInFiller + "/" + weapons[weaponIndex].ammo;

        Ammo._weapons_ = this;
    }

    private uint DifferenceInAmmo;
    void Update()
    {
        if (!reloading)
        {
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

            weapons[weaponIndex].weaponSway.Recoil();

            if(Physics.Raycast(transform.position, transform.forward, out hitInfo))
            {
                ExplodingBarrel expl;
                if (hitInfo.transform.TryGetComponent<ExplodingBarrel>(out expl))
                    expl.Explode();
            }
        }
        else
        {
            // No ammo sound
            noAmmo.Play();
        }

        /*if (weapons[weaponIndex].leftAmmoInFiller != 0)
        {
            --weapons[weaponIndex].leftAmmoInFiller;
            weapons[weaponIndex].Ammo.text = weapons[weaponIndex].leftAmmoInFiller + "/" + 
                ((int)(weapons[weaponIndex].maxAmmo * (weapons[weaponIndex].fillers - 1)) + weapons[weaponIndex].leftAmmoInFiller);
            //weapons[weaponIndex].muzzleFlash.Play();

            weapons[weaponIndex].weaponSway.Recoil();
        }
        else
        {
            // No ammo sound
            weapons[weaponIndex].Ammo.text = weapons[weaponIndex].leftAmmoInFiller + "/" +
                ((int)(weapons[weaponIndex].maxAmmo * (weapons[weaponIndex].fillers - 1)) + weapons[weaponIndex].leftAmmoInFiller);

        }*/
    }

    public void ChangeWeapon(byte _weaponIndex_)
    {
        if(_weaponIndex_ < weapons.Length)
        {
            weapons[_weaponIndex_].WeaponObj.active = false;
            weaponIndex = _weaponIndex_;
            weapons[_weaponIndex_].WeaponObj.active = true;

            fillerRb = weapons[weaponIndex].WeaponFiller.GetComponent<Rigidbody>();
            weapons[weaponIndex].AmmoUI.text = weapons[weaponIndex].leftAmmoInFiller + "/" + weapons[weaponIndex].ammo;
        }
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
    public WeaponSway weaponSway;

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
