using System;
using UnityEngine;

public class Stats_Modifier : MonoBehaviour
{
    public static Stats_Modifier Instance { get; private set; }
    private UpgradeDataSO m_upgradeReference = null;
    private Enum m_modifierType => m_upgradeReference.ModifierType;
    private Player_Stats m_playerStats => Player_Stats.Instance;


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
        if(upgradeData == null || m_playerStats == null)
        {
            Debug.LogWarning("UpgradeData or Player_Stats is null.");
            return;
        }

        m_upgradeReference = upgradeData;
        if(m_upgradeReference == null)
        {
            Debug.LogWarning("Upgrade reference is null.");
            return;
        }

        float currentStatValue = GetStatToModify(upgradeData.UpgradeType, m_playerStats);
        float modifierValue = GetModifierValue(upgradeData);
        

        float newStatValue = ApplyModifier(currentStatValue, modifierValue, upgradeData.ModifierType);

        m_playerStats.SetStat(upgradeData.UpgradeType, newStatValue);

    }

    #region Upgrade Application Methods
    /*private float ApplyAdditiveUpgrade(float modifierValue, float statToModify)
    {
        statToModify += modifierValue;
        return statToModify;
    }

    private float ApplyMultiplicativeUpgrade(float modifierValue, float statToModify)
    {
        statToModify *= modifierValue;
        return statToModify;
    }*/

    private float ApplyModifier(float currentValue, float modifierValue, StatModifierType modifierType)
    {
        switch(modifierType)
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

        return upgradeData.UpgradeType;
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
