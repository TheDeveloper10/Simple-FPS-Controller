using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class DroppedItem : MonoBehaviour {
    public Item item;

    public const float maxAmplitude = 0.5f;

    private void OnTriggerEnter(Collider col){
        ItemsManager.mainItemManager.PickUpUIShow(this);
    }

    private void OnTriggerExit(Collider col){
        ItemsManager.mainItemManager.PickUpUIHide();
    }
}