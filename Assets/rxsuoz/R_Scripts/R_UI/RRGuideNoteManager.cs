using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RRGuideNoteManager : MonoBehaviour
{
    [Header("Tap Note Prefabs")]
    public GameObject upGuidePrefab;
    public GameObject downGuidePrefab;

    [Header("Long Note Prefabs")]
    public GameObject upLongPrefab;
    public GameObject downLongPrefab;

    [Header("Main UI")]
    public RectTransform playArea;
    public RectTransform hitLine;

    [Header("Timing")]
    public float travelTime = 1.8f;

    private List<RRNoteData> guideNotes = new List<RRNoteData>();
    private int nextIndex = 0;
    private bool active = false;
    private RRGameManager gm;

    public void Init(RRGameManager manager, List<RRNoteData> allNotes)
    {
        gm = manager;
        guideNotes.Clear();
        nextIndex = 0;

        // CSV에서 guide 타입만 필터링
        foreach (var n in allNotes)
        {
            if (n.type == "guide")
                guideNotes.Add(n);
        }

        active = guideNotes.Count > 0;
    }

    void Update()
    {
        if (!active || gm == null || guideNotes.Count == 0) return;

        double songTime = gm.GetSongTime();

        // spawn 시점 계산
        while (nextIndex < guideNotes.Count)
        {
            var n = guideNotes[nextIndex];
            double spawnTime = n.time - travelTime;
            if (songTime >= spawnTime)
            {
                SpawnGuideNote(n);
                nextIndex++;
            }
            else break;
        }
    }

    void SpawnGuideNote(RRNoteData data)
    {
        if (data == null) return;

        GameObject prefab = null;

        if (data.lane == "up") prefab = upGuidePrefab;
        else if (data.lane == "down") prefab = downGuidePrefab;

        if (prefab == null)
        {
            Debug.LogWarning("GuideNoteManager: prefab missing for " + data.lane);
            return;
        }

        GameObject go = Instantiate(prefab, playArea);
        RectTransform rt = go.GetComponent<RectTransform>();
        if (rt == null) rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0f, 600f);

        // 단순하게 위→아래 이동 (입력 반응 없음)
        RRGuideNote gNote = go.AddComponent<RRGuideNote>();
        gNote.Init(data, hitLine, travelTime);
    }
}
