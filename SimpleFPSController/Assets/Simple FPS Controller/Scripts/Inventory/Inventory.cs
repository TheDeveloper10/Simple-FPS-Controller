using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    public static Inventory mainInventory;

    public const int inventoryWidth = 4, inventoryHeight = 4;

    public GameObject[] itemSlots;

     // Open and Close inventory keybind
    public KeyCode inventory; 

    public GameObject inventoryObj;

    private void Start(){
        mainInventory = this;
    }

    private float timeScaleBackup = 0.0f;
    private bool isCursorVisible =  false;
    private CursorLockMode cursorLockMode;
    private void Update(){
        if(Input.GetKeyDown(inventory)){
            if(!TimeManager.mainTimeManager.changingTime){
                // if on the next step it'll become active
                if(!inventoryObj.active){
                    timeScaleBackup = TimeManager.currentTimeScale;
                    TimeManager.mainTimeManager.SetTimeScale(0.001f, 0.5f);
                    ShowItems();

                    isCursorVisible = Cursor.visible;
                    cursorLockMode = Cursor.lockState;

                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                inventoryObj.active = !inventoryObj.active;
                // if it is not active
                if(!inventoryObj.active)
                {
                    TimeManager.mainTimeManager.SetTimeScale(timeScaleBackup, 0.5f);
                    HideItems();

                    Cursor.visible = isCursorVisible;
                    Cursor.lockState = cursorLockMode;
                }
                // The ifs are situated like that(not in if-else form), because
                // you want to show the items before showing the inventory and
                // you also want to hide the items after closing the inventory
            }
        }
    }

    public void HideItems(){
        for(int i = 0 ; i < itemSlots.Length ; ++i)
            itemSlots[i].active = false;
    }

    public void ShowItems(){
        int slotIndex;
        for(int i = 0 ; i < ItemsManager.mainItemManager.items.Count ; ++i){
            slotIndex = ItemsManager.mainItemManager.items[i].slotY * inventoryWidth + 
            ItemsManager.mainItemManager.items[i].slotX;
            
            itemSlots[slotIndex].GetComponent<Image>().sprite = 
            ItemsManager.mainItemManager.itemData[ItemsManager.mainItemManager.items[i].itemDataIndex].itemImage;
            itemSlots[slotIndex].active = true;
        }
    }

    public void ChangeItemSlot(int currentSlotX, int currentSlotY, int newSlotX, int newSlotY){
        int currentIndex = currentSlotY * inventoryWidth + currentSlotX,
            newIndex = newSlotY * inventoryWidth + newSlotX;
        itemSlots[newIndex].GetComponent<Image>().sprite = itemSlots[currentIndex].GetComponent<Image>().sprite;
        
        itemSlots[currentIndex].active = false;
        itemSlots[newIndex].active = true;
    }
}