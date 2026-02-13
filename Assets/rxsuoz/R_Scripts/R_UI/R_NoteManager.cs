using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class R_NoteManager : MonoBehaviour
{
    [Header("Tap Note Prefabs")]
    public GameObject upTapPrefab;      //  
    public GameObject downTapPrefab;    //  

    [Header("Long Note Prefabs")]
    public GameObject upLongPrefab;     //  
    public GameObject downLongPrefab;   //  

    [Header("Main Layers")]
    public RectTransform playArea;      //   
    public RectTransform hitLine;       // 

    [Header("External References")]
    public RRGameManager judgeManager;    // GameManager  ( )

    [Header("Note Settings")]
    public float travelTime = 1.5f;     //    ()
    public float spawnY = 350f;         //   Y

    //   
    private List<RRNoteData> noteList = new List<RRNoteData>();
    private int nextIndex = 0;
    private bool spawning = false;

    //  GameManager :     
    public void StartNotes(List<RRNoteData> notes)
    {
        noteList = notes;
        nextIndex = 0;
        spawning = true;
    }

    void Update()
    {
        if (!spawning || noteList == null || noteList.Count == 0)
            return;

        double songTime = judgeManager.GetSongTime();

        //        
        while (nextIndex < noteList.Count)
        {
            var note = noteList[nextIndex];
            double spawnTime = note.time - travelTime;

            if (songTime >= spawnTime)
            {
                SpawnNote(note);
                nextIndex++;
            }
            else
            {
                break; //       
            }
        }
    }

    //   
    void SpawnNote(RRNoteData data)
    {
        GameObject prefab = null;

        //     
        if (data.type == "tap")
        {
            prefab = (data.lane == "up") ? upTapPrefab : downTapPrefab;
        }
        else if (data.type == "long")
        {
            prefab = (data.lane == "up") ? upLongPrefab : downLongPrefab;
        }

        if (prefab == null)
        {
            Debug.LogWarning($" NoteManager: {data.type}/{data.lane}   .");
            return;
        }

        //   
        GameObject go = Instantiate(prefab, playArea);
        RectTransform rt = go.GetComponent<RectTransform>();

        // /   X  
        float x = (data.lane == "up") ? -100f : 100f;
        rt.anchoredPosition = new Vector2(x, spawnY);

        // Note  
        RRNote noteComp = go.GetComponent<RRNote>();
        if (noteComp == null)
            noteComp = go.AddComponent<RRNote>();

        noteComp.Init(data, judgeManager, hitLine.anchoredPosition.y, spawnY, travelTime);
    }
}
