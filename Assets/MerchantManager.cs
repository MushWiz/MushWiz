using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantManager : MonoBehaviour
{
    public string merchantName;
    public List<MerchantItem> itemsInStock;

    public bool sellOnSignal = false;
    public string sellSignal = "";
    bool signaled = false;
    public bool isActive = false;
    public bool hasShop = false;
    int itemsOnDisplay = 0;

    private void Awake()
    {
        GameStateManager.Instance.OnSignalReceived += OnSignalReceived;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnSignalReceived -= OnSignalReceived;
    }

    private void Start()
    {
        if (sellOnSignal)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<CircleCollider2D>().enabled = false;
        }
        SetupShop();
    }

    private void Update()
    {
        if (!isActive || hasShop)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            SetupShop(true);
        }
    }

    public void SetupShop(bool forced = false)
    {
        if (!forced && (sellOnSignal && !signaled))
        {
            return;
        }
        int increment = -1;
        List<MerchantItem> stockCopy = new List<MerchantItem>(itemsInStock);
        for (int i = 0; i < 3; i++)
        {
            MerchantItem merchantItem = stockCopy[Random.Range(0, stockCopy.Count)];
            stockCopy.Remove(merchantItem);
            Collectible placedCollectible = Instantiate(merchantItem.itemSold.collectiblePrefab, transform.position + new Vector3(increment, -1, 0), Quaternion.identity).GetComponent<Collectible>();
            placedCollectible.item = merchantItem.itemSold;
            placedCollectible.consumeMicelium = true;
            placedCollectible.requiredMicelium = merchantItem.miceliumRequirement;
            placedCollectible.fromMerchant = true;
            placedCollectible.seller = this;
            increment++;
            itemsOnDisplay++;
        }
        if (itemsOnDisplay > 0)
        {
            hasShop = true;
        }
    }

    public void SoldItem()
    {
        itemsOnDisplay--;
        if (itemsOnDisplay <= 0)
        {
            hasShop = false;
        }
    }

    public void OnSignalReceived(GameObject source, string signal)
    {
        if (signal == sellSignal)
        {
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<CircleCollider2D>().enabled = true;
            signaled = true;
            SetupShop();
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isActive = true;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isActive = false;
        }
    }
}
