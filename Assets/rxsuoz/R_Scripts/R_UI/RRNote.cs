// Note.cs
using UnityEngine;

public class RRNote : MonoBehaviour
{
    public RRNoteData data;
    private RRGameManager gm;
    private RectTransform rt;
    private float hitY;
    private float spawnY;
    private double travelTime;
    private bool judged = false;
    private bool isHoldActive = false;
    private double holdEndTime = 0;

    // 판정 윈도 (ms)
    const double PERFECT_MS = 15.0;
    const double GREAT_MS_MAX = 35.0;
    const double GOOD_MS_MAX = 60.0;
    const double BAD_MS_MAX = 90.0;

    public void Init(RRNoteData nd, RRGameManager manager, float hitY, float spawnY, double travelTime)
    {
        data = nd;
        gm = manager;
        rt = GetComponent<RectTransform>();
        this.hitY = hitY;
        this.spawnY = spawnY;
        this.travelTime = travelTime;
        UpdateVisual();
    }

    void UpdateVisual()
    {
        var img = GetComponent<UnityEngine.UI.Image>();
        if (img != null)
        {
            if (data.lane == "up") img.color = Color.cyan;
            else img.color = Color.magenta;
            if (data.type == "long") img.color *= 0.8f;
        }
    }

    void Update()
    {
        if (gm == null) return;

        double curSongTime = gm.GetSongTime();
        double timeToHit = data.time - curSongTime;
        double progress = 1.0 - (timeToHit / travelTime);
        progress = Mathf.Clamp01((float)progress);
        float y = Mathf.Lerp(spawnY, hitY, (float)progress);
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);

        // Miss 자동 판정: 판정 윈을 완전히 지난 경우
        if (!judged && curSongTime - data.time > (BAD_MS_MAX / 1000.0 + 0.05))
        {
            judged = true;
            ApplyJudge("Miss", -50);
            Destroy(gameObject, 0.02f);
        }
    }

    public void OnHitAttempt(double inputSongTime)
    {
        if (judged) return;
        double diffMs = (inputSongTime - data.time) * 1000.0;
        double absMs = System.Math.Abs(diffMs);

        if (absMs > BAD_MS_MAX)
        {
            judged = true;
            ApplyJudge("Miss", -50);
            Destroy(gameObject, 0.02f);
            return;
        }

        string label;
        int scoreAdd;
        if (absMs <= PERFECT_MS) { label = "Perfect"; scoreAdd = 10; }
        else if (absMs <= GREAT_MS_MAX) { label = "Great"; scoreAdd = 7; }
        else if (absMs <= GOOD_MS_MAX) { label = "Good"; scoreAdd = 4; }
        else { label = "Bad"; scoreAdd = 1; }

        judged = true;
        ApplyJudge(label, scoreAdd);

        if (data.type == "long" && data.holdDuration > 0)
        {
            holdEndTime = data.time + data.holdDuration;
            isHoldActive = true;
            Destroy(gameObject, (float)(data.holdDuration + 0.5)); // 안전하게 지연 파괴
        }
        else
        {
            Destroy(gameObject, 0.02f);
        }
    }

    public void OnHoldRelease(double releaseSongTime)
    {
        if (!isHoldActive) return;
        isHoldActive = false;
        if (releaseSongTime + 0.001 < holdEndTime - 0.05)
            ApplyJudge("Miss (Hold)", -50);
        else
            ApplyJudge("Hold Complete", 0);
    }

    void ApplyJudge(string label, int scoreDelta)
    {
        gm.AddScore(scoreDelta);

        Vector2 anchored = gm.hitLine.anchoredPosition;
        float xOffset = (data.lane == "up") ? -120f : 120f;
        Vector2 showPos = new Vector2(anchored.x + xOffset, anchored.y + 30f);
        gm.ShowJudgeAt(label, showPos);

        Debug.Log($"Judge: {label} ({scoreDelta}) at note {data.time:F3}s");
    }
}
