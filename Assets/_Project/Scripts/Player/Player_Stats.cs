using UnityEngine;
using System.Collections.Generic;

public enum UpgradeType
{
    none,
    qualitiy_of_life,
    damage,
    fuel,
    money,
    resources
}

/// <summary>
/// Singleton instance of the Player_Stats class.
/// The Player_Stats class is responsible for managing the player's statistics in the game.
/// </summary>
public class Player_Stats : Singleton<Player_Stats>
{
    #region Player Stats
    [Header("Player Stats")]
    [SerializeField] private float m_currentMoney = 0f;
    [SerializeField] private float m_currentCobalt = 0f;
    [SerializeField] private float m_maxFuel = 0f;
    #endregion

    #region Player Upgrades
    private List<UpgradeDataSO> m_purchasedUpgrades = new List<UpgradeDataSO>();
    #endregion

    #region Public References
    public float CurrentMoney => m_currentMoney;
    public float CurrentCobalt => m_currentCobalt;
    public float CurrentFuel => m_maxFuel;
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
            case UpgradeType.money:
                return m_currentMoney;
            case UpgradeType.fuel:
                return m_maxFuel;
            case UpgradeType.resources:
                return m_currentCobalt;
            default:
                Debug.LogWarning("Unhandled upgrade type: " + upgradeType);
                return 0f;
        }
    }

    public void SetStat(UpgradeType upgradeType, float newValue)
    {
        switch (upgradeType)
        {
            case UpgradeType.money:
                m_currentMoney = newValue;
                Debug.Log("Set money to: " + newValue);
                break;
            case UpgradeType.fuel:
                m_maxFuel = newValue;
                Debug.Log("Set fuel to: " + newValue);
                break;
            case UpgradeType.resources:
                m_currentCobalt = newValue;
                Debug.Log("Set cobalt to: " + newValue);
                break;
            default:
                Debug.LogWarning("Unhandled upgrade type: " + upgradeType);
                break;
        }
    }
    #endregion
}
