using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

public class N_44InputManager : MonoBehaviour
{
    public N_44GameManager gameManager;
    public N_44JudgementManager judgeManager;
    public N_44JudgeEffectManager judgeEffectManager;
	public N_44RabbitAnimation rabbitAnimation;
    // 각 라인별로 생성된 노트들을 관리하는 리스트 (0:Left, 1:Down, 2:Up, 3:Right)
    public List<N_44Note>[] activeNotes = new List<N_44Note>[4];
    private N_444SFXList sfx;
    void Awake() // Start보다 먼저 실행되는 Awake에서 리스트 초기화
    {
        for (int i = 0; i < 4; i++)
        {
            activeNotes[i] = new List<N_44Note>();
        }
        sfx = Object.FindFirstObjectByType<N_444SFXList>();
    }
    void Update()
    {
        // 입력 키 매핑 (펌프/FNF 스타일)
        if (Input.GetKeyDown(KeyCode.LeftArrow)) ProcessInput(0);
        if (Input.GetKeyDown(KeyCode.DownArrow)) ProcessInput(1);
        if (Input.GetKeyDown(KeyCode.UpArrow)) ProcessInput(2);
        if (Input.GetKeyDown(KeyCode.RightArrow)) ProcessInput(3);

        if (Input.GetKeyUp(KeyCode.LeftArrow)) CheckRelease(0);
        if (Input.GetKeyUp(KeyCode.DownArrow)) CheckRelease(1);
        if (Input.GetKeyUp(KeyCode.UpArrow)) CheckRelease(2);
        if (Input.GetKeyUp(KeyCode.RightArrow)) CheckRelease(3);
		if (Input.anyKeyDown)
		{
			string dir = "";
			if (Input.GetKeyDown(KeyCode.UpArrow)) dir = "Up";
			else if (Input.GetKeyDown(KeyCode.DownArrow)) dir = "Down";
			else if (Input.GetKeyDown(KeyCode.LeftArrow)) dir = "Left";
			else if (Input.GetKeyDown(KeyCode.RightArrow)) dir = "Right";

			if (dir != "" && rabbitAnimation != null)
			{
				rabbitAnimation.RabbitMove(dir);
			}
		}
	}
	private void ProcessInput(int direction)
	{
		// 1. 해당 라인에서 플레이어용 노트를 '시간순'으로 가져옴
		var targetNote = activeNotes[direction]
			.Where(n => n.Data.isPlayerNote && !n.IsFinished && !n.IsHolding)
			.OrderBy(n => n.Data.hitTime)
			.FirstOrDefault();

		if (targetNote == null) return; // 칠 노트가 없으면 리턴

		float currentBeat = gameManager.GetBeatTime();
		float beatDiff = targetNote.Data.hitTime - currentBeat;

		// [중요] 너무 빨리 눌렀을 때 (예: 0.5박자보다 더 전)
		// 리듬 게임에서 너무 일찍 누르면 보통 무시하거나 'Bad'를 줍니다.
		if (beatDiff > 0.5f) return;

		// [중요] 이미 한참 지나간 노트를 누르려고 하면 (미스 이펙트 없이 지나가는 주범)
		// 여기서 강제로 Miss 판정을 내고 리스트에서 지워줘야 다음 노트가 밀리지 않습니다.
		if (beatDiff < -0.3f)
		{
			Debug.Log("너무 늦게 눌러서 강제 미스 처리");
			judgeManager.GetJudgement(-1.0f); // 강제 Miss 유도
			RemoveNote(targetNote, direction);
			return;
		}

		// 정상 범위 판정
		N_44JudgementManager.Judge result = judgeManager.GetJudgement(beatDiff);

		if (result != N_44JudgementManager.Judge.None)
		{
			if (targetNote.Data.duration > 0 && result != N_44JudgementManager.Judge.Miss)
			{
				targetNote.StartHolding();
				// 롱노트는 activeNotes에서 바로 지우지 말고 
				// 나중에 CheckRelease나 Note 내부에서 지우게 관리하는 게 안전합니다.
			}
			else
			{
				SoundManager.Instance.PlaySFX(sfx.NoteSound);
				RemoveNote(targetNote, direction);
			}
		}
	}

	// RemoveNote도 더 안전하게 수정
	public void RemoveNote(N_44Note note, int line)
	{
		if (note == null) return;

		if (activeNotes[line].Contains(note))
		{
			activeNotes[line].Remove(note);
		}

		note.IsFinished = true; // 플래그 설정
		if (note.gameObject != null)
		{
			Destroy(note.gameObject);
		}
	}


	void CheckRelease(int direction)
    {
        N_44Note holdNote = Object.FindObjectsByType<N_44Note>(FindObjectsSortMode.None)
            .FirstOrDefault(n => n.IsHolding && (int)n.Data.direction == direction);

        if (holdNote != null)
        {
			holdNote.StopHoldSFX();
			float currentBeat = gameManager.GetBeatTime();
            float endBeat = holdNote.Data.hitTime + holdNote.Data.duration;
            float releaseDiff = Mathf.Abs(currentBeat - endBeat);
			if (currentBeat < endBeat - 0.1f)
			{
				// [추가] 얼마나 채웠는지 Note에게 물어봄 (longNoteTickCount 활용)
				float diff = holdNote.Data.duration - holdNote.GetTickCount(); // GetTickCount()는 public으로 선언 필요

				if (diff <= 0.6f)
				{
					Debug.Log("까비~");
				}
				else
				{
					Debug.Log("롱노트 엔딩 미스(빠름)");
				}

				judgeEffectManager.ShowJudge("miss");
				holdNote.ProcessLongNoteEndMiss();
			}
			else
			{
                Debug.Log("롱노트 엔딩 퍼펙트");
				judgeEffectManager.ShowJudge("perfect");
				RemoveNote(holdNote, direction);
			}
		}
    }
}