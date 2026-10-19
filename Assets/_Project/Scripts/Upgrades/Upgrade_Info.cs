using UnityEngine;

public class Upgrade_Info : MonoBehaviour
{
    [Header("Upgrade Data")]
    [SerializeField] private UpgradeDataSO m_upgradeData = null;

    public UpgradeDataSO UpgradeData => m_upgradeData;

    // [Header("UI References")]
    // [SerializeField] private TMPro.TMP_Text m_upgradeNameText = null;
    //[SerializeField] private TMPro.TMP_Text m_upgradeDescriptionText = null;

    private void OnEnable()
    {
        /*
        if(m_upgradeData != null)
        {
            m_upgradeNameText.text = m_upgradeData.name;
            //m_upgradeDescriptionText.text = m_upgradeData.Description;
        }
        */
    }

}
