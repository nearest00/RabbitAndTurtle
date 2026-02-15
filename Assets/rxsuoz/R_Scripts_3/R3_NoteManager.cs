using System.Collections.Generic;
using UnityEngine;

public class R3_NoteManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject noteUpPrefab;
    public GameObject noteDownPrefab;
    public GameObject noteLeftPrefab;
    public GameObject noteRightPrefab;

    [Header("Settings")]
    public RectTransform playArea;
    public float travelTime = 2.0f;
    public float spawnDistanceX = 900f;

    private List<R3_NoteData> _noteQueue = new List<R3_NoteData>();
    private int _nextIndex = 0;

    void Start()
    {
        // 초기화 로직 (CSV 로드 후 호출)
        string diff = N_StageSellectButton.Instance?.StageDifficulty.ToLower() ?? "easy";
        LoadAndPrepare(diff);
    }

    private void LoadAndPrepare(string diff)
    {
        TextAsset csv = GetCsv(diff);
        if (csv == null) return;

        string[] lines = csv.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines)
        {
            string[] cols = line.Trim().Split(',');
            if (cols.Length < 3) continue;
            _noteQueue.Add(new R3_NoteData
            {
                time = double.Parse(cols[0]),
                noteDirection = cols[1].Trim().ToLower(),
                noteType = cols[2].Trim().ToLower(),
                holdDuration = cols.Length > 3 ? double.Parse(cols[3]) : 0
            });
        }
        _noteQueue.Sort((a, b) => a.time.CompareTo(b.time));
    }

    private TextAsset GetCsv(string diff)
    {
        RRSongData s = R3_GameManager.Instance.songData;
        if (diff == "normal") return s.chartNormal;
        if (diff == "hard") return s.chartHard;
        return s.chartEasy;
    }

    void Update()
    {
        if (Time.timeScale <= 0 || _nextIndex >= _noteQueue.Count) return;

        double currentTime = R3_GameManager.Instance.GetCurrentTime();
        if (currentTime >= _noteQueue[_nextIndex].time - travelTime)
        {
            Spawn(_noteQueue[_nextIndex]);
            _nextIndex++;
        }
    }

    void Spawn(R3_NoteData data)
    {
        GameObject prefab = GetPrefab(data.noteDirection);
        GameObject go = Instantiate(prefab, playArea);
        float side = Random.value > 0.5f ? 1f : -1f;
        Vector2 spawnPos = new Vector2(side * spawnDistanceX, 0f);

        go.GetComponent<R3_Note>().Initialize(data, spawnPos, travelTime);
    }

    GameObject GetPrefab(string dir) => dir switch
    {
        "up" => noteUpPrefab,
        "down" => noteDownPrefab,
        "left" => noteLeftPrefab,
        "right" => noteRightPrefab,
        _ => null
    };
}