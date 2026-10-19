using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    public void EnterCombat()
    {
        SceneManager.LoadScene("Enemy Testing");
    }
    public void EnterNearSide()
    {
        SceneManager.LoadScene("NearSideScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
