using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;

public class N221_GameManager : MonoBehaviour
{
    public N_221Ending Ending;
    private bool isEnding = false;
    private N_221SFXList sfx;

    [Header("Timer Settings")]
    public TMP_Text timerText;
    public float easyTimeLimit = 60f;
    public float normalTimeLimit = 70f;
    public float hardTimeLimit = 80f;
    private float currentTimeLimit;
    private float timeRemaining;
    private bool isTimerRunning = false;

    [Header("2D References")] // UI에서 일반 Prefab으로 변경
    public GameObject fishPrefab;
    public Transform fishParent; // 일반 Transform

    [Header("2D Settings")] // 좌표 단위를 일반 Unit(소수점)으로 설정 권장
    public float[] laneY = { 2.0f, -2.0f };
    public float moveStep = 2.5f;
    public float playerX = 7.0f;
    float nextSpawnX;

    [Header("Difficulty Count Settings")]
    public int easyMaxCount = 55;
    public int normalMaxCount = 85;
    public int hardMaxCount = 120;

    private int currentSpawnedCount = 0;
    private int currentClearedCount = 0;
    private int targetTotalCount = 0;

    [Header("State")]
    private List<Fish> activeFishes = new List<Fish>(); // UI_Fish -> Fish

    [Header("Player")]
    public N221_CharacterMove playerScript;

    public string currentDifficulty
    {
        get => N_StageSellectButton.Instance.StageDifficulty;
        set => N_StageSellectButton.Instance.StageDifficulty = value;
    }
    public bool isCounting
    {
        get => PauseCountDown.Instance.isCounting;
        set => PauseCountDown.Instance.isCounting = value;
    }
    public float Max
    {
        get => N_221LifeSlider.Instance.Max;
        set => N_221LifeSlider.Instance.Max = value;
    }

    void Start()
    {
        sfx = Object.FindFirstObjectByType<N_221SFXList>();
        SetDifficulty(currentDifficulty);

        for (int i = 0; i < 8; i++)
        {
            if (activeFishes.Count > 0)
            {
                // 리스트의 마지막 물고기 X좌표에서 moveStep만큼 왼쪽으로
                nextSpawnX = activeFishes[activeFishes.Count - 1].transform.position.x - moveStep;
                Debug.Log(activeFishes[activeFishes.Count - 1].transform.position.x - moveStep);
            }
            else
            {
                // 아예 처음 만드는 한 마리는 기준점에서 한 칸 떨어진 곳
                nextSpawnX = playerX - moveStep;
            }
            SpawnNewFish(nextSpawnX);
        }
    }

    public void SetDifficulty(string difficulty)
    {
        switch (currentDifficulty.ToLower())
        {
            case "easy": targetTotalCount = easyMaxCount; currentTimeLimit = easyTimeLimit; break;
            case "normal": targetTotalCount = normalMaxCount; currentTimeLimit = normalTimeLimit; break;
            case "hard": targetTotalCount = hardMaxCount; currentTimeLimit = hardTimeLimit; break;
        }
        N_221LifeSlider.Instance.Max = targetTotalCount * 10;
        N_221LifeSlider.Instance.targetSlider.maxValue = N_221LifeSlider.Instance.Max;
        timeRemaining = currentTimeLimit;
        isTimerRunning = true;
    }

    void Update()
    {
        if (isEnding) return;
        if (PauseCountDown.Instance != null && isCounting) return;

        if (Input.GetKeyDown(KeyCode.UpArrow)) ProcessStep(0);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) ProcessStep(1);

        if (isTimerRunning)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                isTimerRunning = false;
                UpdateTimerUI();
                isEnding = true;
                Ending.StageFailed();
                return;
            }
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            if (timeRemaining <= 10f) timerText.color = Color.red;
        }
    }

    void ProcessStep(int inputLane)
    {
        if (SoundManager.Instance != null && sfx != null) SoundManager.Instance.PlaySFX(sfx.SFX);

        if (playerScript != null) playerScript.ChangeLane(laneY[inputLane]);

        if (activeFishes.Count > 0)
        {
            Fish frontFish = activeFishes[0];
            if (frontFish.lane == inputLane)
            {
                N_221LifeSlider.Instance.AddValue(-50f);
                return;
            }
            N_221LifeSlider.Instance.AddValue(10f);
            activeFishes.RemoveAt(0);
            Destroy(frontFish.gameObject);
            currentClearedCount++;
        }
        float lastFishXBeforeMove = playerX;
        if (activeFishes.Count > 0)
        {
            // 리스트의 마지막 물고기(가장 왼쪽)의 현재 위치
            lastFishXBeforeMove = activeFishes[activeFishes.Count - 1].transform.position.x;
        }
        foreach (Fish f in activeFishes) f.MoveRight(moveStep);

        if (currentSpawnedCount < targetTotalCount)
        {
            if (activeFishes.Count > 0)
            {
                nextSpawnX = lastFishXBeforeMove;
            }
            else
                nextSpawnX = playerX - moveStep;

            SpawnNewFish(nextSpawnX);
        }

        if (currentClearedCount >= targetTotalCount)
        {
            isEnding = true;
            isTimerRunning = false;
            EndingJud();
        }
    }

    void SpawnNewFish(float xPos)
    {
        int randomLane = Random.Range(0, 2);
        GameObject go = Instantiate(fishPrefab, fishParent);
        Fish fish = go.GetComponent<Fish>();
        fish.SetPosition(randomLane, laneY[randomLane], xPos);
        activeFishes.Add(fish);
        currentSpawnedCount++;
    }

    public void EndingJud()
    {
        if (N_221LifeSlider.Instance.internalValue / N_221LifeSlider.Instance.Max >= 0.6) Ending.StageClear();
        else Ending.StageFailed();
    }
}