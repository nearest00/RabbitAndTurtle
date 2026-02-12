using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

public class N_44InputManager : MonoBehaviour
{
    public N_44GameManager gameManager;
    public N_44JudgementManager judgeManager;
    public N_44JudgeEffectManager judgeEffectManager;
    // 각 라인별로 생성된 노트들을 관리하는 리스트 (0:Left, 1:Down, 2:Up, 3:Right)
    public List<N_44Note>[] activeNotes = new List<N_44Note>[4];
    void Awake() // Start보다 먼저 실행되는 Awake에서 리스트 초기화
    {
        for (int i = 0; i < 4; i++)
        {
            activeNotes[i] = new List<N_44Note>();
        }
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
    }

    private void ProcessInput(int direction)
{
    if (activeNotes[direction].Count > 0)
    {
        N_44Note note = activeNotes[direction][0];
        float currentBeat = gameManager.GetBeatTime();
        
        // 1. 박자 차이 계산
        float beatDiff = note.Data.hitTime - currentBeat;

        // 2. JudgementManager를 통해 실제 판정 결과 가져오기
        N_44JudgementManager.Judge result = judgeManager.GetJudgement(beatDiff);

        // 3. Miss가 아닐 때만 처리
        if (result != N_44JudgementManager.Judge.Miss && result != N_44JudgementManager.Judge.None)
        {
            if (note.Data.duration > 0)
            {
                // 롱노트 시작
                note.StartHolding();
                activeNotes[direction].RemoveAt(0);
                // Tip: 롱노트 유지 점수는 Note의 Update에서 처리하는 것이 프나펑 방식입니다.
            }
            else
            {
                // 단타 완료
                activeNotes[direction].RemoveAt(0);
                RemoveNote(note, direction);
            }
        }
    }
}

    void HandleSingleNote(N_44Note note, N_44JudgementManager.Judge result, int line)
    {
        Debug.Log($"Single Note Hit: {result}");
        RemoveNote(note, line);
    }

    void HandleLongNoteHead(N_44Note note, N_44JudgementManager.Judge result, int line)
    {
        if (result == N_44JudgementManager.Judge.Miss || result == N_44JudgementManager.Judge.Bad)
        {
            N_44LifeSlider.Instance.AddValue(-50);
            note.FailLongNote(); // 즉시 미스 처리
        }
        else
        {
            note.StartHolding(); // 홀딩 시작
            Debug.Log($"Long Note Head: {result}");
        }
    }

    void CheckRelease(int direction)
    {
        N_44Note holdNote = Object.FindObjectsByType<N_44Note>(FindObjectsSortMode.None)
            .FirstOrDefault(n => n.IsHolding && (int)n.Data.direction == direction);

        if (holdNote != null)
        {
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
					N_44LifeSlider.Instance.AddValue(-40);
				}
				else
				{
					Debug.Log("롱노트 엔딩 미스(빠름)");
					N_44LifeSlider.Instance.AddValue(-50);
				}

				judgeEffectManager.ShowJudge("miss");
				holdNote.FailLongNote();
			}
			else
			{
                Debug.Log("롱노트 엔딩 퍼펙트");
				judgeEffectManager.ShowJudge("perfect");
				RemoveNote(holdNote, direction);
			}
		}
    }

    public void RemoveNote(N_44Note note, int line)
    {
        // 리스트에서 제거
        if (activeNotes[line].Contains(note))
        {
            activeNotes[line].Remove(note);
        }

        if (note != null && note.gameObject != null)
        {
            Destroy(note.gameObject);
        }
    }
}