using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SceneClear : MonoBehaviour
{
    private bool isTransitioning = false;
    public bool isCountingDown
    {
        get => PauseCountDown.Instance.isCounting;
        set => PauseCountDown.Instance.isCounting = value;
    }
    public void ButtonClick(int sceneName)
    {
        if (isTransitioning) {
            Debug.Log("isTransitioning");
            return;
        }

        if (isCountingDown)
        {
            Debug.Log("isCountingDown");
            return;
        }
        if (SettingPanel.Instance != null && SettingPanel.Instance.IsAnyPanelOpen())
        {
            Debug.Log("SettingPanel");
            return;
        }
        isTransitioning = true;
        if (SoundManager.Instance != null)
        {
            Debug.Log("실행");
            SoundManager.Instance.StageFadeAndLoadScene(sceneName, 1.5f);
        }
    }
	public void ButtonClick2(string sceneName)
	{
		if (isTransitioning)
		{
			Debug.Log("isTransitioning");
			return;
		}

		if (isCountingDown)
		{
			Debug.Log("isCountingDown");
			return;
		}
		if (SettingPanel.Instance != null && SettingPanel.Instance.IsAnyPanelOpen())
		{
			Debug.Log("SettingPanel");
			return;
		}
		isTransitioning = true;
		if (SoundManager.Instance != null)
		{
			Debug.Log("실행");
			SoundManager.Instance.FadeAndLoadScene(sceneName, 1.5f);
		}
	}
}
