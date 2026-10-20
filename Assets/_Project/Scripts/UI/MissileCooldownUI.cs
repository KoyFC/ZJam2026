using UnityEngine;
using UnityEngine.UI;

public class MissileCooldownUI : MonoBehaviour
{
    [SerializeField] private RectTransform m_Content;
    [SerializeField] private Image m_Fill;
    [SerializeField] private Vector2 m_CursorOffset = Vector2.zero;

    private void Update()
    {
        if (!PlayerFiringController.InstanceExists || !UnlockedMechanics.IsUnlocked(PlayerMechanic.Missiles))
        {
            if (m_Content.gameObject.activeSelf)
                m_Content.gameObject.SetActive(false);

            return;
        }

        bool charging = !PlayerFiringController.Instance.IsMissileReady;

        if (m_Content.gameObject.activeSelf != charging)
            m_Content.gameObject.SetActive(charging);

        if (!charging)
            return;

        m_Fill.fillAmount = PlayerFiringController.Instance.MissileCooldownProgress;

        if (PlayerCrosshair.InstanceExists)
            m_Content.position = PlayerCrosshair.Instance.CursorScreenPosition + m_CursorOffset;
    }
}
