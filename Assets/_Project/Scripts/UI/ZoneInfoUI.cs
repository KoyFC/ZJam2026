using TMPro;
using UnityEngine;

public class ZoneInfoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text m_Title;
    [SerializeField] private TMP_Text m_Description;

    private void Start()
    {
        if (!ZoneManager.InstanceExists)
            return;

        ZoneManager.Instance.OnZoneChanged += Refresh;
        Refresh();
    }

    private void OnDestroy()
    {
        if (ZoneManager.InstanceExists)
            ZoneManager.Instance.OnZoneChanged -= Refresh;
    }

    private void Refresh()
    {
        ZoneDataSO zone = ZoneManager.Instance.CurrentZone;
        m_Title.text = zone != null ? zone.Title : string.Empty;
        m_Description.text = zone != null ? zone.Description : string.Empty;
    }
}
