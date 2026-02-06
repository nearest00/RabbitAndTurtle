using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class N_221NextScene : MonoBehaviour
{
    private bool isTransitioning = false;
    public bool isCountingDown
    {
        get => PauseCountDown.Instance.isCounting;
        set => PauseCountDown.Instance.isCounting = value;
    }
    public void ButtonClick()
    {
        if (isTransitioning) return;
        if (isCountingDown) return;
        if (SettingPanel.Instance != null && SettingPanel.Instance.IsAnyPanelOpen())
        {
            return;
        }
        isTransitioning = true;
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.FadeAndLoadScene("2-2-2Game", 1.5f);
        }
    }
}
