using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;

public class Collectible : MonoBehaviour
{
    public Item item;
    public GameObject actionText;

    public bool autoCollect = false;
    public float collectionRadius = 0.8f;
    public Color lightColor;

    public float floatStrength = 0.1f;

    MushInventory touchingInventory;
    bool isCollected = false;
    float originalY;
    bool isActive = false;

    private void Start()
    {
        originalY = transform.position.y;
        Light2D light = GetComponent<Light2D>();
        light.color = lightColor;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = item.itemIcon;
        CircleCollider2D circleCollider2D = GetComponent<CircleCollider2D>();
        circleCollider2D.radius = collectionRadius;
        actionText?.SetActive(false);
        transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = item.itemName;
        isActive = false;
    }
    private void Update()
    {

        transform.position = new Vector3(transform.position.x,
                    originalY + ((float)Mathf.Sin(Time.time) * floatStrength),
                    transform.position.z);

        if (!isActive)
        {
            return;
        }

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
            if (autoCollect)
            {
                touchingInventory.AddToInventory(item);
                Destroy(gameObject);
                return;
            }
            isActive = true;
            actionText?.SetActive(true);
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            touchingInventory = null;
            isActive = false;
            actionText?.SetActive(false);
        }
    }
}
