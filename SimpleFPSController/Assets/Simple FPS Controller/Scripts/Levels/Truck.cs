using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour {
    public static float speed;
    private static bool speedDefined = false;

    public bool release = false;

    private void Start(){
        if(!speedDefined){
            speed = Random.Range(20, 30);
            speedDefined = true;
        }
    }

    private void Update(){
        if(release)
            transform.localPosition+=transform.forward * speed * Time.deltaTime;
    }
}