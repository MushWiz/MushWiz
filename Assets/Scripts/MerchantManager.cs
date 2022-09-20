using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantManager : MonoBehaviour
{
    public string merchantName;
    public List<MerchantItem> itemsInStock;

    public bool sellOnSignal = false;
    public string sellSignal = "";
    public bool disableOnSignal = false;
    public string signalToDisable = "";
    bool signaled = false;
    public bool isActive = false;
    public bool hasShop = false;
    int itemsOnDisplay = 0;

    List<GameObject> itemsOnSale = new List<GameObject>();

    [Range(0, 200)] public int costOfItemsPercentage = 100;

    MushController mushController;

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
        mushController = GameObject.FindGameObjectWithTag("Player").GetComponent<MushController>();
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

        DismantleShop();

        int increment = -1;
        List<MerchantItem> stockCopy = new List<MerchantItem>(itemsInStock);
        for (int i = 0; i < 3; i++)
        {
            MerchantItem merchantItem = stockCopy[Random.Range(0, stockCopy.Count)];
            stockCopy.Remove(merchantItem);

            int statValue = 0;
            if (merchantItem.itemSold.itemType == MushInventoryType.StatBoost)
            {
                StatBoost statToBuy = (StatBoost)merchantItem.itemSold;
                statValue = mushController.GetStatValueByType(statToBuy.stat) * statToBuy.boostAmount;
            }

            Collectible placedCollectible = Instantiate(merchantItem.itemSold.collectiblePrefab, transform.position + new Vector3(increment, -1 + 0.5f * Mathf.Abs(increment), 0), Quaternion.identity).GetComponent<Collectible>();
            placedCollectible.item = merchantItem.itemSold;
            placedCollectible.consumeMicelium = true;
            placedCollectible.requiredMicelium = (int)((merchantItem.miceliumRequirement + statValue * 0.5f) * (costOfItemsPercentage / 100));
            placedCollectible.fromMerchant = true;
            placedCollectible.seller = this;
            itemsOnSale.Add(placedCollectible.gameObject);
            increment++;
            itemsOnDisplay++;
        }
        if (itemsOnDisplay > 0)
        {
            hasShop = true;
        }
    }

    public void DismantleShop()
    {
        foreach (GameObject item in itemsOnSale)
        {
            Destroy(item);
        }
        hasShop = false;
        itemsOnDisplay = 0;
    }

    public void SoldItem(Collectible itemSold)
    {
        itemsOnSale.Remove(itemSold.gameObject);

        itemsOnDisplay--;
        if (itemsOnDisplay <= 0)
        {
            SetupShop(true);
            hasShop = false;
        }
    }

    public void OnSignalReceived(GameObject source, string signal)
    {
        if (sellOnSignal && signal == sellSignal)
        {
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<CircleCollider2D>().enabled = true;
            signaled = true;
            SetupShop();
        }

        if (disableOnSignal && signal == signalToDisable)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<CircleCollider2D>().enabled = false;
            signaled = false;
            DismantleShop();
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
