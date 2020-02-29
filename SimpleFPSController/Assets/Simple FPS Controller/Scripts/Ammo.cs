using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    public uint ammo = 30;

    public void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            Weapons.mainWeapons.AddAmmo(ammo);
            Destroy(this.gameObject);
        }
    }
    
    private static Vector3 upVector = Vector3.up;
    private static float amplitude = .7f;

    public Vector3 initialPosition;
    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    private float timer = 0.0f;
    private void Update()
    {
        timer += Time.deltaTime * TimeManager.currentTimeScale;
        if (timer > 3.14f)
            timer -= 6.28f;

        transform.localPosition = initialPosition +
            upVector * Mathf.Sin(timer) * amplitude;
    }
}