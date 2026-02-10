using UnityEngine;
using UnityEngine.Rendering;

public class N_44GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public string currentDifficulty
    {
        get => N_StageSellectButton.Instance.StageDifficulty;
        set => N_StageSellectButton.Instance.StageDifficulty = value;
    }
    public float bpm; // 실시간 계산에 사용될 BPM
    private float startTime;
    public float Max;
    [Header("References")]
    public N_44NoteSpawner noteSpawner;
    // 인스펙터에서 3개 할당 (Element 0: Easy, 1: Normal, 2: Hard)
    public N_44Chart EasyChart;
    public N_44Chart NormalChart;
    public N_44Chart HardChart;
    void Start()
    {
        // 게임 시작 시 선택된 난이도에 맞춰 초기화
        StartGame();
    }

    void StartGame()
    {
        N_44Chart selectedChart = null;

        // 문자열 비교를 통해 차트 선택 (대소문자 구분 주의)
        // 차트 이름을 아예 소문자로 통일하거나 대문자로 통일하는 것이 안전합니다.
        string difficulty = currentDifficulty.ToLower();

        if (difficulty == "easy")
        {
            Max = 550;
            selectedChart = EasyChart;
        }
        else if (difficulty == "normal")
        {
            Max = 800;
            selectedChart = NormalChart;
        }
        else if (difficulty == "hard")
        {
            Max = 1200;
            selectedChart = HardChart;
        }

        if (selectedChart == null)
        {
            Debug.LogError($"'{currentDifficulty}'에 해당하는 차트 데이터가 없습니다!");
            return;
        }
        if (N_44LifeSlider.Instance != null)
        {
            N_44LifeSlider.Instance.Max = Max; // <-- 이 줄이 빠져있었습니다.
            N_44LifeSlider.Instance.targetSlider.maxValue = Max;
        }
        Debug.Log(N_44LifeSlider.Instance.targetSlider.maxValue);

        // 차트 정보 적용
        this.bpm = selectedChart.bpm;
        this.startTime = Time.time;

        if (noteSpawner != null)
        {
            noteSpawner.Initialize(selectedChart);
        }

        Debug.Log($"게임 시작! 난이도: {currentDifficulty}, BPM: {bpm}");
    }

    public float GetBeatTime()
    {
        if (bpm <= 0) return 0; // 0으로 나누기 방지
        float elapsedSeconds = Time.time - startTime;
        return elapsedSeconds * (bpm / 60f);
    }

    /// <summary>
    /// 박자(Beat)를 초(Seconds)로 변환합니다.
    /// </summary>
    public float BeatToSeconds(float beat)
    {
        if (bpm <= 0) return 0;
        return beat * (60f / bpm);
    }

    /// <summary>
    /// 현재 경과 시간을 '초(Seconds)' 단위로 반환합니다.
    /// </summary>
    public float GetMusicTime()
    {
        return Time.time - startTime;
    }
}