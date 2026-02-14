using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public enum NoteDir { Left, Right, Up, Down, None } // 우리가 쓸 방향 정의

[Serializable]
public class RoundNoteData
{
    public N_222NoteBase.NoteType noteType;
    public float beat;

    // 인스펙터에서는 이 Enum이 드롭다운으로 뜹니다.
    public NoteDir keyDir = NoteDir.Left;
    public NoteDir keyDir2 = NoteDir.None;

    // 기존 시스템(string)과의 호환을 위한 프로퍼티
    public string key => keyDir.ToString();
    public string key2 => keyDir2.ToString();
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

    [Header("Note Slots (Actual Game)")]
    public RectTransform[] noteSlots = new RectTransform[5];

    [Header("Preview Slots (Decoration)")]
    public RectTransform[] secondNoteSlots = new RectTransform[5];

    [Header("Settings")]
    public Vector2 judgeLineStartPos = new Vector2(-346, 0);

    [Header("Movement Settings")]
    [SerializeField] private float easyBPM = 120f;
    [SerializeField] private float normalBPM = 150f;
    [SerializeField] private float hardBPM = 180f;
    [SerializeField] private float distancePerBeat = 300f;

    [Header("Preview Layers (Decoration)")]
    public RectTransform previewBodyLayer;
    public RectTransform previewElseLayer;

    [SerializeField] private N_222NoteManager noteManager;
    [SerializeField] private N_222JudgeManager judgeManager;
    [SerializeField] private N_222RabbitAnimation rabbitAnimation;
    [SerializeField] private N_222KingAnimation kingAnimation;
    [Header("Judge Lines")]
    [SerializeField] private N_222JudgeLine mainLine;
    [SerializeField] private N_222PrevJudgeLine previewLine;
    [SerializeField] private N_222LifeSlider lifeslider;
    [SerializeField] private Ending ending;

	[Header("Speed Up Settings")]
	[SerializeField] private float speedWeight = 30f;
	[SerializeField] private float judgeRangeMultiplier = 0.9f;
	public string currentDifficulty
    {
        get => N_StageSellectButton.Instance.StageDifficulty;
        set => N_StageSellectButton.Instance.StageDifficulty = value;
    }
    public float Max
    {
        get => (lifeslider != null) ? lifeslider.Max : 0;
        set { if (lifeslider != null) lifeslider.Max = value; }
    }
	public bool tutorialing
	{
		get => GuidePanelOff.Instance.tutorialing;
		set => GuidePanelOff.Instance.tutorialing = value;
	}
	public bool isCountingDown
	{
		get => PauseCountDown.Instance.isCounting;
		set => PauseCountDown.Instance.isCounting = value;
	}
	public bool CanSettingOn
	{
		get => SettingPanel.Instance.CanSettingOn;
		set => SettingPanel.Instance.CanSettingOn = value;
	}
	public static N_222RoundManager Instance;
    public int currentRoundIndex = -1;
    private float currentBPM;
    public int MaxLife;
	private float timer = 0f;
	private bool isTimerRunning = false;
	private bool isPreviewFinished = false;
	private bool lastCountingState = true;
	private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
		isTimerRunning = false;
		isPreviewFinished = false;
	}
	private int GetSpeedStepInterval()
	{
		switch (currentDifficulty.ToLower())
		{
			case "easy": return 5;
			case "normal": return 4;
			case "hard": return 3;
			default: return 5;
		}
	}

    void Update()
	{
		// 1. 카운트다운 체크 (true였다가 false가 되는 순간 감지)
		if (lastCountingState == true && isCountingDown == false)
		{
			Debug.Log("<color=cyan>카운트다운 종료! 첫 라운드를 시작합니다.</color>");
			StartRound(currentDifficulty, 0); // 여기서 첫 라운드 시작
		}
		lastCountingState = isCountingDown; // 현재 상태를 저장해서 다음 프레임에서 비교

		// 2. 기존 코루틴 종료 후 6박 대기 로직
		if (isPreviewFinished && !isTimerRunning)
		{
			timer = (60f / currentBPM) * 6f;
			isTimerRunning = true;
			isPreviewFinished = false;
		}

		if (isTimerRunning)
		{
			timer -= Time.deltaTime;
			if (timer <= 0)
			{
				if (tutorialing) return;
				if (isCountingDown) return; // 카운트다운 중에는 실행 방지
				if (!CanSettingOn) return;
				if (rabbitAnimation != null) rabbitAnimation.StopTalking();

				NextRound();
				Debug.Log("6박자가 지났습니다! 다음 라운드 실행.");
				isTimerRunning = false;
			}
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
        if (currentRoundIndex == targetList.Count)
        {
            Time.timeScale = 0f;
            
            Debug.Log("분모"+ lifeslider.Max);
            if (lifeslider.targetSlider.value / lifeslider.Max >= 0.6)
            {
                ending.StageClear();
            }
            else ending.StageFailed();
            return;
        }

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
		int speedStep = (currentRoundIndex + 1) / GetSpeedStepInterval();
		currentBPM = currentBPM + (speedStep * speedWeight);
		float currentJudgeScale = Mathf.Pow(judgeRangeMultiplier, speedStep);
		Debug.Log($"<color=cyan>[Round {currentRoundIndex + 1}]</color> 적용 BPM: {currentBPM}, 판정배율: {currentJudgeScale}");
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
			judgeManager.AdjustJudgeRange(currentJudgeScale);
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
        kingAnimation.PlayTalking();
        previewLine.StartMoving();

        // [Step 2] 6박자 동안 대기
        // 계산식: 1박자 시간(60/BPM) * 6박자
        float waitTime = (60f / currentBPM) * 6f;
        yield return new WaitForSeconds(waitTime);

        // [Step 3] 미리보기 판정선 멈춤
        kingAnimation.StopTalking();
        previewLine.StopMoving();

        // [Step 4] 메인 판정선 출발
        mainLine.StartMoving();
        rabbitAnimation.PlayTalking();
		isPreviewFinished = true;
		Debug.Log("<color=orange>미리보기 종료, 메인 판정 시작!</color>");
    }
}