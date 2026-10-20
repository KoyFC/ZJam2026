using UnityEngine;

public class CreditsController : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        SceneLoader.Load("MainMenuScene");
    }
}
