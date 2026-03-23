using UnityEngine;
using System.Collections.Generic;

public class N_44NoteSpawner : MonoBehaviour
{
	public GameObject notePrefab;
	public RectTransform[] playerReceptors;
	public RectTransform[] opponentReceptors;
	public float noteSpeed = 500f;
	public Sprite[] noteSprites;

	[Header("Long Note Settings")]
	public Sprite longBodySprite;

	[Header("Containers")]
	public Transform playerNoteContainer;
	public Transform opponentNoteContainer;

	public float pixelsPerBeat = 600f;
	private int totalNoteCount = 0;
	private int spawnedNoteCount = 0;
	private float lastNoteEndTime = 0f;
	private bool allNotesSpawned = false;
	private bool gameEnded = false;

	// GameManager의 NoteInfo 클래스를 사용하도록 수정
	private List<N_44GameManager.NoteInfo> remainingNotes;
	private N_44GameManager gameManager;
	public Ending Ending;
	public EndingIlustScript EndingIlustScript;

	// [수정] 차트 에셋 대신 리스트와 BPM을 직접 받는 방식으로 변경
	public void Initialize(List<N_44GameManager.NoteInfo> generatedNotes, float bpm)
	{
		gameManager = FindFirstObjectByType<N_44GameManager>();

		if (generatedNotes == null || generatedNotes.Count == 0)
		{
			Debug.LogWarning("전달받은 노트 데이터가 비어있습니다.");
			return;
		}

		// 전달받은 데이터를 복사하여 리스트 생성
		remainingNotes = new List<N_44GameManager.NoteInfo>(generatedNotes);

		totalNoteCount = remainingNotes.Count;
		spawnedNoteCount = 0;
		allNotesSpawned = false;
		gameEnded = false;

		// 마지막 노트의 종료 박자 계산 (게임 종료 체크용)
		N_44GameManager.NoteInfo lastNote = generatedNotes[generatedNotes.Count - 1];
		lastNoteEndTime = lastNote.hitTime + lastNote.duration;

		Debug.Log($"스포너 초기화 완료: 총 {totalNoteCount}개 노트 생성 예정");
	}

	void Update()
	{
		if (gameManager == null || remainingNotes == null) return;

		float currentBeat = gameManager.GetBeatTime();
		// 4박자 앞에 미리 소환 (노트가 내려오는 시간 확보)
		float spawnThreshold = currentBeat + 4.0f;

		// 리스트 뒤에서부터 검사하여 성능 최적화 및 제거 시 인덱스 꼬임 방지
		for (int i = remainingNotes.Count - 1; i >= 0; i--)
		{
			if (remainingNotes[i].hitTime <= spawnThreshold)
			{
				SpawnNote(remainingNotes[i]);
				remainingNotes.RemoveAt(i);
				spawnedNoteCount++;
			}
		}

		// 모든 노트 소환 완료 체크
		if (!allNotesSpawned && spawnedNoteCount >= totalNoteCount && totalNoteCount > 0)
		{
			allNotesSpawned = true;
			Debug.Log("<color=cyan>[Spawner]</color> 모든 노트 스폰 완료");
		}

		// 종료 체크 로직 (마지막 노트 종료 후 8박자 여유)
		if (allNotesSpawned && !gameEnded)
		{
			if (currentBeat >= lastNoteEndTime + 8f)
			{
				gameEnded = true;
				Debug.Log("인터널밸류: "+ N_44LifeSlider.Instance.internalValue);
				Debug.Log("맥스: " + N_44LifeSlider.Instance.Max);
				OnGameComplete();
			}
		}
	}

	void OnGameComplete()
	{
		if (N_44LifeSlider.Instance.internalValue / N_44LifeSlider.Instance.Max >= 0.6)
		{
			Ending.StageClear();
			EndingIlustScript.ShowEnding();
		}
		else Ending.StageFailed();
	}

	// [수정] N_44GameManager.NoteInfo 타입을 사용하도록 변경
	void SpawnNote(N_44GameManager.NoteInfo info)
	{
		Sprite selectedSprite = null;
		if (noteSprites != null && noteSprites.Length > (int)info.direction)
		{
			selectedSprite = noteSprites[(int)info.direction];
		}
		Transform parentContainer = info.isPlayerNote ? playerNoteContainer : opponentNoteContainer;
		GameObject go = Instantiate(notePrefab, parentContainer); N_44Note note = go.GetComponent<N_44Note>();

		// 방향(NoteDirection)을 int로 캐스팅하여 인덱스로 사용
		int dirIndex = (int)info.direction;
		RectTransform targetReceptor = info.isPlayerNote ?
			playerReceptors[dirIndex] : opponentReceptors[dirIndex];

		if (info.type == N_44GameManager.NoteType.Long)
		{
			// 롱노트 설정
			note.SetupLongNote(info, pixelsPerBeat, targetReceptor, selectedSprite, longBodySprite, info.duration);
		}
		else
		{
			// 단타 설정
			note.Setup(info, pixelsPerBeat, targetReceptor, selectedSprite);
		}

		if (info.isPlayerNote)
		{
			var inputManager = Object.FindFirstObjectByType<N_44InputManager>();
			if (inputManager != null)
			{
				inputManager.activeNotes[dirIndex].Add(note);
			}
		}
	}
}