using UnityEngine;

public class SettingPanel : MonoBehaviour
{
    private bool isCountingDown = false;
    public static SettingPanel Instance { get; private set; }
    public PauseCountDown Count;
    private void Awake()
    {
        // --- 싱글톤 및 씬 전환 방지 로직 ---
        if (Instance == null)
        {
            Instance = this;
            // 이 오브젝트가 씬이 바뀌어도 파괴되지 않게 설정
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 만약 다른 씬에서 또 생성되려 하면 중복이므로 파괴
            Destroy(gameObject);
            return;
        }
    }
    [Header("Panels")]
    [SerializeField] private GameObject basePanel;   // 기본 패널
    [SerializeField] private GameObject soundPanel;  // 사운드 패널

    void Update()
    {
        if (isCountingDown) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscape();
        }
    }
    public void SetIsCountingDown(bool value)
    {
        isCountingDown = value;
    }
    private void HandleEscape()
    {
        if (soundPanel.activeSelf)
        {
            soundPanel.SetActive(false);
            StartResumeSequence();
            return;
        }

        if (basePanel.activeSelf)
        {
            basePanel.SetActive(false);
            StartResumeSequence();
            return;
        }
        
        OpenBasePanel(); //아무 패널도 안 열려있으면 베이스 열기
    }
    private void StartResumeSequence()
    {
        if (PauseCountDown.Instance != null)
        {
            isCountingDown = true; // 입력 차단 시작
            PauseCountDown.Instance.ResumeGameCountDown();
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
    public void OpenBasePanel() //esc 베이스 / 사운드->베이스 공통함수 사운드끄고 베이스켜기
    {
        basePanel.SetActive(true);
        soundPanel.SetActive(false);
        PauseGame();
    }

    public void CloseBasePanel()
    {
        basePanel.SetActive(false);
        StartResumeSequence();
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
    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public bool IsAnyPanelOpen()
    {
        return basePanel.activeSelf || soundPanel.activeSelf;
    }
}
