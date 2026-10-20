using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LevelButton : MonoBehaviour
{
    [SerializeField, Min(1)] private int m_Level = 1;

    private Button m_Button;
    private Color m_SelectedColor;
    private Color m_UnselectedColor;

    private void Awake()
    {
        m_Button = GetComponent<Button>();
        m_SelectedColor = m_Button.colors.normalColor;
        m_UnselectedColor = m_Button.colors.disabledColor;
        m_Button.onClick.AddListener(Select);

        gameObject.SetActive(LevelProgress.IsUnlocked(m_Level));
    }

    private void OnEnable()
    {
        LevelProgress.OnSelected += Refresh;
        Refresh(LevelProgress.Selected);
    }

    private void OnDisable()
    {
        LevelProgress.OnSelected -= Refresh;
    }

    private void Select()
    {
        LevelProgress.Select(m_Level);
    }

    private void Refresh(int selectedLevel)
    {
        ColorBlock colors = m_Button.colors;
        colors.normalColor = selectedLevel == m_Level ? m_SelectedColor : m_UnselectedColor;
        // Keeps the EventSystem focus from overriding the selection tint
        colors.selectedColor = colors.normalColor;
        m_Button.colors = colors;
    }
}
