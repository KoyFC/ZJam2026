using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneLoader.Load("NearSideScene");
    }

    public void StartTutorial()
    {
        SceneLoader.Load("TutorialScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}