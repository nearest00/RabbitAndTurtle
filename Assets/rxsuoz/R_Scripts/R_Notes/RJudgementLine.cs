using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

// 판정 라인 클래스
public class RJudgementLine : MonoBehaviour
{
    // 이 판정 라인에 대응하는 입력 키
    public KeyCode inputKey;

    // 이 판정 라인의 타입 (0: 왼쪽, 1: 아래쪽)
    public int noteType;

    // 판정 라인의 Y 위치 (픽셀 단위)
    public float judgementLineY = 0f;

    // 판정 가능 범위 (픽셀 단위)
    public float judgementRange = 100f;

    // 현재 판정 범위 내의 노트 리스트
    private List<RNote> notesInRange = new List<RNote>();

    void Update()
    {
        // 입력 키가 눌렸는지 확인
        if (Input.GetKeyDown(inputKey))
        {
            CheckNearestNote();
        }

        // 판정 범위 내의 노트 업데이트
        UpdateNotesInRange();
    }

    // 판정 범위 내에서 가장 가까운 노트를 찾아 판정하는 메소드
    void CheckNearestNote()
    {
        // 씬의 모든 Note 객체 찾기
        RNote[] allNotes = FindObjectsByType<RNote>(FindObjectsSortMode.None);
        RNote nearestNote = null;
        float minDistance = judgementRange;

        // 같은 타입의 노트 중 가장 가까운 것 찾기
        foreach (RNote note in allNotes)
        {
            if (note.noteType == noteType)
            {
                float noteY = note.GetCurrentPosition().y;
                float distance = Mathf.Abs(noteY - judgementLineY);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestNote = note;
                }
            }
        }

        // 판정 범위 내에 노트가 있으면 판정 처리
        if (nearestNote != null)
        {
            // 타이밍 오차 계산 (픽셀을 시간으로 변환)
            float timingError = (nearestNote.GetCurrentPosition().y - judgementLineY) / noteSpeed;

            // 점수 처리
            RScoreManager.instance.ProcessJudgement(timingError);

            // 노트 삭제
            Destroy(nearestNote.gameObject);
        }
    }

    // 판정 범위 내의 노트 업데이트
    void UpdateNotesInRange()
    {
        RNote[] allNotes = FindObjectsByType<RNote>(FindObjectsSortMode.None);
        notesInRange.Clear();

        foreach (RNote note in allNotes)
        {
            if (note.noteType == noteType)
            {
                float noteY = note.GetCurrentPosition().y;
                float distance = Mathf.Abs(noteY - judgementLineY);

                if (distance < judgementRange)
                {
                    notesInRange.Add(note);
                }
            }
        }
    }

    // 노트 속도 (NoteSpawner에서 설정)
    private float noteSpeed = 500f;

    public void SetNoteSpeed(float speed)
    {
        noteSpeed = speed;
    }
}