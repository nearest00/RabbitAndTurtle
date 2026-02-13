using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RRGameManager : MonoBehaviour
{
    [Header("Song and Data")]
    public RRSongData song;
    
    [Header("Managers")]
    public RRGuideNoteManager guideManager;
    public R_NoteManager noteManager;

    [Header("UI")]
    public Slider scoreSlider;   // 점수 표시용 슬라이더

    [Header("Judge Popup")]
    public GameObject judgeTextPrefab;
    public RectTransform judgeParent;

    [Header("Audio")]
    public AudioSource musicSource;

    [Header("Score Settings")]
    public int maxScore = 550;
    public float clearRate = 0.6f; // (현재 미사용, 향후 사용 가능)

    // 내부 상태 관리
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

        // CSV -> NoteData 파싱
        notes = LoadChart(song.chartCsv);

        // LoadChart 이후
        notes = LoadChart(song.chartCsv);

        // guide 노트 매니저 초기화
        if (guideManager != null)
            guideManager.Init(this, notes);


        // 곡 길이 설정
        if (musicSource != null && musicSource.clip != null)
        {
            songLength = musicSource.clip.length;
        }
        else
        {
            double last = 0.0;
            foreach (var n in notes)
                if (n.time > last)
                    last = n.time;
            songLength = last + 1.0;
        }

        // UI 초기화
        if (scoreSlider != null)
        {
            scoreSlider.minValue = 0;
            scoreSlider.maxValue = maxScore;
            scoreSlider.value = 0;
        }

        // NoteManager 초기화
        if (noteManager != null)
        {
            noteManager.judgeManager = this;
            noteManager.StartNotes(notes);
        }

        // 오디오 재생
        dspStartTime = AudioSettings.dspTime + 0.1;
        if (musicSource != null && song.musicClip != null)
        {
            musicSource.clip = song.musicClip;
            musicSource.PlayScheduled(dspStartTime);
        }

        started = true;
        score = 0;

        Debug.Log("GameManager started, songLength=" + songLength);
    }

    void Update()
    {
        if (!started) return;

        double cur = GetSongTime();

        // 필요 시, 여기서 자동으로 게임 종료 로직 추가 가능
        // 현재는 점수 표시만 유지
    }

    // 점수 추가 (노트 판정 시 호출됨)
    public void AddScore(int delta)
    {
        score += delta;

        if (score < 0)
            score = 0; // 음수 점수 방지

        if (score > maxScore)
            score = maxScore;

        if (scoreSlider != null)
            scoreSlider.value = score;

        Debug.Log("GameManager: score = " + score);
    }

    // 현재 곡 진행 시간 반환
    public double GetSongTime()
    {
        if (!started) return 0.0;
        return AudioSettings.dspTime - dspStartTime;
    }

    // CSV 읽기
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

            list.Add(new RRNoteData() { time = t, lane = lane, type = type, holdDuration = h });
        }

        list.Sort((a, b) => a.time.CompareTo(b.time));
        return list;
    }

    // 판정 텍스트 표시
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
