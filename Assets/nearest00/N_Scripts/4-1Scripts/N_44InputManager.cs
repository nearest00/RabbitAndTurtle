using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class N_44InputManager : MonoBehaviour
{
    public N_44GameManager gameManager;
    public N_44JudgementManager judgeManager;

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

        // 롱노트 떼기 판정 (Hold 체크)
        if (Input.GetKeyUp(KeyCode.LeftArrow)) CheckRelease(0);
        // ... 다른 키도 동일하게 처리
    }

    void ProcessInput(int line)
    {
        if (activeNotes[line].Count == 0) return;

        N_44Note targetNote = activeNotes[line][0];
        float currentTime = gameManager.GetMusicTime();
        float diff = targetNote.Data.hitTime - currentTime;

        var result = judgeManager.GetJudgement(diff);

        if (targetNote.Data.type == NoteType.Single)
        {
            HandleSingleNote(targetNote, result, line);
        }
        else
        {
            HandleLongNoteHead(targetNote, result, line);
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

    void CheckRelease(int line)
    {
        // 롱노트 도중 떼었을 때 미스 처리 로직
    }

    public void RemoveNote(N_44Note note, int line)
    {
        // 리스트에서 제거
        if (activeNotes[line].Contains(note))
        {
            activeNotes[line].Remove(note);
        }

        // 하이러키에서 실제 오브젝트 삭제
        if (note != null && note.gameObject != null)
        {
            Destroy(note.gameObject);
        }
    }
}