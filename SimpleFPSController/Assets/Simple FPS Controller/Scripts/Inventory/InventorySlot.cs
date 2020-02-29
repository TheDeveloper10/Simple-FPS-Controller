using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class InventorySlot : MonoBehaviour {
    public int slotX, slotY;
    private EventTrigger eventTrigger;
    private RectTransform rectTransform;

    private void Start() {
        eventTrigger = this.gameObject.AddComponent<EventTrigger>();

        rectTransform = this.GetComponent<RectTransform>();

        AddOnPointerEnter();
        AddOnPointerExit();
    }

    #region AddingEvents

    private void AddOnPointerEnter(){
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener( (eventData) => { OnPointerEnter((PointerEventData)eventData); } );
        eventTrigger.triggers.Add(entry);
    }

    private void AddOnPointerExit(){
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener( (eventData) => { OnPointerExit((PointerEventData)eventData); } );
        eventTrigger.triggers.Add(entry);
    }

    #endregion

    #region Events

    public void OnPointerEnter(PointerEventData eventData){
        ItemsManager.selectedSlotX = slotX;
        ItemsManager.selectedSlotY = slotY;
        ItemsManager.selectedSlotPosition = rectTransform.position;
    }

    public void OnPointerExit(PointerEventData eventData){
        if(ItemsManager.selectedSlotX == slotX && ItemsManager.selectedSlotY == slotY){
            ItemsManager.selectedSlotX = ItemsManager.selectedSlotY = -1;
            ItemsManager.selectedSlotPosition = Vector3.zero;
        }
    }

    #endregion
}