using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

// 홀드 노트 클래스
public class RHoldNote : RNote
{
    // 홀드 노트를 유지해야 하는 시간 (초)
    public float holdDuration = 1.0f;

    // 홀드가 시작된 시간
    private float holdStartTime = 0;

    // 현재 홀드 중인지 여부
    private bool isHolding = false;

    // 홀드 시작 메소드 (키를 눌렀을 때 호출)
    public void StartHold()
    {
        isHolding = true;
        holdStartTime = Time.time;
    }

    // 홀드 종료 메소드 (키를 뗐을 때 호출)
    public void EndHold()
    {
        isHolding = false;
        float holdTime = Time.time - holdStartTime;

        // 필요한 시간만큼 홀드했는지 확인
        if (holdTime >= holdDuration)
        {
            // Perfect 판정
            RScoreManager.instance.ProcessJudgement(0);
        }
        else
        {
            // Miss 판정
            RScoreManager.instance.ProcessMiss();
        }
    }

    // 현재 홀드 중인지 반환하는 메소드
    public bool IsHolding()
    {
        return isHolding;
    }

    // 홀드 진행도를 반환하는 메소드 (0.0 ~ 1.0)
    public float GetHoldProgress()
    {
        if (!isHolding) return 0;

        float elapsed = Time.time - holdStartTime;
        return Mathf.Clamp01(elapsed / holdDuration);
    }
}