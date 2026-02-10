using UnityEngine;
using System.Collections.Generic;

public class N_44NoteSpawner : MonoBehaviour
{
    public GameObject notePrefab;
    public RectTransform[] playerReceptors; // 오른쪽 판정선 위치들
    public RectTransform[] opponentReceptors; // 왼쪽 판정선 위치들
    public float noteSpeed = 500f; // 노트가 올라가는 속도
    public Sprite[] noteSprites;
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

        // 현재 박자(Beat)를 가져옵니다.
        float currentBeat = gameManager.GetBeatTime();

        // 미리 생성할 박자 수 (예: 현재 0박자인데 4박자 뒤에 나올 노트를 미리 스폰)
        float spawnThreshold = currentBeat + 4.0f;

        for (int i = remainingNotes.Count - 1; i >= 0; i--)
        {
            // 여기서 spawnThreshold를 사용하여 에러를 해결합니다.
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

        // 2. 노트 생성
        GameObject go = Instantiate(notePrefab);
        N_44Note note = go.GetComponent<N_44Note>();

        // 3. 타겟 판정선 설정
        RectTransform targetReceptor = info.isPlayerNote ?
            playerReceptors[(int)info.direction] : opponentReceptors[(int)info.direction];

        // 4. Setup 호출 (인자 순서 확인: 데이터, 속도, 판정선, 이미지)
        note.Setup(info, pixelsPerBeat, targetReceptor, selectedSprite);
        if (info.isPlayerNote)
        {
            var inputManager = Object.FindFirstObjectByType<N_44InputManager>();
            inputManager.activeNotes[(int)info.direction].Add(note);
        }
    }
}