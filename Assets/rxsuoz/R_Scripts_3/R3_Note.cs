using UnityEngine;

public class R3_Note : MonoBehaviour
{
    public R3_NoteData Data { get; private set; }
    private Vector2 _startPos;
    private float _travelTime;
    private bool _isJudged = false;
    public bool isBeingHeld = false; // InputHandler에서 제어

    const double PERFECT = 0.035;
    const double BAD = 0.160;

    public void Initialize(R3_NoteData data, Vector2 startPos, float travelTime)
    {
        Data = data;
        _startPos = startPos;
        _travelTime = travelTime;
        GetComponent<RectTransform>().anchoredPosition = startPos;
    }

    void Update()
    {
        if (Time.timeScale <= 0) return;

        double currentTime = R3_GameManager.Instance.GetCurrentTime();

        // 이동 처리
        if (!_isJudged || (Data.noteType == "long" && isBeingHeld))
        {
            float progress = 1f - (float)((Data.time - currentTime) / _travelTime);
            GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(_startPos, Vector2.zero, progress);
        }

        // 롱노트 유지 완료 체크
        if (Data.noteType == "long" && isBeingHeld)
        {
            if (currentTime >= Data.time + Data.holdDuration)
            {
                ApplyJudgment("Perfect");
            }
        }

        // Miss 체크
        if (!_isJudged && currentTime > Data.time + BAD)
        {
            ApplyJudgment("Miss");
        }
    }

    public void OnHit(double hitTime)
    {
        if (_isJudged) return;

        double diff = System.Math.Abs(hitTime - Data.time);
        string res = diff <= PERFECT ? "Perfect" : (diff <= 0.07 ? "Great" : (diff <= 0.11 ? "Good" : (diff <= 0.16 ? "Bad" : "Miss")));

        if (Data.noteType == "long" && res != "Miss")
        {
            isBeingHeld = true; // 롱노트 시작
            // 롱노트 헤드 제거 혹은 투명화 연출 가능
        }
        else
        {
            ApplyJudgment(res);
        }
    }

    public void OnRelease(double releaseTime)
    {
        if (Data.noteType == "long" && isBeingHeld)
        {
            isBeingHeld = false;
            if (releaseTime < Data.time + Data.holdDuration * 0.9) // 90% 이상 유지 못하면 실패
            {
                ApplyJudgment("Miss");
            }
        }
    }

    private void ApplyJudgment(string label)
    {
        _isJudged = true;
        R3_GameManager.Instance.CreateJudgePopup(label);
        Destroy(gameObject, 0.05f);
    }
}