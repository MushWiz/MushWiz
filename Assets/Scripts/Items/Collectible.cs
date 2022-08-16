using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public Item item;
    bool isCollected = false;

    public GameObject actionText;

    MushInventory touchingInventory;

    private void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = item.itemIcon;
        actionText.SetActive(false);
        enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isCollected)
        {
            isCollected = true;
            if (touchingInventory.AddToInventory(item))
            {
                Destroy(gameObject);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            touchingInventory = other.gameObject.GetComponent<MushController>().controller.uIHandler.mushInventory;
            enabled = true;
            actionText.SetActive(true);
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            touchingInventory = null;
            enabled = false;
            actionText.SetActive(false);
        }
    }
}
