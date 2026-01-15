using UnityEngine;
using UnityEngine.SceneManagement;

public class N_222NextScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ButtonClick()
    {
        SceneManager.LoadScene("MiniGame1");
    }
}
