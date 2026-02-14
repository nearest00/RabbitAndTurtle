using UnityEngine;
using UnityEngine.UI;

public class RRNote : MonoBehaviour
{
    public RRNoteData data;

    private RRGameManager gm;
    private RectTransform rt;
    private RectTransform hitLineRect;
    private float spawnY;
    private double travelTime;
    private float judgeRadius;
    private bool judged = false;
    private bool isHoldActive = false;
    private double holdEndTime = 0.0;

    // Judge timing in milliseconds
    const double PERFECT_MS = 35.0;
    const double GREAT_MS_MAX = 70.0;
    const double GOOD_MS_MAX = 110.0;
    const double BAD_MS_MAX = 160.0;
    const double MISS_MS_MAX = 200.0;

    public void Init(RRNoteData nd, RRGameManager manager, RectTransform hitLineRect, float spawnY, double travelTime, float judgeRadius)
    {
        this.data = nd;
        this.gm = manager;
        this.rt = GetComponent<RectTransform>();
        this.hitLineRect = hitLineRect;
        this.spawnY = spawnY;
        this.travelTime = travelTime;
        this.judgeRadius = judgeRadius;

        UpdateVisual();
    }

    void UpdateVisual()
    {
        var img = GetComponent<Image>();
        /*if (img != null)
        {
            if (data.lane == "up") img.color = Color.cyan;
            else img.color = Color.magenta;

            if (data.type == "long") img.color *= 0.85f;
        }*/
    }

    void Update()
    {
        if (gm == null) return;

        double curSongTime = gm.GetSongTime();
        double timeToHit = data.time - curSongTime;
        double progress = 1.0 - (timeToHit / travelTime);
        progress = Mathf.Clamp01((float)progress);

        float y = Mathf.Lerp(spawnY, hitLineRect.anchoredPosition.y, (float)progress);
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);

        // Auto miss if way too late
        if (!judged && curSongTime - data.time > (MISS_MS_MAX / 1000.0))
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

        // Time-based judge (no distance condition)
        string label;
        int add;

        if (absMs <= PERFECT_MS) { label = "Perfect"; add = 10; } //¿ø·¡10
        else if (absMs <= GREAT_MS_MAX) { label = "Great"; add = 7; }
        else if (absMs <= GOOD_MS_MAX) { label = "Good"; add = 4; }
        else if (absMs <= BAD_MS_MAX) { label = "Bad"; add = 1; }
        else { label = "Miss"; add = -50; }

        judged = true;
        ApplyJudge(label, add);

        if (data.type == "long" && data.holdDuration > 0.0)
        {
            holdEndTime = data.time + data.holdDuration;
            isHoldActive = true;
            Destroy(gameObject, (float)(data.holdDuration + 0.5));
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
            ApplyJudge("Miss", -50);
        else
            ApplyJudge("Hold Complete", 0);
    }

    void ApplyJudge(string label, int scoreDelta)
    {
        if (gm != null)
        {
            gm.AddScore(scoreDelta);

            Vector2 showPos = hitLineRect.anchoredPosition;
            float xOffset = (data.lane == "up") ? 80f : -80f;
            Vector2 anchored = new Vector2(showPos.x + xOffset, showPos.y + 20f);
            gm.ShowJudgeAt(label, anchored);
        }
        Debug.Log("Judge: " + label + " (" + data.lane + ") " + " diff=" + label);
    }
}
