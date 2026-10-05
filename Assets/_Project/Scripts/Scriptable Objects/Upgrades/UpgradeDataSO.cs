using System;
using UnityEngine;
using System.Collections.Generic;

public enum UpgradeType
{
    none,
    qualitiy_of_life,
    damage,
    fuel,
    resources
}

[CreateAssetMenu(fileName = "UpgradeData", menuName = "ScriptableObjects/UpgradeDataSO", order = 1)]

public class UpgradeDataSO : ScriptableObject
{

    [field: SerializeField] public  UpgradeType UgradeType {get; private set;} = UpgradeType.none;
    [field: SerializeField] public Sprite Icon {get; private set;} = null;
    [field: SerializeField] public string Description {get; private set;} = "";
    [field: SerializeField] public int Cost {get; private set;} = 0;
    [field: SerializeField] public List<UpgradeDataSO> Prerequisites {get; private set;} = new List<UpgradeDataSO>();
    
}
