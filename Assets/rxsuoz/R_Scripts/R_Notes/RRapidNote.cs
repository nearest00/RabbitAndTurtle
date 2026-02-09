using UnityEditor.Experimental.GraphView;
using UnityEngine;

// 연타 노트 클래스
public class RRapidNote : RNote
{
    // 연타를 입력해야 하는 시간 (초)
    public float duration = 1.5f;

    // 필요한 탭 개수
    public int requiredTaps = 10;

    // 현재까지 입력된 탭 개수
    private int currentTapCount = 0;

    // 연타가 시작된 시간
    private float startTime = 0;

    // 연타가 활성화되어 있는지 여부
    private bool isActive = false;

    // 연타 시작 메소드
    public void StartRapid()
    {
        isActive = true;
        currentTapCount = 0;
        startTime = Time.time;
    }

    // 탭 입력을 등록하는 메소드
    // 반환값: Perfect 달성 여부
    public bool RegisterTap()
    {
        if (!isActive) return false;

        // 경과 시간 계산
        float elapsedTime = Time.time - startTime;

        // 시간 초과 확인
        if (elapsedTime > duration)
        {
            FinishRapid();
            return false;
        }

        // 탭 개수 증가
        currentTapCount++;

        // 필요한 탭 개수에 도달했는지 확인
        if (currentTapCount >= requiredTaps)
        {
            FinishRapid();
            return true;  // Perfect 달성
        }

        return false;  // 아직 진행 중
    }

    // 연타 종료 메소드
    public void FinishRapid()
    {
        isActive = false;
    }

    // 현재 탭 개수 반환
    public int GetCurrentTapCount()
    {
        return currentTapCount;
    }

    // 연타가 활성화되어 있는지 반환
    public bool IsActive()
    {
        return isActive;
    }
}