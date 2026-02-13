using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
    public RectTransform hitLine; // used for position reference

    [Header("External References")]
    public RRGameManager judgeManager;

    [Header("Note Settings")]
    public float travelTime = 1.8f;   // seconds from spawn to hit
    public float spawnYOffset = 100f; // extra offset above playArea top
    public float laneXOffset = 150f;  // horizontal separation for lanes
    public float judgeRadius = 60f;   // pixel radius for circular judge area

    private List<RRNoteData> noteList = new List<RRNoteData>();
    private int nextIndex = 0;
    private bool spawning = false;
    private float computedSpawnY = 600f;

    public void StartNotes(List<RRNoteData> notes)
    {
        noteList = notes;
        nextIndex = 0;
        spawning = true;

        ComputeSpawnY();
    }

    void ComputeSpawnY()
    {
        if (playArea != null)
        {
            // PlayArea rect height / 2 is center in anchored coordinates.
            // top y = rect.height/2. We add spawnYOffset so spawn above visible top.
            computedSpawnY = playArea.rect.height * 0.5f + spawnYOffset;
        }
    }

    void Update()
    {
        if (!spawning || noteList == null || noteList.Count == 0) return;
        if (judgeManager == null) return;

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
            Debug.LogWarning("NoteManager: prefab not assigned for " + data.type + "/" + data.lane);
            return;
        }

        GameObject go = Instantiate(prefab, playArea);
        RectTransform rt = go.GetComponent<RectTransform>();
        if (rt == null) rt = go.AddComponent<RectTransform>();

        float x = 0f;
        if (data.lane == "up") x = laneXOffset;
        else x = -laneXOffset;

        rt.anchoredPosition = new Vector2(x, computedSpawnY);

        RRNote noteComp = go.GetComponent<RRNote>();
        if (noteComp == null) noteComp = go.AddComponent<RRNote>();

        // pass hitLine RectTransform and judgeRadius
        noteComp.Init(data, judgeManager, hitLine, computedSpawnY, travelTime, judgeRadius);
    }
}
