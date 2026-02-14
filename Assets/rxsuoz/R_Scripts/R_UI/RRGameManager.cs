using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RRGameManager : MonoBehaviour
{
    [Header("Song and Data")]
    public RRSongData song;

    [Header("Managers")]
    public R_NoteManager noteManager;
    public RRGuideNoteManager guideManager;

    [Header("UI")]
    public Slider scoreSlider;

    [Header("Judge Popup")]
    public GameObject judgeTextPrefab;
    public RectTransform judgeParent;

    [Header("Audio")]
    public AudioSource musicSource;

    private int score = 0;
    private double dspStartTime = 0.0;
    private bool started = false;
    private List<RRNoteData> notes = new List<RRNoteData>();
    private double songLength = 0.0;

    public string currentDifficulty
    {
        get => N_StageSellectButton.Instance.StageDifficulty;
        set => N_StageSellectButton.Instance.StageDifficulty = value;
    }

    void Start()
    {
        if (song == null)
        {
            Debug.LogError("GameManager: SongData not assigned.");
            return;
        }

        //     
        StartCoroutine(InitializeAfterSliderReady());
    }

    private IEnumerator InitializeAfterSliderReady()
    {
        //  N_221LifeSlider    
        while (N_221LifeSlider.Instance == null || N_221LifeSlider.Instance.targetSlider == null)
        {
            yield return null;
        }

        //    
        string diff = string.IsNullOrEmpty(currentDifficulty)
            ? "easy"
            : currentDifficulty.ToLower();

        Debug.Log($"[GameManager] Detected difficulty from StageSelect: {diff}");

        //   
        SetSliderMaxByDifficulty(diff);

        //   
        TextAsset csv = GetCsvForDifficulty(song, diff);
        notes = LoadChart(csv);

        if (guideManager != null)
            guideManager.Init(this, notes);

        if (noteManager != null)
        {
            noteManager.judgeManager = this;
            noteManager.StartNotes(notes);
        }

        //    
        if (musicSource != null && musicSource.clip != null)
            songLength = musicSource.clip.length;
        else
        {
            double last = 0.0;
            foreach (var n in notes)
                if (n.time > last)
                    last = n.time;
            songLength = last + 1.0;
        }

        //   
        dspStartTime = AudioSettings.dspTime + 0.1;
        if (musicSource != null && song.musicClip != null)
        {
            musicSource.clip = song.musicClip;
            musicSource.PlayScheduled(dspStartTime);
        }

        started = true;
        score = 0;
    }

    void Update()
    {
        if (!started) return;

        if (scoreSlider != null && N_221LifeSlider.Instance != null)
        {
            scoreSlider.value = N_221LifeSlider.Instance.internalValue;
        }
    }

    private void SetSliderMaxByDifficulty(string diff)
    {
        N_221LifeSlider lifeSlider = N_221LifeSlider.Instance;
        if (lifeSlider == null)
        {
            Debug.LogWarning("[GameManager] N_221LifeSlider instance not found!");
            return;
        }

        float maxVal = 550f;
        if (diff == "normal") maxVal = 800f;
        else if (diff == "hard") maxVal = 1200f;

        if (lifeSlider.targetSlider != null)
        {
            lifeSlider.targetSlider.maxValue = maxVal;
            lifeSlider.Max = maxVal;
        }

        lifeSlider.internalValue = 0f;
        Debug.Log($"[GameManager] Slider maxValue set to {maxVal} for difficulty {diff}");
    }

    public void AddScore(int delta)
    {
        if (N_221LifeSlider.Instance != null)
        {
            N_221LifeSlider.Instance.AddValue(delta);
            score = (int)N_221LifeSlider.Instance.internalValue;

            if (scoreSlider != null)
                scoreSlider.value = N_221LifeSlider.Instance.internalValue;
        }
    }

    public double GetSongTime()
    {
        if (!started) return 0.0;
        return AudioSettings.dspTime - dspStartTime;
    }

    private TextAsset GetCsvForDifficulty(RRSongData s, string diff)
    {
        if (s == null) return null;

        if (diff == "easy" && s.chartEasy != null) return s.chartEasy;
        if (diff == "normal" && s.chartNormal != null) return s.chartNormal;
        if (diff == "hard" && s.chartHard != null) return s.chartHard;

        if (s.chartEasy != null) return s.chartEasy;
        if (s.chartNormal != null) return s.chartNormal;
        if (s.chartHard != null) return s.chartHard;

        return null;
    }

    private List<RRNoteData> LoadChart(TextAsset csv)
    {
        var list = new List<RRNoteData>();
        if (csv == null) return list;

        var lines = csv.text.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (string.IsNullOrEmpty(line)) continue;

            var cols = line.Split(',');
            if (cols.Length < 3) continue;

            double t = 0.0;
            double h = 0.0;
            double.TryParse(cols[0].Trim(), out t);
            string lane = cols[1].Trim().ToLower();
            string type = cols[2].Trim().ToLower();
            if (cols.Length >= 4) double.TryParse(cols[3].Trim(), out h);

            list.Add(new RRNoteData()
            {
                time = t,
                lane = lane,
                type = type,
                holdDuration = h
            });
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
        if (popup != null)
            popup.Play(label);
    }
}
