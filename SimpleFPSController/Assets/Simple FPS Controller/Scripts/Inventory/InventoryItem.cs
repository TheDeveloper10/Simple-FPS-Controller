using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour {
    public int slotX = 0, slotY = 0;

    private RectTransform rectTransform;
    private EventTrigger eventTrigger;
    private CanvasGroup canvasGroup;

    private void Start(){
        rectTransform = this.GetComponent<RectTransform>();

        eventTrigger = this.gameObject.AddComponent<EventTrigger>();
        canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
        
        // I am using methods to add all of
        // the events because it doesn't work
        // properly without them
        AddOnPointerClick();

        AddOnBeginDrag();
        AddOnDrag();
        AddOnEndDrag();
    }

    #region AddingEvents
    private void AddOnPointerClick(){
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener( (eventData) => { OnPointerClick((PointerEventData)eventData); } );
        eventTrigger.triggers.Add(entry);
    }

    private void AddOnBeginDrag(){
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.BeginDrag;
        entry.callback.AddListener( (eventData) => { OnBeginDrag((PointerEventData)eventData); } );
        eventTrigger.triggers.Add(entry);
    }

    private void AddOnDrag(){
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drag;
        entry.callback.AddListener( (eventData) => { OnDrag((PointerEventData)eventData); } );
        eventTrigger.triggers.Add(entry);
    }

    private void AddOnEndDrag(){
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.EndDrag;
        entry.callback.AddListener( (eventData) => { OnEndDrag((PointerEventData)eventData); } );
        eventTrigger.triggers.Add(entry);
    }
    #endregion

    #region Events
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left){
            // Double click
            if(eventData.clickCount >= 2){
                // Use item
                if(ItemsManager.mainItemManager.UseItem(slotX, slotY) != -1){
                    this.gameObject.active = false;
                    return;
                }
                return;
            }
        }
    }

    private Vector3 initialDifference, initialPosition;
    public void OnBeginDrag(PointerEventData data) {
        initialPosition = rectTransform.position;
        initialDifference = rectTransform.position - new Vector3(data.position.x, data.position.y, 0.0f);
        transform.SetSiblingIndex(transform.parent.childCount - 1);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData data) {
        if(ItemsManager.selectedSlotX == slotX && ItemsManager.selectedSlotY == slotY){ 
            // The item is on the same slot
            rectTransform.position = initialPosition;
            initialDifference = initialPosition = Vector3.zero;
            canvasGroup.blocksRaycasts = true;
            return;
        }
        else if(ItemsManager.selectedSlotX == -1 || ItemsManager.selectedSlotY == -1){ 
            // Drop item
            
            ItemsManager.mainItemManager.DropItem(slotX, slotY);

            rectTransform.position = initialPosition;
            initialDifference = initialPosition = Vector3.zero;
            canvasGroup.blocksRaycasts = true;
            this.gameObject.active = false;
            return;
        }
        else{ 
            // Set this item to a new slot
            ItemsManager.mainItemManager.ChangeItemPosition(
                slotX,
                slotY,
                ItemsManager.selectedSlotX,
                ItemsManager.selectedSlotY
            );

            rectTransform.position = initialPosition;
            initialDifference = initialPosition = Vector3.zero;
            canvasGroup.blocksRaycasts = true;

            // Showing the replaced item
            Inventory.mainInventory.ChangeItemSlot(slotX, slotY, ItemsManager.selectedSlotX, ItemsManager.selectedSlotY);
            return;
        }
    }

    public void OnDrag(PointerEventData data) {
        rectTransform.position = new Vector3(data.position.x, data.position.y, 0.0f) + initialDifference;
    }
    #endregion
}