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
    private int totalNoteCount = 0;       // 전체 노트 개수
    private int spawnedNoteCount = 0;    // 지금까지 소환한 노트 개수
    private float lastNoteEndTime = 0f;
    private bool allNotesSpawned = false;
    private bool gameEnded = false;
    private List<NoteInfo> remainingNotes;
    private N_44GameManager gameManager;
    public Ending Ending;

    public void Initialize(N_44Chart chart)
    {
        gameManager = FindFirstObjectByType<N_44GameManager>();
        if (chart == null || chart.noteList.Count == 0) return;

        remainingNotes = new List<NoteInfo>(chart.noteList);

        // [수정] 전체 개수를 미리 저장
        totalNoteCount = remainingNotes.Count;
        spawnedNoteCount = 0;
        allNotesSpawned = false;
        gameEnded = false;

        // 마지막 노트 종료 시간 저장
        NoteInfo lastNote = chart.noteList[chart.noteList.Count - 1];
        lastNoteEndTime = lastNote.hitTime + lastNote.duration;
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
                spawnedNoteCount++;
            }
        }

        // 2. 모든 노트 소환 완료 체크
        if (!allNotesSpawned && spawnedNoteCount >= totalNoteCount && totalNoteCount > 0)
        {
            allNotesSpawned = true;
            Debug.Log("스폰끝");
        }

        // 3. 종료 체크 로직 (8박자 후)
        if (allNotesSpawned && !gameEnded)
        {
            // 꼬리가 끝난 박자(lastNoteEndTime)로부터 8박자 체크
            if (currentBeat >= lastNoteEndTime + 8f)
            {
                gameEnded = true;
                OnGameComplete();
            }
        }
    }
    void OnGameComplete()
    {
        if (N_44LifeSlider.Instance.internalValue / N_44LifeSlider.Instance.Max >= 0.6) Ending.StageClear();
        else Ending.StageFailed();
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

        if (info.duration > 0)
        {
            note.SetupLongNote(info, pixelsPerBeat, targetReceptor, selectedSprite, longBodySprite, info.duration);
        }
        else
        {
            note.Setup(info, pixelsPerBeat, targetReceptor, selectedSprite);
        }


        if (info.isPlayerNote)
        {
            var inputManager = Object.FindFirstObjectByType<N_44InputManager>();
            inputManager.activeNotes[(int)info.direction].Add(note);
        }
    }
}