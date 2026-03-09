using UnityEngine;
using System.Collections.Generic;

public class N_44GameManager : MonoBehaviour
{
	public enum NoteType { Single, Long }
	public enum NoteDirection { Left, Down, Up, Right }

	[System.Serializable]
	public class NoteInfo
	{
		public float hitTime;           // 노트를 쳐야 하는 시간(박자)
		public NoteType type;           // 단타 vs 롱노트
		public NoteDirection direction; // 방향 (0~3)
		public float duration;          // 롱노트일 경우의 길이
		public bool isPlayerNote;       // 플레이어용인지 가이드용인지 구분
	}
	[Header("Animators")]
	public Animator playerAnimator; // 플레이어 캐릭터용
	public Animator guideAnimator;  // 가이드 캐릭터용

	[Header("Game Settings")]
	public string currentDifficulty
	{
		get => N_StageSellectButton.Instance.StageDifficulty;
		set => N_StageSellectButton.Instance.StageDifficulty = value;
	}

	public float bpm;
	private float startTime;
	public float Max;

	[Header("Difficulty Scale (Guide Note Count)")]
	public int easyNoteGoal = 80;
	public int normalNoteGoal = 120;
	public int hardNoteGoal = 200;

	[Header("References")]
	public N_44NoteSpawner noteSpawner;

	private List<NoteInfo> generatedNoteList = new List<NoteInfo>();
	private float[] lineBusyUntil = new float[4];

	void Start()
	{
		StartGame();
	}

	void StartGame()
	{
		generatedNoteList.Clear();
		for (int i = 0; i < 4; i++) lineBusyUntil[i] = 0;

		string difficulty = currentDifficulty.ToLower();
		int targetGoal = normalNoteGoal; // 기본값

		// 1. 난이도별 설정값 할당
		if (difficulty == "easy")
		{
			bpm = 100;
			targetGoal = easyNoteGoal;
		}
		else if (difficulty == "normal")
		{
			bpm = 60;
			targetGoal = normalNoteGoal;
		}
		else if (difficulty == "hard")
		{
			bpm = 140;
			targetGoal = hardNoteGoal;
		}

		// 2. 라이프 슬라이더 설정
		if (N_44LifeSlider.Instance != null)
		{
			N_44LifeSlider.Instance.Max = Max;
			N_44LifeSlider.Instance.targetSlider.maxValue = Max;
		}

		// 3. 실시간 차트 생성 (목표 개수 기반)
		GenerateRandomChart(targetGoal);

		// 4. 게임 시작 시간 기록 및 스포너 초기화
		this.startTime = Time.time;

		if (noteSpawner != null)
		{
			noteSpawner.Initialize(generatedNoteList, bpm);
		}

		Debug.Log($"게임 시작! 난이도: {currentDifficulty}, 목표 노트(가이드): {targetGoal}, BPM: {bpm}");
	}

