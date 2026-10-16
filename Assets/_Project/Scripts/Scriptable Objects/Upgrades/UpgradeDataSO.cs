using System;
using UnityEngine;
using System.Collections.Generic;

public enum StatModifierType
{
    none,
    additive,
    multiplicative
}

[CreateAssetMenu(fileName = "UpgradeData", menuName = "ScriptableObjects/UpgradeDataSO", order = 1)]

public class UpgradeDataSO : ScriptableObject
{
    [Header("Upgrade Type Data")]
    [field: SerializeField] public  UpgradeType UpgradeType {get; private set;} = UpgradeType.none;
    [field: SerializeField] public StatModifierType ModifierType {get; private set;} = StatModifierType.none;

    [Header("Upgrade Information")]
    [field: SerializeField] public Sprite Icon {get; private set;} = null;
    [field: SerializeField] public string Description {get; private set;} = "";
    [field: SerializeField] public int Cost {get; private set;} = 0;

    [Header("Upgrade Modifier Data")]
    [field: SerializeField] public float AdditiveValue {get; private set;} = 0f;

    [field: SerializeField] public float MultiplicativeValue { get; private set; } = 1f;


    [Header("Upgrade Consequents")]
    [field: SerializeField] public List<UpgradeDataSO> Consequents {get; private set;} = new List<UpgradeDataSO>();
    
}
