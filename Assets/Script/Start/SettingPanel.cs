using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingPanel : MonoBehaviour
{
    public static SettingPanel Instance { get; private set; }
    public bool CanSettingOn;
    public bool isCountingDown
    {
        get => PauseCountDown.Instance.isCounting;
        set=>PauseCountDown.Instance.isCounting = value;
    }
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
    private void Start()
    {
        CanSettingOn = true;
        Debug.Log("설정패널 사용가능");
    }
    void Update()
    {
        if (isCountingDown) return; 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(!CanSettingOn) return;
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
        SoundManager.Instance.PauseAllSounds();
        OpenBasePanel();
    }
    private void StartResumeSequence()
    {
        if (PauseCountDown.Instance != null)
        {
            basePanel.SetActive(false);
            soundPanel.SetActive(false);

            isCountingDown = true;
            PauseCountDown.Instance.ResumeGameCountDown();
        }
        else
        {
            basePanel.SetActive(false);
            soundPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }
    public void OpenBasePanel() 
    {
        basePanel.SetActive(true);
        soundPanel.SetActive(false);
        PauseGame();
    }

    public void CloseBasePanel()
    {
        Debug.Log("클로즈베이스패널실행");
        basePanel.SetActive(false);
        if (PauseCountDown.Instance != null)
        {
            PauseCountDown.Instance.ResumeGameCountDown();
        }
    }

    // 기본 패널 버튼 1
    public void OnCloseBasePanelButton()
    {
        Debug.Log("온클로즈베이스패널버튼 클릭");
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
    public void OnResetGameButton()
    {
        Time.timeScale = 1f;
        SoundManager.Instance.ResetBGM();
        // 2. 현재 씬 이름을 가져와서 다시 로드
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
        basePanel.SetActive(false);
        Debug.Log($"{sceneName} 씬을 완전히 새로 시작합니다.");
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
