using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class N_222NextScene : MonoBehaviour
{
    private bool isTransitioning = false;
    public bool isCountingDown
    {
        get => PauseCountDown.Instance.isCounting;
        set => PauseCountDown.Instance.isCounting = value;
    }
    public void ButtonClick()
    {
        Destroy(N_222LifeSlider.Instance);
        Destroy(N_222RoundManager.Instance);
        if (isTransitioning) return;
        if (isCountingDown) return;
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
