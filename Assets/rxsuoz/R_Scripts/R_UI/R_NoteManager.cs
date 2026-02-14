using System.Collections.Generic;
using UnityEngine;

public class R_NoteManager : MonoBehaviour
{
    [Header("Tap Note Prefabs")]
    public GameObject upTapPrefab;
    public GameObject downTapPrefab;

    [Header("Long Note Prefabs")]
    public GameObject upLongPrefab;
    public GameObject downLongPrefab;

    [Header("Main Layers")]
    public RectTransform playArea;
    public RectTransform hitLine;

    [Header("External References")]
    public RRGameManager judgeManager;

    [Header("Note Settings")]
    public float travelTime = 1.8f;
    public float judgeRadius = 60f;

    [System.Serializable]
    public class LanePosition
    {
        public string laneName;
        public Vector2 spawnPos;
    }

    [Header("Lane Positions (edit manually)")]
    public LanePosition[] lanePositions = new LanePosition[2]
    {
        new LanePosition(){ laneName = "up", spawnPos = new Vector2(0f, 600f) },
        new LanePosition(){ laneName = "down", spawnPos = new Vector2(0f, 600f) }
    };

    private List<RRNoteData> noteList = new List<RRNoteData>();
    private int nextIndex = 0;
    private bool spawning = false;

    public void StartNotes(List<RRNoteData> notes)
    {
        noteList = notes;
        nextIndex = 0;
        spawning = true;
    }

    void Update()
    {
        if (!spawning || noteList == null || noteList.Count == 0) return;
        if (judgeManager == null) return;

        double songTime = judgeManager.GetSongTime();

        while (nextIndex < noteList.Count)
        {
            var data = noteList[nextIndex];
            double spawnTime = data.time - travelTime;

            if (songTime >= spawnTime)
            {
                SpawnNote(data);
                nextIndex++;
            }
            else break;
        }
    }

    void SpawnNote(RRNoteData data)
    {
        if (data == null) return;

        GameObject prefab = null;

        // Select prefab based on note type and lane
        if (data.type == "tap")
        {
            if (data.lane == "up") prefab = upTapPrefab;
            else if (data.lane == "down") prefab = downTapPrefab;
        }
        else if (data.type == "long")
        {
            if (data.lane == "up") prefab = upLongPrefab;
            else if (data.lane == "down") prefab = downLongPrefab;
        }

        if (prefab == null)
        {
            Debug.LogWarning("NoteManager: prefab missing for lane=" + data.lane + " type=" + data.type);
            return;
        }

        // Instantiate note
        GameObject go = Instantiate(prefab, playArea);
        RectTransform rt = go.GetComponent<RectTransform>();
        if (rt == null) rt = go.AddComponent<RectTransform>();

        // Get spawn position for this lane
        Vector2 spawnPos = GetLaneSpawnPos(data.lane);
        rt.anchoredPosition = spawnPos;

        RRNote noteComp = go.GetComponent<RRNote>();
        if (noteComp == null) noteComp = go.AddComponent<RRNote>();

        noteComp.Init(data, judgeManager, hitLine, spawnPos.y, travelTime, judgeRadius);
    }

    Vector2 GetLaneSpawnPos(string lane)
    {
        foreach (var lanePos in lanePositions)
        {
            if (lanePos.laneName == lane)
                return lanePos.spawnPos;
        }

        // default position
        return new Vector2(0f, 600f);
    }
}
