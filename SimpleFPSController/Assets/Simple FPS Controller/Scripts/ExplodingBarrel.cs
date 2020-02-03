using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ExplodingBarrel : MonoBehaviour
{
    public float explosionRange = 15, explosionForce = 100;
    public void Explode()
    {
        Vector3 explosionPos = transform.position + Vector3.up;

        // Spawning an explosion
        ParticleSystem spawnedExplosion = Instantiate(Weapons.ExplosionFX, explosionPos, Quaternion.Euler(Vector3.zero)).GetComponent<ParticleSystem>();

        // Destroying the explosion after its duration
        Destroy(spawnedExplosion.gameObject, spawnedExplosion.duration);

        Collider[] cols = Physics.OverlapSphere(explosionPos, explosionRange);
        int i = 0;

        GrapplingHook.pvm.explosionVelocity += Vector3.ClampMagnitude(
            (PlayerMovement.cc.transform.position - explosionPos).normalized *
            Mathf.Clamp(1.0f / GrapplingHook.DistanceSquared(explosionPos, PlayerMovement.cc.transform.position), 0, .5f) * 
            explosionForce / 80.0f, 
            50);
        //GrapplingHook.pvm.gravityVector += (PlayerMovement.cc.transform.position - explosionPos).normalized * Mathf.Clamp(1.0f / GrapplingHook.DistanceSquared(explosionPos, PlayerMovement.cc.transform.position), 0, .35f) * explosionForce / 80.0f;
        //PlayerMovement.cc.Move((PlayerMovement.cc.transform.position - explosionPos).normalized * (1.0f / GrapplingHook.DistanceSquared(explosionPos, PlayerMovement.cc.transform.position)) * explosionForce / 50.0f);

        for (i = 0; i < cols.Length; ++i)
        {
            Rigidbody bufferRb = cols[i].GetComponent<Rigidbody>();
            if(bufferRb != null)
            {
                bufferRb.AddExplosionForce(explosionForce, transform.position, explosionRange);
                
                /*ExplodingBarrel expl = cols[i].GetComponent<ExplodingBarrel>();
                if(expl != null)
                {
                    expl.Explode();
                }*/
            }
        }

        Destroy(this.gameObject);
    }
}