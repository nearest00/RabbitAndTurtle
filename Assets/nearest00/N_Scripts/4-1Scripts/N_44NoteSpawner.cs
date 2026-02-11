using UnityEngine;
using System.Collections.Generic;

public class N_44NoteSpawner : MonoBehaviour
{
    public GameObject notePrefab;
    public RectTransform[] playerReceptors;
    public RectTransform[] opponentReceptors;
    public float noteSpeed = 500f;
    public Sprite[] noteSprites;

    // [추가] 롱노트 몸통으로 사용할 스프라이트를 인스펙터에서 넣어주세요
    public Sprite longBodySprite;

    public float pixelsPerBeat = 600f;
    private List<NoteInfo> remainingNotes;
    private N_44GameManager gameManager;

    public void Initialize(N_44Chart chart)
    {
        gameManager = FindFirstObjectByType<N_44GameManager>();
        remainingNotes = new List<NoteInfo>(chart.noteList);
    }

    void Update()
    {
        if (gameManager == null) return;

        float currentBeat = gameManager.GetBeatTime();
        float spawnThreshold = currentBeat + 4.0f;

        for (int i = remainingNotes.Count - 1; i >= 0; i--)
        {
            if (remainingNotes[i].hitTime <= spawnThreshold)
            {
                SpawnNote(remainingNotes[i]);
                remainingNotes.RemoveAt(i);
            }
        }
    }

    void SpawnNote(NoteInfo info)
    {
        Sprite selectedSprite = null;
        if (noteSprites != null && noteSprites.Length > (int)info.direction)
        {
            selectedSprite = noteSprites[(int)info.direction];
        }

        GameObject go = Instantiate(notePrefab);
        N_44Note note = go.GetComponent<N_44Note>();

        RectTransform targetReceptor = info.isPlayerNote ?
            playerReceptors[(int)info.direction] : opponentReceptors[(int)info.direction];

        // --- [여기서부터 수정] ---

        // 1. 롱노트인지 확인 (duration이 0보다 큰지 체크)
        // 만약 NoteInfo에 duration 변수가 없다면 차트 구조에 맞춰 수정이 필요합니다.
        if (info.duration > 0)
        {
            // 롱노트 전용 셋업 호출 (데이터, 속도, 판정선, 머리이미지, 몸통이미지, 길이박자)
            note.SetupLongNote(info, pixelsPerBeat, targetReceptor, selectedSprite, longBodySprite, info.duration);
        }
        else
        {
            // 일반 노트 셋업 호출
            note.Setup(info, pixelsPerBeat, targetReceptor, selectedSprite);
        }

        // --- [수정 끝] ---

        if (info.isPlayerNote)
        {
            var inputManager = Object.FindFirstObjectByType<N_44InputManager>();
            inputManager.activeNotes[(int)info.direction].Add(note);
        }
    }
}