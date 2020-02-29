using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ReleaseNextTrucks : MonoBehaviour{
    public Truck[] trucks;
    public Material releasedMaterial;

    public void ReleaseTrucks(){
        for(int i = 0 ; i < trucks.Length ; ++i)
            trucks[i].release = true;
        MeshRenderer meshRenderer = this.GetComponent<MeshRenderer>();
        Material[] mats = meshRenderer.materials;
        mats[0] = releasedMaterial;
        meshRenderer.materials = mats;
    }
}