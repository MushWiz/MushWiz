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
    public bool flyToTarget = false;
    [HideInInspector] public GameObject target;
    public bool consumeMicelium = false;
    public int requiredMicelium = 0;
    public float collectionRadius = 0.8f;
    public Color lightColor;

    public float floatStrength = 0.1f;

    MushInventory touchingInventory;
    bool isCollected = false;
    float originalY;
    bool isActive = false;
    [HideInInspector] public bool fromMerchant = false;
    [HideInInspector] public MerchantManager seller;

    private void Start()
    {
        originalY = transform.position.y;
        Light2D light = GetComponent<Light2D>();
        light.color = lightColor;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = item.itemIcon;
        CircleCollider2D circleCollider2D = GetComponent<CircleCollider2D>();
        circleCollider2D.radius = collectionRadius;
        if (actionText)
        {
            actionText.SetActive(false);
            transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = item.itemName;
        }
        isActive = false;
    }
    private void Update()
    {
        if (!flyToTarget)
        {
            transform.position = new Vector3(transform.position.x,
                    originalY + ((float)Mathf.Sin(Time.time) * floatStrength),
                    transform.position.z);
        }
        else
        {
            Vector2 direction = (target.transform.position - transform.position).normalized;
            transform.Translate(direction * 0.1f * (1 / Vector2.Distance(target.transform.position, transform.position)));
        }

        if (!isActive)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.E) && !isCollected)
        {
            if (consumeMicelium)
            {
                if (!touchingInventory.UseCurrency(requiredMicelium))
                {
                    return;
                }
            }

            if (touchingInventory.AddToInventory(item))
            {
                isCollected = true;
                if (fromMerchant)
                {
                    seller?.SoldItem();
                }
                Destroy(gameObject);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            touchingInventory = other.gameObject.GetComponent<MushController>().controller.uIHandler.mushInventory;
            if (autoCollect)
            {
                touchingInventory.AddToInventory(item);
                Destroy(gameObject);
                return;
            }
            isActive = true;
            if (actionText)
            {
                actionText.SetActive(true);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            touchingInventory = null;
            isActive = false;
            if (actionText)
            {
                actionText.SetActive(false);
            }
        }
    }
}
