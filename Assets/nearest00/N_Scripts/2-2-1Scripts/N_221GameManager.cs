using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class N221_GameManager : MonoBehaviour
{
    public N_221Ending Ending;
    private bool isEnding=false;
    private N_221SFXList sfx;
    [Header("Timer Settings")]
    public TMP_Text timerText; // 시간을 표시할 UI 텍스트
    public float easyTimeLimit = 60f;
    public float normalTimeLimit = 70f;
    public float hardTimeLimit = 80f;
    private float currentTimeLimit;
    private float timeRemaining;
    private bool isTimerRunning = false;

    [Header("UI References")]
    public GameObject fishPrefab;
    public Transform canvasParent;

    [Header("UI Settings")]
    public float[] laneY = { 150f, -150f }; // 위(0), 아래(1) Y좌표
    public float moveStep = 200f;           // 칸 간격
    public float playerX = 500f;            // 플레이어 X 위치 (오른쪽)
    [Header("Difficulty Count Settings")]
    public int easyMaxCount = 55;
    public int normalMaxCount = 85;
    public int hardMaxCount = 120;

    private int currentSpawnedCount = 0; // 지금까지 생성된 총 개수
    private int currentClearedCount = 0;  // 지금까지 피한 총 개수
    private int targetTotalCount = 0;    // 이번 판 목표 개수
    [Header("State")]
    [HideInInspector]
    private List<UI_Fish> activeFishes = new List<UI_Fish>();

    [Header("Player")]
    public N221_CharacterMove playerScript; // 플레이어 스크립트 연결
    void Start()
    {
        sfx=Object.FindFirstObjectByType<N_221SFXList>();
        SetDifficulty("easy");
        // 게임 시작 시 플레이어 앞에 8개의 물고기를 미리 생성
        for (int i = 0; i < 8; i++)
        {
            // 플레이어로부터 왼쪽으로 i칸 떨어진 위치 계산
            float targetX = playerX - ((i + 1) * moveStep);
            SpawnNewFish(targetX);
        }
    }
    public void SetDifficulty(string difficulty)
    {
        // 1. 목표 개수 설정
        switch (difficulty.ToLower())
        {
            case "easy": 
                targetTotalCount = easyMaxCount;
                currentTimeLimit = easyTimeLimit;
                break;

            case "normal": 
                targetTotalCount = normalMaxCount;
                currentTimeLimit = normalTimeLimit; 
                break;
            case "hard": 
                targetTotalCount = hardMaxCount;
                currentTimeLimit = hardTimeLimit;
                break;
        }
        timeRemaining = currentTimeLimit;
        isTimerRunning = true;
    }

    void Update()
    {
        if (isEnding) return;
        if (Input.GetKeyDown(KeyCode.UpArrow)) ProcessStep(0);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) ProcessStep(1);
        if (isTimerRunning)
        {
            timeRemaining -= Time.deltaTime; // Time.timeScale 영향받음
            UpdateTimerUI();

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                isTimerRunning = false;
                UpdateTimerUI();
                isEnding = true;
                Ending.StageFailed(); // 시간 초과 시 실패 처리
                return;
            }
        }
    }
    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            // "00:00" 형식으로 표시
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            // 시간이 얼마 안 남았을 때 붉은색으로 변경 (선택사항)
            if (timeRemaining <= 10f) timerText.color = Color.red;
        }
    }
    void ProcessStep(int inputLane)
    {
        if (SoundManager.Instance != null && sfx != null) SoundManager.Instance.PlaySFX(sfx.SFX);
        // 1. 플레이어 캐릭터 이동 (시각적 피드백)
        if (playerScript != null)
        {
            playerScript.ChangeLane(laneY[inputLane]);
        }

        // 2. 판정 및 첫 번째 물고기 제거
        if (activeFishes.Count > 0)
        {
            UI_Fish frontFish = activeFishes[0];
            // 내가 누른 레일에 물고기가 있다면 충돌!
            if (frontFish.lane == inputLane)
            {
                Debug.Log("충돌!");
                N_221LifeSlider.Instance.AddValue(-50f);
                Debug.Log("-50");
                return;
            }
            N_221LifeSlider.Instance.AddValue(10f);
            Debug.Log("+10");
            activeFishes.RemoveAt(0);
            Destroy(frontFish.gameObject);
            currentClearedCount++;
        }

        // 3. 남은 모든 물고기 오른쪽으로 이동
        foreach (UI_Fish f in activeFishes)
        {
            f.MoveRight(moveStep);
        }

        // 4. 맨 왼쪽 끝에 새로운 물고기 하나 보충
        // 현재 리스트에 남은 물고기 개수 뒤에 오도록 좌표 계산
        if (currentSpawnedCount < targetTotalCount)
        {
            float nextSpawnX;
            if (activeFishes.Count > 0)
            {
                nextSpawnX = activeFishes[activeFishes.Count - 1].GetComponent<RectTransform>().anchoredPosition.x - moveStep;
            }
            else nextSpawnX = playerX - moveStep;
            SpawnNewFish(nextSpawnX);
        }
        if (currentClearedCount >= targetTotalCount)
        {
            isEnding = true;
            isTimerRunning = false; // 성공 시 타이머 멈춤
            EndingJud();
            return;
        }
    }

    void SpawnNewFish(float xPos)
    {
        // 0(위) 또는 1(아래) 중 무작위 결정
        int randomLane = Random.Range(0, 2);

        GameObject go = Instantiate(fishPrefab, canvasParent);
        go.transform.localScale = Vector3.one; // UI 스케일 깨짐 방지

        UI_Fish fish = go.GetComponent<UI_Fish>();

        // 초기 위치 설정 (레일 Y값과 계산된 X값)
        fish.SetUIPosition(randomLane, laneY[randomLane], xPos);

        activeFishes.Add(fish);
        currentSpawnedCount++;
    }

    public void EndingJud()
    {
        if (N_221LifeSlider.Instance.internalValue/N_221LifeSlider.Instance.Max>=0.6) 
        {
            Ending.StageClear();
        }
        else Ending.StageFailed();
    }
}