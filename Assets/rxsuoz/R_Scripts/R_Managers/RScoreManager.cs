using UnityEngine;

public enum JudgementType
{
    Perfect,  //15ms
    Great,    //35ms
    Good,     //60ms
    Bad,      //90ms
    Miss      //90ms 초과
}

// 점수 관리 클래스 (싱글톤 패턴)
public class RScoreManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static RScoreManager instance;

    // 총 점수
    private int totalScore = 0;

    // 각 판정별 카운트
    private int perfectCount = 0;
    private int greatCount = 0;
    private int goodCount = 0;
    private int badCount = 0;
    private int missCount = 0;

    void Awake()
    {
        // 싱글톤 패턴: 게임 중 하나의 ScoreManager만 존재하도록 보장
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 타이밍 오차를 받아서 판정을 처리하는 메소드
    // timingError: 정확한 시간과의 차이 (초 단위, 음수/양수 가능)
    public void ProcessJudgement(float timingError)
    {
        float absError = Mathf.Abs(timingError);
        JudgementType judgement;
        int score = 0;

        // 타이밍 오차에 따라 판정 결정
        if (absError <= 0.015f)  // 15ms
        {
            judgement = JudgementType.Perfect;
            score = 10;
            perfectCount++;
        }
        else if (absError <= 0.035f)  // 35ms
        {
            judgement = JudgementType.Great;
            score = 7;
            greatCount++;
        }
        else if (absError <= 0.060f)  // 60ms
        {
            judgement = JudgementType.Good;
            score = 4;
            goodCount++;
        }
        else if (absError <= 0.090f)  // 90ms
        {
            judgement = JudgementType.Bad;
            score = 1;
            badCount++;
        }
        else
        {
            judgement = JudgementType.Miss;
            score = -50;
            missCount++;
        }

        // 총 점수에 추가
        totalScore += score;

        // 디버그 로그 (개발 중 확인용)
        Debug.Log($"Judgement: {judgement}, Score: {score}, Total: {totalScore}");
    }

    // Miss 판정 처리 메소드
    public void ProcessMiss()
    {
        totalScore -= 50;
        missCount++;
        Debug.Log($"Miss! Total Score: {totalScore}");
    }

    // 총 점수 반환
    public int GetTotalScore()
    {
        return totalScore;
    }

    // 각 판정별 카운트 반환
    public int GetPerfectCount() { return perfectCount; }
    public int GetGreatCount() { return greatCount; }
    public int GetGoodCount() { return goodCount; }
    public int GetBadCount() { return badCount; }
    public int GetMissCount() { return missCount; }

    // 점수 초기화 (새 게임 시작 시)
    public void ResetScore()
    {
        totalScore = 0;
        perfectCount = 0;
        greatCount = 0;
        goodCount = 0;
        badCount = 0;
        missCount = 0;
    }
}