	private void GenerateRandomChart(int targetGoal)
	{
		generatedNoteList.Clear();
		// 라인별 점유 시간 초기화
		for (int i = 0; i < 4; i++) lineBusyUntil[i] = 0;

		int currentGuideNoteCount = 0;
		int section = 0;
		float totalDurationSum = 0f;

		// 가이드 노트가 목표 개수에 도달할 때까지 섹션 반복
		while (currentGuideNoteCount < targetGoal)
		{
			// 섹션 시작 박자 (5, 13, 21...)
			float guideStartBeat = (section * 8) + 5;

			// 한 섹션(4박자) 내에서 1박자씩 이동하며 검사
			for (float beat = guideStartBeat; beat < guideStartBeat + 4; beat += 1f)
			{
				if (currentGuideNoteCount >= targetGoal) break;

				// 현재 박자가 섹션 내에서 몇 번째인지 (0, 1, 2, 3)
				int beatStep = Mathf.RoundToInt(beat - guideStartBeat);
				// 가이드 종료까지 남은 박자 (침범 방지용)
				int maxAllowedDuration = 4 - beatStep;

				// [추가] 현재 박자에서 연주 중인 라인과 생성 가능한 라인 구분
				List<int> availableLines = new List<int>();
				int activeLongNoteCount = 0; // 현재 롱노트 유지 중인 라인 수

				for (int i = 0; i < 4; i++)
				{
					// lineBusyUntil이 현재 beat보다 크면 해당 라인은 '연주 중' 혹은 '휴식 중'
					// 단, 단순 휴식(beat + 1) 상태도 '밀도'에 포함시켜 손가락 꼬임을 방지함
					if (lineBusyUntil[i] <= beat)
					{
						availableLines.Add(i);
					}
					else
					{
						activeLongNoteCount++;
					}
				}

				// [핵심] 이미 2개 이상의 라인이 사용 중이면 이번 박자에서는 생성 패스
				if (activeLongNoteCount >= 2 || availableLines.Count == 0) continue;

				// 새로 생성할 수 있는 최대 개수 (전체 2개 - 현재 연주 중인 개수)
				int maxSpawnCount = 2 - activeLongNoteCount;
				int remainingToGoal = targetGoal - currentGuideNoteCount;

				// 실제로 생성할 노트 개수 결정 (최소 1개 ~ maxSpawnCount)
				int noteCount = Random.Range(1, Mathf.Min(maxSpawnCount, remainingToGoal) + 1);

				// 라인 셔플
				for (int i = 0; i < availableLines.Count; i++)
				{
					int temp = availableLines[i];
					int rand = Random.Range(i, availableLines.Count);
					availableLines[i] = availableLines[rand];
					availableLines[rand] = temp;
				}

				for (int i = 0; i < noteCount; i++)
				{
					int lineIndex = availableLines[i];
					bool isLong = Random.value > 0.5f;
					float duration = 0f;

					if (isLong)
					{
						// 롱노트 길이가 남은 가이드 시간을 넘지 않도록 제한 (최소 1박자 필요)
						if (maxAllowedDuration >= 1)
						{
							int randLimit = Mathf.Min(4, maxAllowedDuration + 1);
							duration = (float)Random.Range(1, randLimit);
						}
						else
						{
							isLong = false;
							duration = 0f;
						}
					}

					if (isLong) totalDurationSum += duration;

					// 데이터 리스트에 추가 (가이드)
					generatedNoteList.Add(new NoteInfo
					{
						hitTime = beat,
						type = isLong ? NoteType.Long : NoteType.Single,
						direction = (NoteDirection)lineIndex,
						duration = duration,
						isPlayerNote = false
					});

					// 데이터 리스트에 추가 (플레이어 - 4박 뒤)
					generatedNoteList.Add(new NoteInfo
					{
						hitTime = beat + 4f,
						type = isLong ? NoteType.Long : NoteType.Single,
						direction = (NoteDirection)lineIndex,
						duration = duration,
						isPlayerNote = true
					});

					// 다음 노트 생성 가능 시점 (노트 끝 + 1박자 강제 휴식)
					lineBusyUntil[lineIndex] = beat + duration + 1f;
					currentGuideNoteCount++;
				}
			}
			section++; // 8박자 뒤 다음 섹션으로
		}

		// 최종 Max 라이프 계산 (모든 노트 개수 * 10 + 롱노트 전체 지속시간 * 10)
		this.Max = (currentGuideNoteCount * 10f) + (totalDurationSum * 10f);

		if (N_44LifeSlider.Instance != null)
		{
			N_44LifeSlider.Instance.Max = this.Max;
			N_44LifeSlider.Instance.targetSlider.maxValue = this.Max;
		}

		// 시간순 정렬
		generatedNoteList.Sort((a, b) => a.hitTime.CompareTo(b.hitTime));

		Debug.Log($"채보 생성 완료: 가이드 {currentGuideNoteCount}개, Max {this.Max}");
	}
	public float GetBeatTime()
	{
		if (bpm <= 0) return 0;
		float elapsedSeconds = Time.time - startTime;
		return elapsedSeconds * (bpm / 60f);
	}

	public float BeatToSeconds(float beat)
	{
		if (bpm <= 0) return 0;
		return beat * (60f / bpm);
	}

	public float GetMusicTime()
	{
		return Time.time - startTime;
	}
}