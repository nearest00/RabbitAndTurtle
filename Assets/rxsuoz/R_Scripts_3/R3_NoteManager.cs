using System.Collections.Generic;
using UnityEngine;

public class R3_NoteManager : MonoBehaviour
{
    [Header("Tap Note Prefabs")]
    public GameObject upTapPrefab;
    public GameObject downTapPrefab;
    public GameObject leftTapPrefab;
    public GameObject rightTapPrefab;

    [Header("Long Note Prefabs")]
    public GameObject upLongPrefab;
    public GameObject downLongPrefab;
    public GameObject leftLongPrefab;
    public GameObject rightLongPrefab;

    [Header("Main")]
    public RectTransform playArea;
    public RectTransform hitLine;
    public RRGameManager judgeManager;

    [Header("Settings")]
    public float travelTime = 1.8f;
    public float judgeRadius = 60f;

    [System.Serializable]
    public class LanePosition { public string laneName; public Vector2 spawnPos; }

    [Header("Lane Positions")]
    public LanePosition[] lanePositions = new LanePosition[4]
    {
        new LanePosition(){ laneName="up", spawnPos=new Vector2(0f,600f)},
        new LanePosition(){ laneName="down", spawnPos=new Vector2(0f,-600f)},
        new LanePosition(){ laneName="left", spawnPos=new Vector2(-600f,0f)},
        new LanePosition(){ laneName="right", spawnPos=new Vector2(600f,0f)}
    };

    private List<R3_NoteData> noteList = new List<R3_NoteData>();
    private int nextIndex = 0;
    private bool spawning = false;

    public void StartNotes(List<R3_NoteData> notes)
    {
        noteList = notes;
        nextIndex = 0;
        spawning = true;
    }

    void Update()
    {
        if (!spawning || noteList == null || noteList.Count == 0) return;
        if (judgeManager == null) return;

        double time = judgeManager.GetSongTime();

        while (nextIndex < noteList.Count)
        {
            var data = noteList[nextIndex];
            double spawnTime = data.time - travelTime;

            if (time >= spawnTime)
            {
                SpawnNote(data);
                nextIndex++;
            }
            else break;
        }
    }

    void SpawnNote(R3_NoteData data)
    {
        if (data == null) return;
        GameObject prefab = null;

        if (data.type == "tap")
        {
            if (data.lane == "up") prefab = upTapPrefab;
            else if (data.lane == "down") prefab = downTapPrefab;
            else if (data.lane == "left") prefab = leftTapPrefab;
            else if (data.lane == "right") prefab = rightTapPrefab;
        }
        else if (data.type == "long")
        {
            if (data.lane == "up") prefab = upLongPrefab;
            else if (data.lane == "down") prefab = downLongPrefab;
            else if (data.lane == "left") prefab = leftLongPrefab;
            else if (data.lane == "right") prefab = rightLongPrefab;
        }

        if (prefab == null)
        {
            Debug.LogWarning($"Prefab missing for {data.lane} {data.type}");
            return;
        }

        GameObject go = Instantiate(prefab, playArea);
        RectTransform rt = go.GetComponent<RectTransform>();
        if (rt == null) rt = go.AddComponent<RectTransform>();

        Vector2 spawnPos = GetLaneSpawnPos(data.lane);
        rt.anchoredPosition = spawnPos;

        R3_Note n = go.GetComponent<R3_Note>();
        if (n == null) n = go.AddComponent<R3_Note>();

        float spawnPrimary = (data.lane == "left" || data.lane == "right") ? spawnPos.x : spawnPos.y;

        n.Init(data, judgeManager, hitLine, spawnPrimary, travelTime, judgeRadius);
    }

    Vector2 GetLaneSpawnPos(string lane)
    {
        foreach (var p in lanePositions)
            if (p.laneName == lane) return p.spawnPos;

        return new Vector2(0f, 600f);
    }
}
