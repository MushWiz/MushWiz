using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Interactable : MonoBehaviour
{
    public string SignalToSend;
    public bool reversable = true;
    public bool activateOnPass = false;
    public bool canBeActivatedByObjects = false;
    public string workingObjectTag = "";
    public float activationRadius = 0.3f;
    public GameObject activationText;

    public Sprite startingSprite;
    public Sprite activeSprite;

    public bool disableOnSignal = false;
    public string signalToDisable = "";
    bool disabled = false;

    bool canBeTriggered = false;
    bool triggered = false;

    private void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = startingSprite;
        if (activationText)
        {
            activationText.SetActive(false);
        }
        CircleCollider2D circleCollider2D = GetComponent<CircleCollider2D>();
        circleCollider2D.radius = activationRadius;
    }

    private void Awake()
    {
        if (disableOnSignal)
        {
            GameStateManager.Instance.OnSignalReceived += OnSignalReceived;
        }
    }

    private void OnDestroy()
    {
        if (disableOnSignal)
        {
            GameStateManager.Instance.OnSignalReceived -= OnSignalReceived;
        }
    }

    private void Update()
    {
        if (disabled || !canBeTriggered || (triggered && !reversable))
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Trigger();
        }
    }

    void Trigger()
    {
        if (disabled || (!reversable && triggered))
        {
            return;
        }

        GameStateManager.Instance.SendSignal(gameObject, SignalToSend);

        triggered = !triggered;
        if (!reversable && activationText)
        {
            activationText.SetActive(false);
        }
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (triggered)
        {
            spriteRenderer.sprite = activeSprite;
        }
        else
        {
            spriteRenderer.sprite = startingSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (disabled)
        {
            if (activationText)
            {
                activationText.SetActive(false);
            }
            return;
        }
        if (canBeActivatedByObjects && other.CompareTag(workingObjectTag))
        {
            Trigger();
            return;
        }
        if (!canBeActivatedByObjects && other.CompareTag("Player"))
        {
            if (triggered && !reversable)
            {
                return;
            }
            if (activateOnPass)
            {
                Trigger();
                return;
            }
            canBeTriggered = true;
            if (activationText)
            {
                activationText.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (disabled)
        {
            if (activationText)
            {
                activationText.SetActive(false);
            }
            return;
        }
        if (canBeActivatedByObjects && other.CompareTag(workingObjectTag) && !(triggered && !reversable))
        {
            Trigger();
            return;
        }

        if (!canBeActivatedByObjects && other.CompareTag("Player"))
        {
            canBeTriggered = false;
            if (activationText)
            {
                activationText.SetActive(false);
            }
        }
    }

    private void OnSignalReceived(GameObject source, string signal)
    {
        if (signalToDisable == signal)
        {
            disabled = true;
        }
    }
}
