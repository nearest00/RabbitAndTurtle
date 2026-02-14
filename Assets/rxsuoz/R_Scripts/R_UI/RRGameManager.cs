using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RRGameManager : MonoBehaviour
{
    [Header("Song and Data")]
    public RRSongData song;
    [Tooltip("노래의 총 길이를 초 단위로 입력하세요 (예: 90.5)")]
    public float manualSongLength;

    [Header("Managers")]
    public R_NoteManager noteManager;
    public RRGuideNoteManager guideManager;

    [Header("UI")]
    public Slider scoreSlider;
    // public GameObject clearPanel; // 주석 처리
    // public GameObject failPanel;  // 주석 처리

    [Header("Judge Popup")]
    public GameObject judgeTextPrefab;
    public RectTransform judgeParent;

    [Header("Panel Settings")]
    public float panelFadeDuration = 0.4f;

    private int score = 0;
    private bool started = false;
    private bool resultShown = false;
    private List<RRNoteData> notes = new List<RRNoteData>();

    private float gameTimer = 0f;

    public string currentDifficulty
    {
        get => N_StageSellectButton.Instance.StageDifficulty;
        set => N_StageSellectButton.Instance.StageDifficulty = value;
    }

    void Start()
    {
        if (song == null)
        {
            Debug.LogError("GameManager: SongData가 할당되지 않았습니다.");
            return;
        }

        InitializeGame();
    }

    private void InitializeGame()
    {
        string diff = string.IsNullOrEmpty(currentDifficulty) ? "easy" : currentDifficulty.ToLower();
        SetSliderMaxByDifficulty(diff);

        TextAsset csv = GetCsvForDifficulty(song, diff);
        notes = LoadChart(csv);

        if (guideManager != null) guideManager.Init(this, notes);
        if (noteManager != null)
        {
            noteManager.judgeManager = this;
            noteManager.StartNotes(notes);
        }

        started = true;
        score = 0;
        resultShown = false;
        gameTimer = 0f;

        Debug.Log($"게임 시작! 설정된 노래 길이: {manualSongLength}초");
    }

    void Update()
    {
        if (Time.timeScale <= 0 || (PauseCountDown.Instance != null && PauseCountDown.Instance.isCounting))
        {
            return;
        }

        if (!started) return;

        gameTimer += Time.deltaTime;

        if (scoreSlider != null && RLifeSlider.Instance != null)
            scoreSlider.value = RLifeSlider.Instance.internalValue;

        // 종료 조건 체크 (패널 표시 함수 호출 부분 주석 처리)
        if (!resultShown && gameTimer >= manualSongLength)
        {
            // ShowResultPanel(); // 결과 패널 표시 중단
            resultShown = true;
            Debug.Log("노래가 종료되었습니다.");
        }
    }

    public double GetSongTime()
    {
        return (double)gameTimer;
    }

    /* 패널 관련 함수들 전체 주석 처리
    private void ShowResultPanel()
    {
        N_221LifeSlider lifeSlider = N_221LifeSlider.Instance;
        if (lifeSlider == null) return;

        float currentScore = lifeSlider.internalValue;
        float maxScore = lifeSlider.Max;
        float percent = (currentScore / maxScore) * 100f;

        if (currentScore >= 0 && percent >= 60f) ShowPanel(clearPanel);
        else ShowPanel(failPanel);
    }

    private void ShowPanel(GameObject panel)
    {
        if (panel == null) return;
        var fade = panel.GetComponent<RRPanelFade>();
        if (fade != null) fade.FadeIn(panelFadeDuration);
        else panel.SetActive(true);
    }
    */

    private void SetSliderMaxByDifficulty(string diff)
    {
        RLifeSlider lifeSlider = RLifeSlider.Instance;
        if (lifeSlider == null) return;

        float maxVal = 550f;
        if (diff == "normal") maxVal = 800f;
        else if (diff == "hard") maxVal = 1200f;

        if (lifeSlider.targetSlider != null)
        {
            lifeSlider.targetSlider.maxValue = maxVal;
            lifeSlider.Max = maxVal;
        }
        lifeSlider.internalValue = 0f;
    }

    public void AddScore(int delta)
    {
        if (RLifeSlider.Instance != null)
        {
            RLifeSlider.Instance.AddValue(delta);
            score = (int)RLifeSlider.Instance.internalValue;
        }
    }

    private TextAsset GetCsvForDifficulty(RRSongData s, string diff)
    {
        if (s == null) return null;
        if (diff == "easy" && s.chartEasy != null) return s.chartEasy;
        if (diff == "normal" && s.chartNormal != null) return s.chartNormal;
        if (diff == "hard" && s.chartHard != null) return s.chartHard;
        return s.chartEasy;
    }

    private List<RRNoteData> LoadChart(TextAsset csv)
    {
        var list = new List<RRNoteData>();
        if (csv == null) return list;
        var lines = csv.text.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (var raw in lines)
        {
            var cols = raw.Trim().Split(',');
            if (cols.Length < 3) continue;
            double t = 0.0; double.TryParse(cols[0], out t);
            double h = 0.0; if (cols.Length >= 4) double.TryParse(cols[3], out h);
            list.Add(new RRNoteData() { time = t, lane = cols[1].Trim().ToLower(), type = cols[2].Trim().ToLower(), holdDuration = h });
        }
        list.Sort((a, b) => a.time.CompareTo(b.time));
        return list;
    }

    public void ShowJudgeAt(string label, Vector2 anchoredPos)
    {
        if (judgeTextPrefab == null || judgeParent == null) return;
        GameObject go = Instantiate(judgeTextPrefab, judgeParent);
        RectTransform rt = go.GetComponent<RectTransform>();
        if (rt != null) rt.anchoredPosition = anchoredPos;
        RRJudgePopup popup = go.GetComponent<RRJudgePopup>();
        if (popup != null) popup.Play(label);
    }

    public void OnClearButton() => SceneManager.LoadScene("StageSellect");
    public void OnRetryButton() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}