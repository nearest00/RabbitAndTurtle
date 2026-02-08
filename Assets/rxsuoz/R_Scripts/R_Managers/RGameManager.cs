using UnityEngine;
using UnityEngine.SceneManagement;

// 게임 전체를 관리하는 클래스 (싱글톤 패턴)
public class RGameManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static RGameManager instance;

    // 현재 선택된 채보 파일명
    public string currentChart = "TestChart";

    // 현재 선택된 난이도
    public string currentDifficulty = "EASY";

    // 현재 음악 재생 시간
    private float musicTime = 0f;

    // 음악 소스
    private AudioSource audioSource;

    void Awake()
    {
        // 싱글톤 패턴: 게임 중 하나의 GameManager만 존재하도록 보장
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 전환 시에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // AudioSource 컴포넌트 가져오기
        audioSource = GetComponent<AudioSource>();
    }

    // 게임 시작
    public void StartGame(string chartName, string difficulty)
    {
        currentChart = chartName;
        currentDifficulty = difficulty;

        Debug.Log($"Starting game: {chartName} - {difficulty}");

        // GamePlayScene으로 이동
        SceneManager.LoadScene("GamePlayScene");
    }

    // 메뉴로 돌아가기
    public void GoToMenu()
    {
        Time.timeScale = 1f;  // 게임 일시정지 해제
        SceneManager.LoadScene("MenuScene");
    }

    // 결과 화면으로 이동
    public void GoToResult()
    {
        Time.timeScale = 1f;  // 게임 일시정지 해제
        SceneManager.LoadScene("ResultScene");
    }

    // 게임 일시정지
    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    // 게임 재개
    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    // 현재 음악 시간 반환
    public float GetMusicTime()
    {
        if (audioSource != null)
        {
            return audioSource.time;
        }
        return 0f;
    }
}