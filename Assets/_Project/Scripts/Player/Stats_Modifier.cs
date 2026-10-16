using System;
using UnityEngine;

public class Stats_Modifier : Singleton<Stats_Modifier>
{
    private UpgradeDataSO m_upgradeReference = null;
    private Enum m_modifierType => m_upgradeReference.ModifierType;


    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        Upgrade_Manager.OnUpgradePurchased += ApplyUpgrade;
    }

    private void OnDisable()
    {
        Upgrade_Manager.OnUpgradePurchased -= ApplyUpgrade;
    }
    #endregion
    public void ApplyUpgrade(UpgradeDataSO upgradeData)
    {
        //First check if the upgradeData is null
        if (upgradeData == null || !Player_Stats.InstanceExists)
        {
            Debug.LogWarning("UpgradeData or Player_Stats is null.");
            return;
        }

        m_upgradeReference = upgradeData;
        if (m_upgradeReference == null)
        {
            Debug.LogWarning("Upgrade reference is null.");
            return;
        }

        float currentStatValue = GetStatToModify(upgradeData.UpgradeType);
        float modifierValue = GetModifierValue(upgradeData);


        float newStatValue = ApplyModifier(currentStatValue, modifierValue, upgradeData.ModifierType);

        Player_Stats.Instance.SetStat(upgradeData.UpgradeType, newStatValue);
    }

    #region Upgrade Application Methods
    private float ApplyModifier(float currentValue, float modifierValue, StatModifierType modifierType)
    {
        switch (modifierType)
        {
            case StatModifierType.additive:
                return currentValue + modifierValue;
            case StatModifierType.multiplicative:
                return currentValue * modifierValue;
            default:
                Debug.LogWarning("Unhandled modifier type: " + modifierType);
                return currentValue;
        }


    }
    #endregion

    #region Helper Methods
    private float GetModifierValue(UpgradeDataSO upgradeData)
    {
        if (upgradeData == null)
        {
            Debug.LogWarning("UpgradeData is null.");
            return 0f;
        }

        switch (m_modifierType)
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
        if (upgradeData == null)
        {
            Debug.LogWarning("UpgradeData is null.");
            return UpgradeType.none;
        }

        return upgradeData.UpgradeType;
    }

    private float GetStatToModify(UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.resources:
                return Player_Stats.Instance.CurrentResources;
            case UpgradeType.damage:
                return Player_Stats.Instance.CurrentDamage;
            case UpgradeType.health:
                return Player_Stats.Instance.CurrentHealth;
            case UpgradeType.mobility:
                return Player_Stats.Instance.CurrentDashes;
            default:
                Debug.LogWarning("Unhandled upgrade type: " + upgradeType);
                return 0f;
        }
    }
    #endregion
}
