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
    public Slider scoreSlider;   // optional, will be synced with N_221LifeSlider if present

    [Header("Judge Popup")]
    public GameObject judgeTextPrefab;
    public RectTransform judgeParent;

    [Header("Audio")]
    public AudioSource musicSource;

    // score is delegated to N_221LifeSlider if present
    private int score = 0;
    private double dspStartTime = 0.0;
    private bool started = false;
    private List<RRNoteData> notes = new List<RRNoteData>();
    private double songLength = 0.0;

    void Start()
    {
        if (song == null)
        {
            Debug.LogError("GameManager: SongData not assigned.");
            return;
        }

        // 1) Set difficulty to slider singleton first (so slider max is ready)
        if (RLifeSlider.Instance != null)
        {
            RLifeSlider.Instance.SetDifficulty(song.difficulty);
            // optional: sync our scoreSlider (if assigned) to the same max
            if (scoreSlider != null)
            {
                scoreSlider.maxValue = RLifeSlider.Instance.Max; //
                scoreSlider.value = RLifeSlider.Instance.internalValue; //
            }
        }

        // 2) Load appropriate CSV based on difficulty
        TextAsset chosenCsv = GetCsvForDifficulty(song);
        notes = LoadChart(chosenCsv);

        // 3) Init guide manager and note manager
        if (guideManager != null) guideManager.Init(this, notes);
        if (noteManager != null)
        {
            noteManager.judgeManager = this;
            noteManager.StartNotes(notes);
        }

        // 4) determine song length
        if (musicSource != null && musicSource.clip != null)
            songLength = musicSource.clip.length;
        else
        {
            double last = 0.0;
            foreach (var n in notes) if (n.time > last) last = n.time;
            songLength = last + 1.0;
        }

        // 5) audio start
        dspStartTime = AudioSettings.dspTime + 0.1;
        if (musicSource != null && song.musicClip != null)
        {
            musicSource.clip = song.musicClip;
            musicSource.PlayScheduled(dspStartTime);
        }

        started = true;
        score = (int)RLifeSlider.Instance.internalValue; // sync

        Debug.Log("GameManager started. difficulty=" + song.difficulty + " notes=" + notes.Count);
    }

    // Helper: choose csv based on SongData difficulty
    TextAsset GetCsvForDifficulty(RRSongData s)
    {
        if (s == null) return null;
        string d = (s.difficulty == null) ? "easy" : s.difficulty.Trim().ToLower();

        if (d == "easy" && s.chartEasy != null) return s.chartEasy;
        if (d == "normal" && s.chartNormal != null) return s.chartNormal;
        if (d == "hard" && s.chartHard != null) return s.chartHard;

        // fallback priority: easy -> normal -> hard (any non-null)
        if (s.chartEasy != null) return s.chartEasy;
        if (s.chartNormal != null) return s.chartNormal;
        if (s.chartHard != null) return s.chartHard;
        return null;
    }

    void Update()
    {
        if (!started) return;
        // nothing special here; score is handled via N_221LifeSlider.AddValue from Note.ApplyJudge
        // but if you also keep scoreSlider, sync it
        if (scoreSlider != null && RLifeSlider.Instance != null)
        {
            scoreSlider.value = RLifeSlider.Instance.internalValue;
        }
    }

    // Called by Note when judged
    // We route score changes through N_221LifeSlider singleton for consistent UI + limits
    public void AddScore(int delta)
    {
        if (RLifeSlider.Instance != null)
        {
            RLifeSlider.Instance.AddValue(delta);
            // optional: update internal score var
            score = (int)RLifeSlider.Instance.internalValue;
        }
        else
        {
            // fallback behavior: update local score and optional slider
            score += delta;
            if (score < 0) score = 0;
            if (scoreSlider != null)
            {
                if (scoreSlider.maxValue <= 0) scoreSlider.maxValue = 550f;
                score = Mathf.Clamp(score, 0, (int)scoreSlider.maxValue);
                scoreSlider.value = score;
            }
        }
    }

    public double GetSongTime()
    {
        if (!started) return 0.0;
        return AudioSettings.dspTime - dspStartTime;
    }

    // CSV parsing (unchanged)
    List<RRNoteData> LoadChart(TextAsset csv)
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

        var popup = go.GetComponent<RRJudgePopup>();
        if (popup != null) popup.Play(label);
    }
}
