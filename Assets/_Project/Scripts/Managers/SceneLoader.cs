using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : PersistentLazySingleton<SceneLoader>
{
    private const float LOADED_PROGRESS = 0.9f;
    private const float MAX_WIPE_STEP = 1f / 30f;

    [Header("Timing")]
    [SerializeField, Min(0.01f)] private float m_WipeDuration = 0.6f;
    [SerializeField, Min(0.01f)] private float m_BarSpeed = 1.5f;
    [SerializeField, Min(0.01f)] private float m_MoonFadeDuration = 0.3f;

    [Header("References")]
    [SerializeField] private Image m_Blocker;
    [SerializeField] private RectTransform m_Wipe;
    [SerializeField] private RectTransform m_Edge;
    [SerializeField] private GameObject m_Loading;
    [SerializeField] private RectTransform m_BarFill;
    [SerializeField] private GameObject m_MoonStage;
    [SerializeField] private RawImage m_MoonView;

    private bool m_IsBusy;

    public static bool IsBusy => InstanceExists && Instance.m_IsBusy;

    private float EdgeWidth => m_Edge.rect.width;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Create()
    {
        Instantiate(Resources.Load<SceneLoader>(nameof(SceneLoader)));
    }

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this)
            return;

        SetReveal(1f);
        SetLoadingVisible(false);
        m_Blocker.enabled = false;
    }

    public static void Load(string sceneName)
    {
        if (IsBusy)
            return;

        Instance.StartCoroutine(Instance.Transition(sceneName));
    }

    private IEnumerator Transition(string sceneName)
    {
        m_IsBusy = true;
        m_Blocker.enabled = true;

        ThreadPriority previousPriority = Application.backgroundLoadingPriority;
        Application.backgroundLoadingPriority = ThreadPriority.Low;

        yield return Wipe(SetCover);

        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneName);
        loading.allowSceneActivation = false;
        yield return ShowProgress(loading);
        yield return FadeMoon(0f);

        loading.allowSceneActivation = true;
        yield return loading;
        SetLoadingVisible(false);
        Application.backgroundLoadingPriority = previousPriority;

        yield return null;
        yield return Wipe(SetReveal);

        m_Blocker.enabled = false;
        m_IsBusy = false;
    }

    private IEnumerator Wipe(Action<float> apply)
    {
        float time = 0f;
        while (time < m_WipeDuration)
        {
            apply(Mathf.SmoothStep(0f, 1f, time / m_WipeDuration));
            time += Mathf.Min(Time.unscaledDeltaTime, MAX_WIPE_STEP);
            yield return null;
        }

        apply(1f);
    }

    private IEnumerator ShowProgress(AsyncOperation loading)
    {
        float shown = 0f;
        SetBar(shown);
        SetMoonAlpha(0f);
        SetLoadingVisible(true);

        while (shown < 1f || m_MoonView.color.a < 1f)
        {
            float target = Mathf.Clamp01(loading.progress / LOADED_PROGRESS);
            shown = Mathf.MoveTowards(shown, target, m_BarSpeed * Time.unscaledDeltaTime);
            SetBar(shown);
            StepMoonFade(1f);
            yield return null;
        }
    }

    private IEnumerator FadeMoon(float alpha)
    {
        while (!Mathf.Approximately(m_MoonView.color.a, alpha))
        {
            StepMoonFade(alpha);
            yield return null;
        }
    }

    private void StepMoonFade(float alpha)
    {
        float step = Time.unscaledDeltaTime / m_MoonFadeDuration;
        SetMoonAlpha(Mathf.MoveTowards(m_MoonView.color.a, alpha, step));
    }

    private void SetMoonAlpha(float alpha)
    {
        Color color = m_MoonView.color;
        color.a = alpha;
        m_MoonView.color = color;
    }

    // The soft edges sit outside the wipe, so both ends start and finish off screen
    private void SetCover(float progress)
    {
        m_Wipe.anchorMin = Vector2.zero;
        m_Wipe.anchorMax = new Vector2(progress, 1f);
        m_Wipe.offsetMin = new Vector2(-EdgeWidth, 0f);
        m_Wipe.offsetMax = new Vector2(-EdgeWidth * (1f - progress), 0f);
    }

    private void SetReveal(float progress)
    {
        m_Wipe.anchorMin = new Vector2(progress, 0f);
        m_Wipe.anchorMax = Vector2.one;
        m_Wipe.offsetMin = new Vector2(EdgeWidth * progress, 0f);
        m_Wipe.offsetMax = new Vector2(EdgeWidth, 0f);
    }

    private void SetBar(float progress)
    {
        m_BarFill.anchorMax = new Vector2(progress, 1f);
    }

    private void SetLoadingVisible(bool visible)
    {
        m_Loading.SetActive(visible);
        m_MoonStage.SetActive(visible);
    }
}
