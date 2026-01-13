using UnityEngine;

public class SettingPanel : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject basePanel;   // 기본 패널
    [SerializeField] private GameObject soundPanel;  // 사운드 패널

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscape();
        }
    }

    private void HandleEscape()
    {
        if (soundPanel.activeSelf)
        {
            soundPanel.SetActive(false);
            PauseGame();
            return;
        }

        if (basePanel.activeSelf)
        {
            basePanel.SetActive(false);
            ResumeGame();
            return;
        }
        
        OpenBasePanel();
    }

    // =====================
    // 기본 패널
    // =====================

    public void OpenBasePanel()
    {
        basePanel.SetActive(true);
        soundPanel.SetActive(false);
        PauseGame();
    }

    public void CloseBasePanel()
    {
        basePanel.SetActive(false);
        ResumeGame();
    }

    // 기본 패널 버튼 1
    public void OnCloseBasePanelButton()
    {
        CloseBasePanel();
    }

    // 기본 패널 버튼 2 (사운드 설정)
    public void OnOpenSoundPanelButton()
    {
        basePanel.SetActive(false);
        soundPanel.SetActive(true);
        PauseGame();
    }
    public void OnCloseSoundPanelButton()
    {
        soundPanel.SetActive(false);
        basePanel.SetActive(true);
    }
    public void OnExitGameButton()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }
    // =====================
    // 공통
    // =====================

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
