using UnityEngine;

public class R3_Note : MonoBehaviour
{
    public R3_NoteData data;
    public float fallSpeed;

    private RectTransform rt;
    private RectTransform hitLineRect;
    private RRGameManager gm;

    private float spawnPosPrimary;
    private double travelTime;
    private float judgeRadius;

    private bool judged = false;
    private bool isHoldActive = false;
    private bool isBeingHeld = false;
    private double holdEndTime = 0.0;

    private RectTransform bodyRect;
    private RectTransform headRect;
    private float initialBodyLength = 0f;

    // 판정 범위(ms)
    const double PERFECT_MS = 15.0;
    const double GREAT_MS_MAX = 35.0;
    const double GOOD_MS_MAX = 60.0;
    const double BAD_MS_MAX = 90.0;
    const double MISS_MS_MAX = 200.0;

    public void Init(R3_NoteData nd, RRGameManager manager, RectTransform hitLine, float spawnPrimary, double travelTimeSec, float judgeRadiusPx)
    {
        data = nd;
        gm = manager;
        rt = GetComponent<RectTransform>();
        hitLineRect = hitLine;
        spawnPosPrimary = spawnPrimary;
        travelTime = travelTimeSec;
        judgeRadius = judgeRadiusPx;

        var head = transform.Find("Head");
        if (head != null) headRect = head.GetComponent<RectTransform>();
        var body = transform.Find("Body");
        if (body != null) bodyRect = body.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (gm == null || rt == null || data == null) return;

        double cur = gm.GetSongTime();
        double timeToHit = data.time - cur;
        double progress = 1.0 - (timeToHit / travelTime);
        float p = Mathf.Clamp01((float)progress);

        float hitX = hitLineRect.anchoredPosition.x;
        float hitY = hitLineRect.anchoredPosition.y;

        bool moveHorizontal = (data.lane == "left" || data.lane == "right");

        if (moveHorizontal)
        {
            float x = Mathf.Lerp(spawnPosPrimary, hitX, p);
            rt.anchoredPosition = new Vector2(x, rt.anchoredPosition.y);
        }
        else
        {
            float y = Mathf.Lerp(spawnPosPrimary, hitY, p);
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);
        }

        // 롱노트 길이 줄어들기
        if (data.type == "long" && isHoldActive && isBeingHeld && bodyRect != null)
        {
            double elapsed = cur - data.time;
            float ratio = Mathf.Clamp01((float)(elapsed / (float)data.holdDuration));
            float newLen = Mathf.Lerp(initialBodyLength, 0f, ratio);
            Vector2 sz = bodyRect.sizeDelta;
            sz.y = newLen;
            bodyRect.sizeDelta = sz;
        }

        // 너무 늦은 노트 자동 Miss 처리
        if (!judged && (gm.GetSongTime() - data.time) > (MISS_MS_MAX / 1000.0))
        {
            judged = true;
            ApplyJudge("Miss");
            Destroy(gameObject, 0.02f);
        }
    }

    // 노트 눌렀을 때
    public void OnHitAttempt(double inputSongTime)
    {
        if (judged || data == null) return;

        double diffMs = (inputSongTime - data.time) * 1000.0;
        double absMs = System.Math.Abs(diffMs);
        string label;

        if (absMs <= PERFECT_MS) label = "Perfect";
        else if (absMs <= GREAT_MS_MAX) label = "Great";
        else if (absMs <= GOOD_MS_MAX) label = "Good";
        else if (absMs <= BAD_MS_MAX) label = "Bad";
        else label = "Miss";

        judged = true;
        ApplyJudge(label);

        if (data.type == "long" && data.holdDuration > 0.0)
        {
            holdEndTime = data.time + data.holdDuration;
            isHoldActive = true;
            isBeingHeld = true;

            if (headRect != null) headRect.gameObject.SetActive(false);
            if (bodyRect != null && initialBodyLength <= 0f)
                initialBodyLength = bodyRect.sizeDelta.y;
        }
        else
        {
            Destroy(gameObject, 0.02f);
        }
    }

    //키를 누르고 있을 때 유지
    public void OnHoldMaintain(double curSongTime)
    {
        if (data == null || data.type != "long") return;

        if (!isHoldActive)
        {
            if (curSongTime >= data.time)
            {
                isHoldActive = true;
                isBeingHeld = true;
                holdEndTime = data.time + data.holdDuration;
            }
        }
        else
        {
            isBeingHeld = true;
            if (curSongTime >= holdEndTime)
                HoldComplete();
        }
    }

    //손을 뗐을 때
    public void OnHoldRelease(double releaseTime)
    {
        if (data == null || data.type != "long") return;
        if (!isHoldActive) { Destroy(gameObject, 0.02f); return; }

        isBeingHeld = false;

        double holdRatio = (releaseTime - data.time) / data.holdDuration;

        if (holdRatio >= 0.9) HoldComplete();
        else
        {
            isHoldActive = false;
            Destroy(gameObject, 0.02f);
        }
    }

    void HoldComplete()
    {
        ApplyJudge("Hold Complete");
        isHoldActive = false;
        isBeingHeld = false;
        Destroy(gameObject, 0.02f);
    }

    void ApplyJudge(string label)
    {
        if (gm != null && hitLineRect != null)
        {
            Vector2 showPos = hitLineRect.anchoredPosition;
            gm.ShowJudgeAt(label, showPos);
        }
    }
}
