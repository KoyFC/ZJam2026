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
    #endregion

    #region Player Upgrades
    private List<UpgradeDataSO> m_purchasedUpgrades = new List<UpgradeDataSO>();
    #endregion

    #region Public References
    public float CurrentMoney => m_currentMoney;
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
    #endregion
}
