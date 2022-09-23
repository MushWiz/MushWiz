using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MushInventoryItemOverlay : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI itemName;

    public void SetItem(Item item)
    {
        if (item == null)
        {
            itemIcon.sprite = null;
            itemName.text = "";
            return;
        }
        itemIcon.sprite = item.itemIcon;
        itemName.text = item.itemName;
    }

    public void ClearItem()
    {
        itemIcon.sprite = null;
        itemName.text = "";
    }

    public void ShowOverlay(Item item)
    {
        SetItem(item);

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);

        gameObject.SetActive(true);
    }

    public void HideOverlay()
    {
        ClearItem();
        gameObject.SetActive(false);
    }
}