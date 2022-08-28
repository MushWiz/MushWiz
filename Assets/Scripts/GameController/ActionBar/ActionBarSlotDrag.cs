using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActionBarSlotDrag : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public ActionBarManager actionBarManager;

    public void OnPointerDown(PointerEventData eventData)
    {
        actionBarManager.dragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        actionBarManager.dragging = false;
        //Use the event data raycast results to see if we hit another slot
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            GameObject otherSlot = eventData.pointerCurrentRaycast.gameObject;
            if (otherSlot != null && otherSlot != gameObject && otherSlot.CompareTag("ActionBarSlot"))
            {
                actionBarManager.SwapAbilities(gameObject, otherSlot);
            }
        }
    }
}