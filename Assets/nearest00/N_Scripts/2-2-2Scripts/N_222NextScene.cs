using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class N_222NextScene : MonoBehaviour
{
    private bool isTransitioning = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ButtonClick()
    {
        if (isTransitioning) return;
        if (SettingPanel.Instance != null && SettingPanel.Instance.IsAnyPanelOpen())
        {
            return;
        }
        isTransitioning = true;
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.FadeAndLoadScene("MiniGame1", 1.5f);
        }
    }
}
