using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MushInventorySlotDrag : EventTrigger
{
    public MushInventory mushInventory;

    public float overlayWaitTime = 0.5f;
    public float overlayTimer = 0;
    public bool overlayActive = false;
    public bool overlayShown = false;

    public override void OnPointerDown(PointerEventData eventData)
    {
        mushInventory.dragging = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        mushInventory.dragging = false;
        //Use the event data raycast results to see if we hit another slot
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            GameObject otherSlot = eventData.pointerCurrentRaycast.gameObject;
            if (otherSlot != null && otherSlot != gameObject && otherSlot.tag == "InventorySlot")
            {
                mushInventory.SwapInventories(gameObject, otherSlot);
            }
        }
    }

    private void Update()
    {
        if (overlayActive)
        {
            overlayTimer += Time.deltaTime;
        }

        if (overlayTimer >= overlayWaitTime && !overlayShown)
        {
            mushInventory.ShowOverlay(gameObject);
            overlayShown = true;
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        overlayActive = true;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        overlayActive = false;
        overlayTimer = 0;
        overlayShown = false;
        mushInventory.HideOverlay();
    }

}