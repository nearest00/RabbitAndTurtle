using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
        // 1. 현재 씬에 있는 모든 N_44Note 중, 
        //    '홀딩 중'이면서 '해당 방향'인 노트를 찾습니다.
        N_44Note[] allNotes = Object.FindObjectsByType<N_44Note>(FindObjectsSortMode.None);

        foreach (var note in allNotes)
        {
            if (note.IsHolding && (int)note.Data.direction == direction)
            {
                float currentBeat = gameManager.GetBeatTime();
                float endBeat = note.Data.hitTime + note.Data.duration;

                // 2. 판정: 꼬리 끝이 오기 전(약 0.1박자 여유)에 뗐다면 실패!
                if (currentBeat < endBeat - 0.1f)
                {
                    Debug.Log("Too Early Release! Miss!");

                    // 판정 이펙트 표시
                    if (judgeEffectManager != null) judgeEffectManager.ShowJudge("miss");

                    // 라이프 감소
                    N_44LifeSlider.Instance.AddValue(-50f);

                    // 노트 실패 처리 (파괴)
                    note.FailLongNote();
                }
                // (참고: 끝까지 잘 눌렀다면 Note 본인의 Update에서 알아서 삭제됩니다.)
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

        // 하이러키에서 실제 오브젝트 삭제
        if (note != null && note.gameObject != null)
        {
            Destroy(note.gameObject);
        }
    }
}