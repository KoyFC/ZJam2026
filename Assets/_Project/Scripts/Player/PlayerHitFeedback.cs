using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(HealthController))]
public class PlayerHitFeedback : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float m_FlashDuration = 0.3f;

    private static readonly int s_BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int s_ColorId = Shader.PropertyToID("_Color");

    private HealthController m_HealthController;
    private Outline m_Outline;
    private Material[] m_ModelMaterials;
    private int[] m_ColorPropertyIds;
    private Color[] m_OriginalColors;
    private Sequence m_FlashSequence;

    private void Awake()
    {
        m_HealthController = GetComponent<HealthController>();
        m_Outline = GetComponentInChildren<Outline>(true);

        if (m_Outline == null)
        {
            Debug.LogError($"{name}: PlayerHitFeedback requires an Outline on the ship.", this);
            return;
        }

        Renderer[] renderers = m_Outline.GetComponentsInChildren<Renderer>(true);
        var materials = new List<Material>();
        var colorPropertyIds = new List<int>();
        var originalColors = new List<Color>();

        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                int propertyId = GetColorPropertyId(material);
                if (propertyId == 0)
                    continue;

                materials.Add(material);
                colorPropertyIds.Add(propertyId);
                originalColors.Add(material.GetColor(propertyId));
            }
        }

        m_ModelMaterials = materials.ToArray();
        m_ColorPropertyIds = colorPropertyIds.ToArray();
        m_OriginalColors = originalColors.ToArray();

        if (m_ModelMaterials.Length == 0)
            Debug.LogWarning($"{name}: No color properties were found on the ship materials.", this);
    }

    private void OnEnable()
    {
        if (m_HealthController == null)
            m_HealthController = GetComponent<HealthController>();

        m_HealthController.OnDamaged += HandleDamaged;
    }

    private void OnDisable()
    {
        if (m_HealthController != null)
            m_HealthController.OnDamaged -= HandleDamaged;

        ResetMaterials();
    }

    private void HandleDamaged(float damage)
    {
        if (m_ModelMaterials == null || m_ModelMaterials.Length == 0)
            return;

        m_FlashSequence?.Kill();
        SetMaterialsColor(Color.white);
        m_FlashSequence = DOTween.Sequence()
            .AppendInterval(m_FlashDuration)
            .AppendCallback(RestoreMaterials)
            .SetLink(gameObject);
    }

    private void RestoreMaterials()
    {
        if (m_ModelMaterials == null)
            return;

        for (int i = 0; i < m_ModelMaterials.Length; i++)
        {
            if (m_ModelMaterials[i] != null)
                m_ModelMaterials[i].SetColor(m_ColorPropertyIds[i], m_OriginalColors[i]);
        }
    }

    private void SetMaterialsColor(Color color)
    {
        for (int i = 0; i < m_ModelMaterials.Length; i++)
        {
            if (m_ModelMaterials[i] != null)
                m_ModelMaterials[i].SetColor(m_ColorPropertyIds[i], color);
        }
    }

    private static int GetColorPropertyId(Material material)
    {
        if (material.HasProperty("_BaseColor"))
            return s_BaseColorId;
        if (material.HasProperty("_Color"))
            return s_ColorId;

        return 0;
    }

    private void ResetMaterials()
    {
        m_FlashSequence?.Kill();
        RestoreMaterials();
    }
}
