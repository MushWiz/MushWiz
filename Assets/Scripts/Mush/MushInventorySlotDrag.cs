using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MushInventorySlotDrag : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public MushInventory mushInventory;

    public void OnPointerDown(PointerEventData eventData)
    {
        mushInventory.dragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
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
}