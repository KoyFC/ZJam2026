using UnityEngine;
using System;
using System.Collections.Generic;

public class Upgrade_Manager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the Upgrade_Manager class.
    /// The Upgrade_Manager class is responsible for managing upgrades in the game.
    /// </summary>
    
    public static Upgrade_Manager Instance { get; private set; }

    private List<UpgradeDataSO> m_upgradeDataList = new List<UpgradeDataSO>();

    #region Events
    public static event Action<UpgradeDataSO> OnUpgradePurchased;
    #endregion

    #region Custom Methods
    public void TryPurchaseUpgrade(Upgrade_Info upgradeInfo)
    {
        if(upgradeInfo == null || upgradeInfo.UpgradeData == null)
        {
            Debug.LogWarning("Upgrade_Info or UpgradeData is null.");
            return;
        }

        UpgradeDataSO upgradeData = upgradeInfo.UpgradeData;
        
        if(Player_Stats.Instance.CurrentMoney < upgradeData.Cost)
        {
            Debug.Log("Not enough money to purchase upgrade: " + upgradeData.name);
            return;
        }
        Debug.Log("Purchased upgrade: " + upgradeData.name);
        OnUpgradePurchased?.Invoke(upgradeData);
    }

    private void TryUnlockUpgrade(GameObject upgradeButton)
    {
        UpgradeDataSO upgradeData = upgradeButton.GetComponent<Upgrade_Info>().UpgradeData;
    }
    #endregion
}
