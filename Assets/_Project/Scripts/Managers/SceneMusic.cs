using UnityEngine;

// Tells the SoundManager which music this scene uses; leave the clip empty for silence
public class SceneMusic : MonoBehaviour
{
    [SerializeField] private AudioClip m_music;

    private void Start()
    {
        SoundManager.Instance.PlayMusic(m_music);
    }
}
