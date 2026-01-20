using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class RoundNoteData
{
    public N_222NoteBase.NoteType noteType;
    public KeyCode key;
    public KeyCode key2;
}

[Serializable]
public class RoundPattern
{
    public RoundNoteData[] notes = new RoundNoteData[5];
}

public class N_222RoundManager : MonoBehaviour
{
    [System.Serializable] // 인스펙터에 보이게 함
    public class RoundNoteData
    {
        public N_222NoteBase.NoteType noteType;
        public float beat; // 예시 (박자 정보)

        public string key;
    }
    [Header("Difficulty Lists")]
    public List<RoundPattern> easyRounds = new List<RoundPattern>();
    public List<RoundPattern> normalRounds = new List<RoundPattern>();
    public List<RoundPattern> hardRounds = new List<RoundPattern>();

    [Header("Note Slots (Actual Game)")]
    public RectTransform[] noteSlots = new RectTransform[5]; // 에러 해결: 중복 선언 제거 및 하나로 통합

    [Header("Preview Slots (Decoration)")]
    public RectTransform[] secondNoteSlots = new RectTransform[5];

    [Header("Settings")]
    public Vector2 judgeLineStartPos = new Vector2(-346, 0);

    [Header("Movement Settings")]
    [SerializeField] private float easyBPM = 120f;
    [SerializeField] private float normalBPM = 150f;
    [SerializeField] private float hardBPM = 180f;
    [SerializeField] private float distancePerBeat = 300f; // 한 박자당 이동 거리

    [Header("Preview Layers (Decoration)")]
    public RectTransform previewBodyLayer; // 새로 만든 Preview_Body 연결
    public RectTransform previewElseLayer;

    [SerializeField] private N_222NoteManager noteManager;
    [SerializeField] private N_222JudgeManager judgeManager;
    [SerializeField] private N_222RabbitAnimation rabbitAnimation;

    [Header("Judge Lines")]
    [SerializeField] private N_222JudgeLine mainLine;
    [SerializeField] private N_222PrevJudgeLine previewLine;

    public static N_222RoundManager Instance;
    public string currentDifficulty = "easy";
    public int currentRoundIndex = -1;
    private float currentBPM;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (rabbitAnimation != null) rabbitAnimation.PlayTalking();
            NextRound();
        }
    }

    public void StartRound(string difficulty, int index)
    {
        currentDifficulty = difficulty.ToLower();
        currentRoundIndex = index;
        SpawnCurrent();
    }

    public void NextRound()
    {
        List<RoundPattern> targetList = GetList(currentDifficulty);
        if (targetList == null || targetList.Count == 0) return;

        currentRoundIndex++;
        if (currentRoundIndex >= targetList.Count) currentRoundIndex = 0;

        SpawnCurrent();
    }
    private void ClearPreviewNotes()
    {
        if (previewBodyLayer != null)
            foreach (Transform child in previewBodyLayer) Destroy(child.gameObject);
        if (previewElseLayer != null)
            foreach (Transform child in previewElseLayer) Destroy(child.gameObject);
    }

    private void SpawnCurrent()
    {
        // 1. 난이도 문자열에 따라 BPM 설정
        switch (currentDifficulty.ToLower())
        {
            case "easy": currentBPM = easyBPM; break;
            case "normal": currentBPM = normalBPM; break;
            case "hard": currentBPM = hardBPM; break;
            default: currentBPM = 120f; break;
        }

        // 2. 미리보기(Decoration) 노트 초기화
        ClearPreviewNotes();

        // 3. 두 판정선 초기화 및 속도 설정
        if (mainLine != null && previewLine != null)
        {
            // 위치 리셋 및 정지
            mainLine.ResetPosition(judgeLineStartPos.x);
            previewLine.ResetPosition(judgeLineStartPos.x);

            mainLine.StopMoving();
            previewLine.StopMoving();

            // 속도 계산 및 설정
            mainLine.SetSpeedByBPM(currentBPM, distancePerBeat);
            previewLine.SetSpeed(currentBPM, distancePerBeat);
        }

        // 4. 판정 매니저 리셋 (노트 리스트 등)
        if (judgeManager != null)
        {
            judgeManager.ResetJudgeLine(judgeLineStartPos);
        }

        // 5. 현재 난이도 리스트 및 패턴 가져오기
        List<RoundPattern> targetList = GetList(currentDifficulty);
        if (targetList == null || currentRoundIndex < 0 || currentRoundIndex >= targetList.Count) return;

        RoundPattern pattern = targetList[currentRoundIndex];

        // 6. 노트 생성 루프
        for (int i = 0; i < pattern.notes.Length; i++)
        {
            if (pattern.notes[i] == null) continue;

            // [A] 실제 게임용 노트
            if (i < noteSlots.Length && noteSlots[i] != null)
            {
                noteManager.CreateNote(pattern.notes[i], noteSlots[i].anchoredPosition, currentRoundIndex, false);
            }

            // [B] 미리보기용(Decoration) 노트
            if (i < secondNoteSlots.Length && secondNoteSlots[i] != null)
            {
                noteManager.CreateNote(pattern.notes[i], secondNoteSlots[i].anchoredPosition, currentRoundIndex, true);
            }
        }

        // 7. [핵심] 미리보기 시퀀스 코루틴 시작
        StopAllCoroutines(); // 이전 실행중인 시퀀스 방지
        StartCoroutine(LineSequenceRoutine());

        Debug.Log($"<color=yellow>[RoundManager]</color> 라운드 {currentRoundIndex} 스폰 완료 (BPM: {currentBPM})");
    }

    private List<RoundPattern> GetList(string diff)
    {
        if (diff == "easy") return easyRounds;
        if (diff == "normal") return normalRounds;
        if (diff == "hard") return hardRounds;
        return null;
    }
    private IEnumerator LineSequenceRoutine()
    {
        // [Step 1] 미리보기 판정선만 출발
        previewLine.StartMoving();

        // [Step 2] 6박자 동안 대기
        // 계산식: 1박자 시간(60/BPM) * 6박자
        float waitTime = (60f / currentBPM) * 6f;
        yield return new WaitForSeconds(waitTime);

        // [Step 3] 미리보기 판정선 멈춤
        previewLine.StopMoving();

        // [Step 4] 메인 판정선 출발
        mainLine.StartMoving();

        Debug.Log("<color=orange>미리보기 종료, 메인 판정 시작!</color>");
    }
}