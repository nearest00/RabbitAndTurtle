using UnityEngine;
using System;
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
    [Header("Difficulty Lists")]
    public List<RoundPattern> easyRounds = new List<RoundPattern>();
    public List<RoundPattern> normalRounds = new List<RoundPattern>();
    public List<RoundPattern> hardRounds = new List<RoundPattern>();

    [Header("Note Slots")]
    public RectTransform[] noteSlots = new RectTransform[5];

    [SerializeField] private N_222NoteManager noteManager;
    [SerializeField] private N_222JudgeManager judgeManager;
    [SerializeField] private N_222RabbitAnimation rabbitAnimation;

    public static N_222RoundManager Instance;
    public string currentDifficulty = "easy";
    public int currentRoundIndex = -1; // -1로 시작해야 첫 스페이스바에 0번이 나옴

    N_222RabbitAnimation RabbitAnimation;
    void Start()
    {
        // 게임 시작 시 0번 라운드를 바로 보고 싶다면 아래 주석 해제
        // StartRound("easy", 0);
    }
    private void Awake()
    {
        // 싱글톤 패턴: 인스턴스가 없으면 자신을 할당, 이미 있으면 중복 제거
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (rabbitAnimation != null)
                rabbitAnimation.PlayTalking();

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

        // 인덱스 증가 후 범위를 넘지 않게 조절
        currentRoundIndex++;
        if (currentRoundIndex >= targetList.Count) currentRoundIndex = 0;

        SpawnCurrent();
    }

    private void SpawnCurrent()
    {
        List<RoundPattern> targetList = GetList(currentDifficulty);
        RoundPattern pattern = targetList[currentRoundIndex];

        // 판정선 위치: 0번 슬롯 기준으로 초기화
        if (noteSlots.Length > 0 && noteSlots[0] != null)
        {
            judgeManager.ResetJudgeLine(new Vector2(noteSlots[0].anchoredPosition.x - 100f, 92f));
        }

        for (int i = 0; i < pattern.notes.Length; i++)
        {
            if (i >= noteSlots.Length || noteSlots[i] == null) continue;

            // 슬롯의 위치(Vector2)를 그대로 가져옴
            Vector2 slotPos = noteSlots[i].anchoredPosition;

            // 생성 시점에 슬롯 위치를 로그로 찍어 확인 (좌표가 0, 200, 400... 인지 확인용)
            Debug.Log($"<color=white>[Spawn]</color> Slot {i} Position: {slotPos}");

            noteManager.CreateNote(pattern.notes[i], slotPos, currentRoundIndex);
        }
        Debug.Log($"<color=white>[RoundManager]</color> {currentDifficulty} {currentRoundIndex}번 라운드 생성 완료");
    }

    private List<RoundPattern> GetList(string diff)
    {
        if (diff == "easy") return easyRounds;
        if (diff == "normal") return normalRounds;
        if (diff == "hard") return hardRounds;
        return null;
    }
}