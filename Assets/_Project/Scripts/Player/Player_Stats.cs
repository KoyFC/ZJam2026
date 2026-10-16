using UnityEngine;
using System.Collections.Generic;

public enum UpgradeType
{
    none,
    resources,
    damage,
    health, 
    mobility
}

/// <summary>
/// Singleton instance of the Player_Stats class.
/// The Player_Stats class is responsible for managing the player's statistics in the game.
/// </summary>
public class Player_Stats : Singleton<Player_Stats>
{
    #region Player Stats
    [Header("Player Stats")]
    [SerializeField] private float m_currentResources = 0f;
    [SerializeField] private float m_currentDamage = 0f;
    [SerializeField] private float m_currentHealth = 0f;
    [SerializeField] private float m_currentDashes = 0f;
    #endregion

    #region Player Upgrades
    private List<UpgradeDataSO> m_purchasedUpgrades = new List<UpgradeDataSO>();
    #endregion

    #region Public References
    public float CurrentResources => m_currentResources;
    public float CurrentDamage => m_currentDamage;
    public float CurrentHealth => m_currentHealth;
    public float CurrentDashes => m_currentDashes;
    public IReadOnlyList<UpgradeDataSO> PurchasedUpgrades => m_purchasedUpgrades.AsReadOnly();
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        Upgrade_Manager.OnUpgradePurchased += AddUpgrade;
    }

    private void OnDisable()
    {
        Upgrade_Manager.OnUpgradePurchased -= AddUpgrade;
    }
    #endregion

    #region Custom Methods
    private void AddUpgrade(UpgradeDataSO upgradeData)
    {
        if (!m_purchasedUpgrades.Contains(upgradeData))
        {
            m_purchasedUpgrades.Add(upgradeData);
        }
    }

    public float GetStat(UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.resources:
                return m_currentResources;
            case UpgradeType.damage:
                return m_currentHealth;
            case UpgradeType.health:
                return m_currentDamage;
            case UpgradeType.mobility:
                return m_currentDashes;
            default:
                Debug.LogWarning("Unhandled upgrade type: " + upgradeType);
                return 0f;
        }
    }

    public void SetStat(UpgradeType upgradeType, float newValue)
    {
        switch (upgradeType)
        {
            case UpgradeType.resources:
                m_currentResources = newValue;
                Debug.Log("Set resources to: " + newValue);
                break;
            case UpgradeType.damage:
                m_currentDamage = newValue;
                Debug.Log("Set damage to: " + newValue);
                break;
            case UpgradeType.health:
                m_currentHealth = newValue;
                Debug.Log("Set health to: " + newValue);
                break;
            case UpgradeType.mobility:
                m_currentDashes = newValue;
                Debug.Log("Set dashes to: " + newValue);
                break;
            default:
                Debug.LogWarning("Unhandled upgrade type: " + upgradeType);
                break;
        }
    }
    #endregion
}
