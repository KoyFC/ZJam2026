using System;
using UnityEngine;

public class Stats_Modifier : MonoBehaviour
{
    private UpgradeDataSO m_upgradeReferemce = null;
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

        //Then, get the stat to modify based on the upgrade type and the player stats
        float statToModify = GetStatToModify(upgradeType, playerStats);

        switch(m_modifierType)
        {
            case StatModifierType.additive:
                ApplyAdditiveUpgrade(modifierValue, statToModify);
                break;
            case StatModifierType.multiplicative:
                ApplyMultiplicativeUpgrade(modifierValue, statToModify);
                break;
            default:
                Debug.LogWarning("Unhandled modifier type: " + m_modifierType);
                break;
        }

    }

    #region Upgrade Application Methods
    private void ApplyAdditiveUpgrade(float modifierValue, float statToModify)
    {
        statToModify += modifierValue;
    }

    private void ApplyMultiplicativeUpgrade(float modifierValue, float statToModify)
    {
        statToModify *= modifierValue;
    }
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
                return AdditiveValue;
            case StatModifierType.multiplicative:
                float MultiplicativeValue = upgradeData.MultiplicativeValue;
                return MultiplicativeValue;
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

    private float GetStatToModify(UpgradeType upgradeType, Player_Stats playerStats)
    {
        switch(upgradeType)
        {
            case UpgradeType.money:
                return playerStats.CurrentMoney;
            case UpgradeType.fuel:
                return playerStats.CurrentFuel;
            case UpgradeType.resources:
                return playerStats.CurrentCobalt;
            default:
                Debug.LogWarning("Unhandled upgrade type: " + upgradeType);
                return 0f;
        }
    }
    #endregion
}
