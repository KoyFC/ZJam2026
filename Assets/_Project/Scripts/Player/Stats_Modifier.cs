using System;
using UnityEngine;

public class Stats_Modifier : MonoBehaviour
{
    private UpgradeDataSO m_upgradeReferemce = null;
    private Player_Stats m_playerStats => Player_Stats.Instance;

    private Enum m_upgradeType => m_upgradeReferemce.UgradeType;
    private Enum m_modifierType => m_upgradeReferemce.ModifierType;

    private void ApplyUpgrade(UpgradeDataSO upgradeData, Player_Stats playerStats)
    {
        //First check if the upgradeData is null
        if(upgradeData == null)
        {
            Debug.LogWarning("UpgradeData is null.");
            return;
        }

        //Second we get the modifier value based on the upgradeData and the modifier type
        float modifierValue = GetModifierValue(upgradeData);

        //Then check the value to modify based on the upgrade type
        UpgradeType upgradeType = GetUpgradeType(upgradeData);

        //Then, based on the upgrade type, we apply the modifier to the appropriate player stat

    }

    #region Upgrade Application Methods
    #endregion

    #region Helper Methods
    private float GetModifierValue(UpgradeDataSO upgradeData)
    {
        if(upgradeData == null)
        {
            Debug.LogWarning("UpgradeData is null.");
            return 0f;
        }

        switch(m_modifierType)
        {
            case StatModifierType.additive:
                float AdditiveValue = upgradeData.AdditiveValue;
                return upgradeData.AdditiveValue;
            case StatModifierType.multiplicative:
                float MultiplicativeValue = upgradeData.MultiplicativeValue;
                return upgradeData.MultiplicativeValue;
            default:
                Debug.LogWarning("Unhandled modifier type: " + m_modifierType);
                return 0f;
        }
    }

    private UpgradeType GetUpgradeType(UpgradeDataSO upgradeData)
    {
        if(upgradeData == null)
        {
            Debug.LogWarning("UpgradeData is null.");
            return UpgradeType.none;
        }

        return upgradeData.UgradeType;
    }
    #endregion
}
