using UnityEngine;
using UnityEngine.UI;

public class RRNote : MonoBehaviour
{
    public RRNoteData data;
    public float fallSpeed;

    private RRGameManager gm;
    private RectTransform rt;
    private RectTransform hitLineRect;
    private RectTransform bodyRect;
    private RectTransform headRect;

    private float spawnY;
    private double travelTime;
    private float judgeRadius;
    private bool judged = false;

    private bool isHoldActive = false;
    public bool isBeingHeld = false;
    private double holdEndTime = 0.0;

    private float initialBodyLength = 0f;

    // 판정 기준(ms)
    const double PERFECT_MS = 35.0;
    const double GREAT_MS_MAX = 70.0;
    const double GOOD_MS_MAX = 110.0;
    const double BAD_MS_MAX = 160.0;
    const double MISS_MS_MAX = 200.0;

    public void Init(RRNoteData nd, RRGameManager manager, RectTransform hitLineRect, float spawnY, double travelTime, float judgeRadius)
    {
        data = nd;
        gm = manager;
        rt = GetComponent<RectTransform>();
        this.hitLineRect = hitLineRect;
        this.spawnY = spawnY;
        this.travelTime = travelTime;
        this.judgeRadius = judgeRadius;

        headRect = transform.Find("Head")?.GetComponent<RectTransform>();
        bodyRect = transform.Find("Body")?.GetComponent<RectTransform>();
    }

    public void SetupLongVisual(float holdPixels)
    {
        if (bodyRect == null || headRect == null)
        {
            Debug.LogWarning("RRNote.SetupLongVisual: Body or Head missing in " + gameObject.name);
            return;
        }

        RectTransform root = GetComponent<RectTransform>();
        root.pivot = new Vector2(0.5f, 1.0f);

        Vector2 size = bodyRect.sizeDelta;
        size.y = holdPixels;
        bodyRect.sizeDelta = size;

        // Body는 Head 위쪽 방향으로 늘어남
        bodyRect.anchoredPosition = new Vector2(0f, holdPixels);
        initialBodyLength = holdPixels;
    }

    void Update()
    {
        if (gm == null || rt == null) return;

        double curSongTime = gm.GetSongTime();
        double timeToHit = data.time - curSongTime;
        double progress = 1.0 - (timeToHit / travelTime);
        progress = Mathf.Clamp01((float)progress);

        // 노트 이동
        float y = Mathf.Lerp(spawnY, hitLineRect.anchoredPosition.y, (float)progress);
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);

        // 롱노트 유지 중이면 길이 줄이기
        if (isHoldActive && isBeingHeld && bodyRect != null)
        {
            double elapsed = curSongTime - data.time;
            float ratio = Mathf.Clamp01((float)(elapsed / (float)data.holdDuration));
            float newLen = Mathf.Lerp(initialBodyLength, 0f, ratio);

            Vector2 size = bodyRect.sizeDelta;
            size.y = newLen;
            bodyRect.sizeDelta = size;
            bodyRect.anchoredPosition = new Vector2(0f, newLen);
        }

        // 홀드 완료 (자동)
        if (isHoldActive && curSongTime >= holdEndTime && isBeingHeld)
        {
            HoldComplete();
        }

        // Miss 처리 (탭 전용)
        if (!judged && curSongTime - data.time > (MISS_MS_MAX / 1000.0) && data.type == "tap")
        {
            judged = true;
            ApplyJudge("Miss", 0);
            Destroy(gameObject, 0.05f);
        }
    }

    public void OnHitAttempt(double inputSongTime)
    {
        if (judged) return;

        double diffMs = (inputSongTime - data.time) * 1000.0;
        double absMs = System.Math.Abs(diffMs);
        string label;

        if (absMs <= PERFECT_MS) label = "Perfect";
        else if (absMs <= GREAT_MS_MAX) label = "Great";
        else if (absMs <= GOOD_MS_MAX) label = "Good";
        else if (absMs <= BAD_MS_MAX) label = "Bad";
        else label = "Miss";

        judged = true;
        ApplyJudge(label, 0);

        // 롱노트 시작 시
        if (data.type == "long" && data.holdDuration > 0.0)
        {
            holdEndTime = data.time + data.holdDuration;
            isHoldActive = true;
            isBeingHeld = true;

            // Head 비활성화 (눌렀을 때 사라짐)
            if (headRect != null)
                headRect.gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject, 0.05f);
        }
    }

    public void OnHoldRelease(double releaseSongTime)
    {
        if (!isHoldActive) return;


        isBeingHeld = false;

        double holdProgress = (releaseSongTime - data.time) / data.holdDuration;

        if (holdProgress >= 0.9f)
        {
            HoldComplete();
        }
        else
        {
            isHoldActive = false;
            Destroy(gameObject, 0.05f);
        }
    }


    private void HoldComplete()
    {
        ApplyJudge("Hold Complete", 0);
        isHoldActive = false;
        isBeingHeld = false;
        Destroy(gameObject, 0.05f);
    }

    void ApplyJudge(string label, int scoreDelta)
    {
        if (gm != null)
        {
            Vector2 showPos = hitLineRect.anchoredPosition;
            float xOffset = (data.lane == "up") ? 80f : -80f;
            Vector2 anchored = new Vector2(showPos.x + xOffset, showPos.y + 20f);
            gm.ShowJudgeAt(label, anchored);
        }
    }
}
