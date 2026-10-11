using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Singleton instance of the Upgrade_Manager class.
/// The Upgrade_Manager class is responsible for managing upgrades in the game.
/// </summary>
public class Upgrade_Manager : Singleton<Upgrade_Manager>
{
    [SerializeField] private List<GameObject> m_upgradeButtons = new List<GameObject>();

    #region Events
    public static event Action<UpgradeDataSO> OnUpgradePurchased;
    #endregion

    #region Custom Methods
    public void TryPurchaseUpgrade(Upgrade_Info upgradeInfo)
    {
        if (upgradeInfo == null || upgradeInfo.UpgradeData == null)
        {
            Debug.LogWarning("Upgrade_Info or UpgradeData is null.");
            return;
        }

        UpgradeDataSO upgradeData = upgradeInfo.UpgradeData;

        if (Player_Stats.Instance.CurrentMoney < upgradeData.Cost)
        {
            Debug.Log("Not enough money to purchase upgrade: " + upgradeData.name);
            return;
        }
        Debug.Log("Purchased upgrade: " + upgradeData.name);
        OnUpgradePurchased?.Invoke(upgradeData);

        TryUnlockUpgrade(upgradeData);
    }

    private void TryUnlockUpgrade(UpgradeDataSO upgradeData)
    {
        if (upgradeData == null)
        {
            Debug.LogWarning("UpgradeData is null.");
            return;
        }

        if (upgradeData.Consequents.Count > 0)
        {
            Debug.Log("Unlocking consequent upgrades for: " + upgradeData.name);
            foreach (UpgradeDataSO consequent in upgradeData.Consequents)
            {
                foreach (GameObject button in m_upgradeButtons)
                {
                    if (button.GetComponent<Upgrade_Info>().UpgradeData == consequent)
                    {
                        button.SetActive(true);
                    }
                }
                Debug.Log("Unlocking consequent upgrade: " + consequent.name);
            }
        }
    }
    #endregion
}
