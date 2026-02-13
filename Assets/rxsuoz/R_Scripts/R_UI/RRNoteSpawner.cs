// NoteManager.cs
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RRNoteManager : MonoBehaviour
{
    [Header("Tap Note Prefabs")]
    public GameObject upTapPrefab;
    public GameObject downTapPrefab;

    [Header("Long Note Prefabs")]
    public GameObject upLongPrefab;
    public GameObject downLongPrefab;

    [Header("Main Layers")]
    public RectTransform playArea;  // 노트가 부모로 들어갈 RectTransform
    public RectTransform hitLine;   // 판정선 RectTransform

    [Header("External References")]
    public RRGameManager judgeManager; // GameManager (타이밍 및 점수 담당)

    [Header("Note Settings")]
    public float travelTime = 1.5f;  // spawn -> hit까지 걸리는 시간(초)
    public float spawnY = 350f;      // 생성 Y위치(anchoredPosition.y) 기본값

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
        double songTime = judgeManager.GetSongTime();

        while (nextIndex < noteList.Count)
        {
            var note = noteList[nextIndex];
            double spawnTime = note.time - travelTime;
            if (songTime >= spawnTime)
            {
                SpawnNote(note);
                nextIndex++;
            }
            else break;
        }
    }

    void SpawnNote(RRNoteData data)
    {
        GameObject prefab = null;
        if (data.type == "tap")
            prefab = (data.lane == "up") ? upTapPrefab : downTapPrefab;
        else if (data.type == "long")
            prefab = (data.lane == "up") ? upLongPrefab : downLongPrefab;

        if (prefab == null)
        {
            Debug.LogWarning($"NoteManager: prefab not assigned for {data.type}/{data.lane}");
            return;
        }

        GameObject go = Instantiate(prefab, playArea);
        RectTransform rt = go.GetComponent<RectTransform>();

        float x = (data.lane == "up") ? -100f : 100f; // lane 좌우 위치 조정
        rt.anchoredPosition = new Vector2(x, spawnY);

        RRNote noteComp = go.GetComponent<RRNote>();
        if (noteComp == null) noteComp = go.AddComponent<RRNote>();

        noteComp.Init(data, judgeManager, hitLine.anchoredPosition.y, spawnY, travelTime);
    }
}
