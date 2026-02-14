using System.Collections.Generic;
using UnityEngine;

public class RRGuideNoteManager : MonoBehaviour
{
    [Header("Settings")]
    public GameObject guideNotePrefab;
    public RectTransform guideParent;     // 생성될 부모 (Canvas 내)
    public RectTransform upLanePos;       // Up 노트 생성 위치
    public RectTransform downLanePos;     // Down 노트 생성 위치
    public RectTransform judgeLine;       // 판정선 위치

    public float guideOffsetTime = 0.7f;  // 실제 노트보다 몇 초 먼저 생성할지
    public float fallSpeed = 500f;

    private double songStartTime;
    private List<RRNoteData> guideNotes = new List<RRNoteData>();
    private int nextIndex = 0;

    void Start()
    {
        // GameManager에서 Init()로 초기화되므로 비워둠
    }

    public void Init(RRGameManager gm, List<RRNoteData> notes)
    {
        guideNotes.Clear();
        foreach (var n in notes)
        {
            // 플레이어가 직접 누르는 노트만 가이드 노트 생성
            if (n.type == "tap" || n.type == "long")
            {
                var copy = new RRNoteData()
                {
                    time = n.time - guideOffsetTime,
                    lane = n.lane,
                    type = n.type
                };
                guideNotes.Add(copy);
            }
        }

        guideNotes.Sort((a, b) => a.time.CompareTo(b.time));

        nextIndex = 0;
        songStartTime = AudioSettings.dspTime;
    }

    void Update()
    {
        double currentTime = AudioSettings.dspTime - songStartTime;

        // 생성 조건: 현재 시간 >= guideNote.time
        while (nextIndex < guideNotes.Count && currentTime >= guideNotes[nextIndex].time)
        {
            SpawnGuideNote(guideNotes[nextIndex]);
            nextIndex++;
        }
    }

    void SpawnGuideNote(RRNoteData data)
    {
        RectTransform lanePos = (data.lane == "up") ? upLanePos : downLanePos;
        if (guideNotePrefab == null || guideParent == null || lanePos == null) return;

        GameObject obj = Instantiate(guideNotePrefab, guideParent);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = lanePos.anchoredPosition + new Vector2(0, 600f);

        RRGuideNote gNote = obj.GetComponent<RRGuideNote>();
        gNote.Init(data.lane, judgeLine, fallSpeed);
    }

    // 특정 라인의 가이드 노트를 제거 (Perfect 시 호출)
    public void HideGuideForLane(string lane)
    {
        RRGuideNote[] notes = guideParent.GetComponentsInChildren<RRGuideNote>();
        foreach (var note in notes)
        {
            if (note.lane == lane)
                note.Hide();
        }
    }
}
