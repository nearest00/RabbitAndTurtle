using UnityEngine;
using UnityEngine.UI;

public class N_222LongNote : N_222NoteBase
{
    public override void OnPerfect()
    {
        Debug.Log($"<color=lime>[Perfect]</color> {noteType} (ID: {roundID})");

        // 1. 점수 처리 (헤드와 꼬리뿐만 아니라 홀드(몸통)도 퍼펙트 점수를 줌)
        N_222LifeSlider.Instance.AddValue(10f);

        // 2. 공통 상태 관리 로직 실행
        HandleNoteStatus();
    }

    public override void OnGreat()
    {
        Debug.Log($"<color=yellow>[Great</color> {noteType} (ID: {roundID})");
        N_222LifeSlider.Instance.AddValue(7f);
        HandleNoteStatus();
    }
    public override void OnGood()
    {
        Debug.Log($"<color=yellow>[Good]</color> {noteType} (ID: {roundID})");
        N_222LifeSlider.Instance.AddValue(4f);
        HandleNoteStatus();
    }
    public override void OnBad()
    {
        Debug.Log($"<color=yellow>[Bad]</color> {noteType} (ID: {roundID})");
        N_222LifeSlider.Instance.AddValue(1f);
        HandleNoteStatus();
    }

    public override void OnMiss()
    {
        Debug.Log($"<color=red>[Miss]</color> {noteType} (ID: {roundID})");
        isJudged = true;
        IsFinished = true;
        gameObject.SetActive(false);
        N_222LifeSlider.Instance.AddValue(-50f);

    }
    private void HandleNoteStatus()
    {
        if (noteType == NoteType.LongStart || noteType == NoteType.LongHold)
        {
            isJudged = true;
            IsFinished = false; // 계속 유지
        }
        else if (noteType == NoteType.LongEnd)
        {
            isJudged = true;
            IsFinished = true;
            gameObject.SetActive(false); // 꼬리가 처리되면 비활성화
        }
    }
}