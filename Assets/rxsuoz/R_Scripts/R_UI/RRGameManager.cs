using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RRGameManager : MonoBehaviour
{
    [Header("Song & Audio Settings")]
    public RRSongData song;               //  (ScriptableObject)
    public AudioSource musicSource;     // BGM (     )

    [Header("Main UI References")]
    public RectTransform playArea;      //   
    public RectTransform hitLine;       // 
    public Text scoreText;              //   

    [Header("Judge Popup UI")]
    public GameObject judgeTextPrefab;  // Perfect/Great    
    public RectTransform judgeParent;   //    (Canvas)

    [Header("Managers")]
    public RRNoteManager noteManager;     //   
    public Transform noteParent => playArea; // InputHandler 

    //  
    private List<RRNoteData> notes = new List<RRNoteData>();
    private double dspSongStartTime;
    private int score = 0;

    void Start()
    {
        // 1. SongData   
        if (song == null)
        {
            Debug.LogError(" SongData   !");
            return;
        }

        // 2.  
        LoadChartFromCsv();

        // 3. NoteManager 
        if (noteManager != null)
        {
            noteManager.judgeManager = this;
            noteManager.playArea = playArea;
            noteManager.hitLine = hitLine;
            noteManager.StartNotes(notes);
        }

        // 4.  
        if (scoreText != null)
            scoreText.text = "Score: 0";

        // 5.   
        StartSong();
    }

    //  CSV   
    void LoadChartFromCsv()
    {
        notes.Clear();

        if (song.chartCsv == null)
        {
            Debug.LogWarning(" chartCsv  .");
            return;
        }

        var lines = song.chartCsv.text.Split(
            new char[] { '\n', '\r' },
            System.StringSplitOptions.RemoveEmptyEntries
        );

        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (string.IsNullOrEmpty(line)) continue;

            var cols = line.Split(',');
            if (cols.Length < 3) continue;

            double time = 0;
            double hold = 0;
            double.TryParse(cols[0].Trim(), out time);   // ()
            string lane = cols[1].Trim().ToLower();      // "up" / "down"
            string type = cols[2].Trim().ToLower();      // "tap" / "long"

            if (cols.Length >= 4)
                double.TryParse(cols[3].Trim(), out hold); //  ()

            notes.Add(new RRNoteData()
            {
                time = time,
                lane = lane,
                type = type,
                holdDuration = hold
            });
        }

        //   ()
        notes.Sort((a, b) => a.time.CompareTo(b.time));

        Debug.Log($"    ({notes.Count} )");
    }

    //   
    public void StartSong()
    {
        dspSongStartTime = AudioSettings.dspTime + 0.1f;

        // BGM   (   )
        if (musicSource != null && song.musicClip != null)
        {
            musicSource.clip = song.musicClip;
            musicSource.PlayScheduled(dspSongStartTime);
        }

        Debug.Log("  !");
    }

    //    ()  ( )
    public double GetSongTime()
    {
        return AudioSettings.dspTime - dspSongStartTime;
    }

    //   
    public void AddScore(int delta)
    {
        score += delta;
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    //     (Perfect, Great )
    public void ShowJudgeAt(string judgeLabel, Vector2 anchoredPos)
    {
        if (judgeTextPrefab == null || judgeParent == null)
        {
            Debug.LogWarning(" JudgeTextPrefab  JudgeParent  .");
            return;
        }

        GameObject go = Instantiate(judgeTextPrefab, judgeParent);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;

        RRJudgePopup popup = go.GetComponent<RRJudgePopup>();
        if (popup != null)
        {
            popup.Play(judgeLabel);
        }
        else
        {
            //  JudgePopup    
            var txt = go.GetComponent<Text>();
            if (txt != null) txt.text = judgeLabel;
            Destroy(go, 1.0f);
        }
    }
}
