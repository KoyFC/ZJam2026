using UnityEngine;
using System.Collections.Generic;

public class Player_Stats : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the Player_Stats class.
    /// The Player_Stats class is responsible for managing the player's statistics in the game.
    /// </summary>
    public static Player_Stats Instance { get; private set; }

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
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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
        if(!m_purchasedUpgrades.Contains(upgradeData))
        {
            m_purchasedUpgrades.Add(upgradeData);
        }
    }

    public void SetMoney(float newAmount)
    {
        m_currentMoney = newAmount;
    }

    public void SetFuel(float newAmount)
    {
        m_maxFuel = newAmount;
    }

    public void SetCobalt(float newAmount)
    {
        m_currentCobalt = newAmount;
    }

    public float GetStat(UpgradeType upgradeType)
    {
        switch(upgradeType)
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
        switch(upgradeType)
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
