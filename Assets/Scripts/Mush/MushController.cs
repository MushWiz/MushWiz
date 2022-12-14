using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MushController : MonoBehaviour
{
    [HideInInspector] public GameController controller;
    public float lifePoints = 5f;
    public int maxLifePoints = 10;

    public float stamina = 10f;
    public int maxStamina = 10;

    public float mana = 10f;
    public int maxMana = 10;

    private float lastDamageTime = 0f;

    private MushAttack mushAttack;
    private MushMovementController mushMovementController;

    public bool isActive = true;
    public bool isDead = false;
    public bool isInvincible = false;

    public Image lifeBar;
    public Image expBar;
    public Image staminaBar;
    public Image manaBar;

    public List<MushAbilities> abilities = new List<MushAbilities>();
    public List<MushAbilities> activableAbilities = new List<MushAbilities>();
    private List<MushAbilities> unlockedOrUnreachableAbilities = new List<MushAbilities>();

    public float experience = 0f;
    public float experienceToNextLevel = 10f;
    public float experienceToNextLevelMultiplier = 1.5f;
    public int level = 1;
    public int availablePoints = 0;

    public List<Stats> stats = new List<Stats>(){
        new Stats(10, StatType.Health),
        new Stats(5, StatType.Intelligence),
        new Stats(5, StatType.Speed),
        new Stats(0, StatType.Defense),
        new Stats(0, StatType.Evasion),
        new Stats(0, StatType.BlockChance),
    };

    public MushWeaponHolder weaponHolder;

    private void Start()
    {
        mushAttack = GetComponent<MushAttack>();
        mushMovementController = GetComponent<MushMovementController>();

        weaponHolder = GetComponentInChildren<MushWeaponHolder>();

        //Reset abilities cooldowns
        foreach (MushAbilities ability in activableAbilities)
        {
            ability.cooldownTimer = 0f;
        }

        Heal(maxLifePoints * GetStatValueByType(StatType.Health));
    }

    private void Awake()
    {
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    //On collision stay, if the object is an enemy, take damage once every 1.5 seconds
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (Time.time - lastDamageTime > 1.5f && !isInvincible)
            {
                float damageAmount = TakeDamage(Mathf.Max(other.gameObject.GetComponent<MonsterController>().starterWeapon.meleeDamage, 1));
                lastDamageTime = Time.time;
                if (!isDead)
                {
                    StartCoroutine(TookDamage(damageAmount));
                }
            }
        }
    }

    //on collisioin enter, if the object is an enemy projectile, take damage
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("EnemyProjectile"))
        {
            if (Time.time - lastDamageTime > 1.5f && !isInvincible)
            {
                float damageAmount = TakeDamage(other.gameObject.GetComponent<ProjectileController>().projectileDamage);
                lastDamageTime = Time.time;
                if (!isDead)
                {
                    StartCoroutine(TookDamage(damageAmount));
                }
            }

            Destroy(other.gameObject);
        }
    }

    public int GetStatValueByType(StatType statType)
    {
        foreach (Stats stat in stats)
        {
            if (stat.GetStatType() == statType)
            {
                return stat.GetValue();
            }
        }
        return 0;
    }

    public float TakeDamage(float damage)
    {

        float armor = GetStatValueByType(StatType.Defense) * 0.5f + GetStatValueByType(StatType.Resistance) * 0.1f;

        lifePoints = lifePoints - Mathf.Max(damage - armor, 0);
        if (lifePoints <= 0)
        {
            isDead = true;
            isActive = false;
        }
        lifeBar.fillAmount = lifePoints / (maxLifePoints * GetStatValueByType(StatType.Health));
        return damage;
    }

    public void Heal(float heal)
    {
        lifePoints = Mathf.Min((maxLifePoints * GetStatValueByType(StatType.Health)), lifePoints + heal);
        lifeBar.fillAmount = lifePoints / (maxLifePoints * GetStatValueByType(StatType.Health));

        stamina = maxStamina * GetStatValueByType(StatType.Stamina);

        mana = maxMana * GetStatValueByType(StatType.Mana);
        manaBar.fillAmount = mana / (maxMana * GetStatValueByType(StatType.Mana));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IncreaseExperience(15f);
        }

        if (stamina < maxStamina * GetStatValueByType(StatType.Stamina))
        {
            stamina += Time.deltaTime * 5;
            stamina = Mathf.Min(stamina, maxStamina * GetStatValueByType(StatType.Stamina));
        }

        if (mana < maxMana * GetStatValueByType(StatType.Mana))
        {
            mana += Time.deltaTime * 5;
            mana = Mathf.Min(mana, maxMana * GetStatValueByType(StatType.Mana));
            manaBar.fillAmount = mana / (maxMana * GetStatValueByType(StatType.Mana));
        }

        if (!isActive || isDead)
        {
            return;
        }
        mushAttack.AttackControl(this);
        mushMovementController.MovementControl(this);
    }

    IEnumerator TookDamage(float damageAmount)
    {
        isInvincible = true;
        StartCoroutine(Flash());
        yield return new WaitForSeconds(1.5f);
        isInvincible = false;
    }

    IEnumerator Flash()
    {
        while (isInvincible)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.1f);
            GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(0.2f);
        }
    }

    //Add experience to the player
    public void IncreaseExperience(float experienceGained)
    {
        experience += experienceGained;
        if (experience >= experienceToNextLevel)
        {
            experience -= experienceToNextLevel;
            level++;
            experienceToNextLevel = experienceToNextLevel * experienceToNextLevelMultiplier;
            LevelUp();
            IncreaseExperience(0);
        }
        expBar.fillAmount = experience / experienceToNextLevel;
    }

    //Add an ability to the player
    public void AddAbility(MushAbilities ability)
    {
        if (abilities.Contains(ability))
        {
            return;
        }
        abilities.Add(ability);
        controller.uIHandler.actionBarManager.AddAbility(ability);
    }

    //Remove an ability from the player
    public void RemoveAbility(MushAbilities ability)
    {
        abilities.Remove(ability);
        controller.uIHandler.actionBarManager.RemoveAbility(ability);
    }

    public void RemoveAllAbilities()
    {
        foreach (MushAbilities ability in abilities)
        {
            controller.uIHandler.actionBarManager.RemoveAbility(ability);
        }
        abilities.Clear();
    }

    //Get the player's current level
    public int GetLevel()
    {
        return level;
    }

    //Get the player's current experience
    public float GetExperience()
    {
        return experience;
    }

    //Get the player's current experience to the next level
    public float GetExperienceToNextLevel()
    {
        return experienceToNextLevel;
    }

    //Check if the player has the given ability
    public bool HasAbility(MushAbilities ability)
    {
        return abilities.Contains(ability);
    }

    //Check if the player level can unlock the given ability
    public bool CanUnlockAbility(MushAbilities ability)
    {
        return level >= ability.unlockLevel;
    }

    public void CheckActivableAbilities()
    {
        foreach (MushAbilities ability in activableAbilities)
        {
            //Add the ability to the player's abilities if the player can unlock it
            if (CanUnlockAbility(ability))
            {
                AddAbility(ability);
            }
        }
    }

    public void LevelUp()
    {
        Heal(maxLifePoints * GetStatValueByType(StatType.Health));
        GameStateManager.Instance.SendEvent(GameEvent.LevelUp);
        availablePoints += 3;
    }

    private void OnGameStateChanged(GameState newState)
    {
        if (newState != GameState.Paused)
        {
            enabled = true;
            return;
        }

        enabled = false;

    }

    public void CreateLevelUpButtons()
    {
        UITypeController levelUpUI = controller.uIHandler.GetUITypeControllerByType(UIType.LevelUp);
        Transform uiLevelUp = levelUpUI.transform;

        //Get the child of the level up
        Transform levelUpChild = uiLevelUp.GetChild(0);
        //remove all the children of the level up child
        foreach (Transform child in levelUpChild)
        {
            Destroy(child.gameObject);
        }

        Transform statUpChild = uiLevelUp.GetChild(1);
        //remove all the children of the level up child
        foreach (Transform child in statUpChild)
        {
            Destroy(child.gameObject);
        }


        List<MushAbilities> abilitiesCopy = new List<MushAbilities>();

        foreach (MushAbilities ability in activableAbilities)
        {
            //Continue if the ability is already unlocked
            if (HasAbility(ability) || !CanUnlockAbility(ability))
            {
                continue;
            }
            abilitiesCopy.Add(ability);
        }

        List<MushAbilities> randomAbilities = new List<MushAbilities>();
        while (randomAbilities.Count < 4 && abilitiesCopy.Count > 0)
        {
            int randomIndex = Random.Range(0, abilitiesCopy.Count);
            if (!randomAbilities.Contains(abilitiesCopy[randomIndex]))
            {
                randomAbilities.Add(abilitiesCopy[randomIndex]);
                abilitiesCopy.RemoveAt(randomIndex);
            }
        }

        if (randomAbilities.Count > 0)
        {
            foreach (MushAbilities ability in randomAbilities)
            {
                GameObject button = Instantiate(ability.abilityButtonPrefab, uiLevelUp);
                button.transform.SetParent(uiLevelUp.GetChild(0));
                Button buttonComponent = button.GetComponent<Button>();
                buttonComponent.onClick.AddListener(() =>
                {
                    AddAbility(ability);
                    AddAbilityToUI(button);
                    controller.paused = false;
                });
            }
        }

        foreach (Stats stat in stats)
        {
            GameObject buttonObject = Instantiate(stat.statButton, uiLevelUp) as GameObject;
            buttonObject.transform.SetParent(uiLevelUp.GetChild(1));
            buttonObject.name = stat.GetStatType().ToString();

            GameObject buttonName = buttonObject.transform.GetChild(0).gameObject;
            if (buttonName)
            {
                buttonName.GetComponent<TextMeshProUGUI>().text = stat.GetStatType().ToString() + " up 1";
            }
            GameObject buttonValue = buttonObject.transform.GetChild(1).gameObject;
            if (buttonValue)
            {
                buttonValue.GetComponent<TextMeshProUGUI>().text = stat.GetValue() + " points";
            }

            Button buttonComponent = buttonObject.GetComponent<Button>();
            buttonComponent.onClick.AddListener(() =>
            {
                stat.IncreaseValue();
                controller.paused = false;
            });
        }

    }

    public bool IncreaseStatValue(StatType statType, int increaseAmount = 1)
    {
        foreach (Stats stat in stats)
        {
            if (stat.GetStatType() == statType)
            {
                stat.IncreaseValue(increaseAmount);
                controller.UpdateInfoPanel();
                return true;
            }
        }
        return AddNewStat(statType, increaseAmount);
    }

    public bool AddNewStat(StatType statType, int initialValue)
    {
        stats.Add(new Stats(initialValue, statType));
        return true;
    }

    public void Reset()
    {
        isActive = false;
        isDead = false;
        expBar.fillAmount = 0f;
        experience = 0;
        level = 1;
        experienceToNextLevel = 10;
        foreach (Stats stat in stats)
        {
            stat.ResetValueToInitial();
        }
        Heal(maxLifePoints * GetStatValueByType(StatType.Health));

        RemoveAllAbilities();

        transform.position = new Vector3(0, 0, 0);
    }

    public void AddAbilityToUI(GameObject button)
    {
        button.GetComponent<Button>().interactable = false;
        button.transform.SetParent(controller.uIHandler.infoPanelManager.abilityContainer.transform);
        button.transform.localScale = new Vector3(1, 1, 1);
    }

    public MushAbilities GetAbilityByName(string name)
    {
        foreach (MushAbilities ability in abilities)
        {
            if (ability.name == name)
            {
                return ability;
            }
        }
        return null;
    }

    public MushAbilities GetAvailableAbilityByName(string name)
    {
        foreach (MushAbilities ability in activableAbilities)
        {
            if (ability.name == name)
            {
                return ability;
            }
        }
        return null;
    }

    public void LoadPlayer(PlayerData playerData)
    {
        Reset();

        foreach (string abilityName in playerData.abilities)
        {
            MushAbilities ability = GetAvailableAbilityByName(abilityName);
            if (ability != null)
            {
                AddAbility(ability);
            }
        }

        foreach (Stats stat in stats)
        {
            stat.SetValue(playerData.stats[stats.IndexOf(stat)]);
        }
        experience = playerData.experience;
        level = playerData.level;
        experienceToNextLevel = playerData.experienceToNextLevel;
        isActive = true;
        isDead = false;
        controller.paused = false;

        transform.position = new Vector3(playerData.position[0], playerData.position[1], playerData.position[2]);
    }

}
