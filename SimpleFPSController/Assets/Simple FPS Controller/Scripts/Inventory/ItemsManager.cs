using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour {

    public static ItemsManager mainItemManager;

    public ItemData[] itemData;
    public List<Item> items;

    // if the value is -1 than there is not a selected slot
    public static int selectedSlotX = 0, selectedSlotY = 0;
    public static Vector3 selectedSlotPosition;

    private void Start(){
        mainItemManager = this;
    }

    public void AddItem(Item newItem){
        items.Add(newItem);
    }

    public void RemoveItem(int itemIndex){
        items.RemoveAt(itemIndex);
    }

    #region PickUpItem

    public GameObject PickUpUI;
    private Animation PickUpAnimations;
    private bool isPickupUIShown = false;

    public void PickUpUIShow(DroppedItem droppedItem){
        if(!isPickupUIShown){
            PickUpAnimations = PickUpUI.GetComponent<Animation>();
            PickUpAnimations.PlayQueued("ShowUp");
            isPickupUIShown = true;

            StartCoroutine(CheckForPickup(droppedItem));
        }
    }

    private bool isSlotFree(int slotX, int slotY){
        for(int i = 0 ; i < items.Count ; ++i)
            if(items[i].slotX == slotX)
                if(items[i].slotY == slotY)
                    return false;
        return true;
    }

    private IEnumerator CheckForPickup(DroppedItem droppedItem){
        WaitForEndOfFrame wfeof = new WaitForEndOfFrame();
        do{
            if(Input.GetKey(KeyCode.G)){
                if(items.Count >= Inventory.mainInventory.itemSlots.Length){
                    // Not enough space in the inventory
                    goto _Wfeof;
                }

                // Finding a free slot
                int gridX = -1, gridY = -1, i = 0, j = 0;
                for(; i < Inventory.inventoryWidth ; ++i){
                    for(; j < Inventory.inventoryHeight ; ++j){
                        if(isSlotFree(i, j))
                        {
                            gridX = i;
                            gridY = j;
                            // skipping the next steps
                            i = Inventory.inventoryWidth;
                            j = Inventory.inventoryHeight;
                        }
                    }
                }
                // The inventory is full
                if(gridX != -1 && gridY != -1){
                    items.Add(new Item(gridX, gridY, droppedItem.item.itemDataIndex));

                    Destroy(droppedItem.gameObject);
                }
                PickUpUIHide();
            }
            _Wfeof:
            yield return wfeof;
        } while(isPickupUIShown);
    }

    public void PickUpUIHide(){
        if(isPickupUIShown){
            PickUpAnimations.PlayQueued("Hide");
            isPickupUIShown = false;
        }
    }

    #endregion

    #region ItemUsing
    public int UseItem(int gridX, int gridY){
        for(int i = 0 ; i < items.Count ; ++i) {
            if(items[i].slotX == gridX) {
                if(items[i].slotY == gridY) {

                    ItemStats itemStats = itemData[
                        items[i].itemDataIndex
                        ].itemStats;

                    int returnValue = -1;
                    if(itemStats.drugIndex != -1)
                        returnValue = PillEffects.mainPillEffects.UseDrug(itemStats.drugIndex);
                    
                    if(returnValue != -1)
                        items.RemoveAt(i);
                    return returnValue;
                }
            }
        }
        return -1;
    }
    #endregion

    // Drops the item on the ground
    public void DropItem(int slotX, int slotY) {
        for(int i = 0 ; i < items.Count ; ++i){
            if(items[i].slotX == slotX){
                if(items[i].slotY == slotY){
                    Instantiate(itemData[items[i].itemDataIndex].droppedItemPrefab, 
                    this.transform.position + transform.forward, 
                    Quaternion.identity);

                    items.RemoveAt(i);

                    return;
                }
            }
        }
    }

    public void ChangeItemPosition(int currentX, int currentY, int newX, int newY){
        for(int i = 0 ; i < items.Count ; ++i) {
            if(items[i].slotX == currentX) {
                if(items[i].slotY == currentY) {
                    items[i].slotX = newX;
                    items[i].slotY = newY;
                    return;
                }
            }
        }
    }
}

[System.Serializable]
public class Item {
    // Coordinates in the Inventory grid
    public int slotX, slotY;

    // The index of itemData
    public int itemDataIndex;

    public Item(int slotX_ = 0, int slotY_ = 0, int itemDataIndex_ = 0){
        slotX = slotX_;
        slotY = slotY_;
        itemDataIndex = itemDataIndex_;
    }
}

[System.Serializable]
public class ItemData{
    public Sprite itemImage;
    public ItemStats itemStats;
    public GameObject droppedItemPrefab;
}

[System.Serializable]
public class ItemStats{
    public int drugIndex = -1;
}