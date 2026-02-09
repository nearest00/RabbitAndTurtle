using UnityEngine;
using UnityEngine.SceneManagement;

// 게임 플레이를 제어하는 클래스
public class RGamePlayController : MonoBehaviour
{
    // 각 매니저 참조
    private RNoteSpawner noteSpawner;
    private RScoreManager scoreManager;

    // 게임 시작 여부
    private bool gameStarted = false;

    void Start()
    {
        // 각 매니저 찾기
        noteSpawner = FindFirstObjectByType<RNoteSpawner>();
        scoreManager = FindFirstObjectByType<RScoreManager>();


        // 게임 시작
        StartGame();
    }

    // 게임 시작 메소드
    void StartGame()
    {
        if (noteSpawner != null)
        {
            // 채보 로드 및 노트 생성 시작
            // "TestChart"는 JSON 파일명 (확장자 제외)
            // 500f는 노트 이동 속도
            noteSpawner.StartSpawning("TestChart", 500f);
            gameStarted = true;
            Debug.Log("Game started!");
        }
        else
        {
            Debug.LogError("NoteSpawner not found!");
        }
    }

    // 게임 일시정지
    void Update()
    {
        // ESC 키로 게임 일시정지 (선택사항)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    // 게임 일시정지 메소드
    void PauseGame()
    {
        Time.timeScale = Time.timeScale == 1f ? 0f : 1f;
    }
}