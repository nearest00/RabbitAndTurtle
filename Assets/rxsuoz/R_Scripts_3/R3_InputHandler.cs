using UnityEngine;

public class R3_InputHandler : MonoBehaviour
{
    public RRGameManager gm;

    void Update()
    {
        if (gm == null) return;
        double cur = gm.GetSongTime();

        HandleKey(KeyCode.UpArrow, "up", cur);
        HandleKey(KeyCode.DownArrow, "down", cur);
        HandleKey(KeyCode.LeftArrow, "left", cur);
        HandleKey(KeyCode.RightArrow, "right", cur);
    }

    void HandleKey(KeyCode key, string lane, double cur)
    {
        if (Input.GetKeyDown(key))
            TryHitLane(lane, cur);

        if (Input.GetKey(key))
            TryHoldLane(lane, cur);

        if (Input.GetKeyUp(key))
            TryReleaseLane(lane, cur);
    }

    void TryHitLane(string lane, double time)
    {
        if (gm == null || gm.noteParent == null) return;

        R3_Note best = null;
        double bestDiff = double.MaxValue;

        foreach (Transform t in gm.noteParent)
        {
            R3_Note n = t.GetComponent<R3_Note>();
            if (n == null || n.data == null) continue;
            if (n.data.lane != lane) continue;

            double diffMs = System.Math.Abs((time - n.data.time) * 1000.0);
            if (diffMs < bestDiff)
            {
                bestDiff = diffMs;
                best = n;
            }
        }

        if (best != null) best.OnHitAttempt(time);
    }

    void TryHoldLane(string lane, double time)
    {
        if (gm == null || gm.noteParent == null) return;

        foreach (Transform t in gm.noteParent)
        {
            R3_Note n = t.GetComponent<R3_Note>();
            if (n == null || n.data == null) continue;
            if (n.data.lane != lane) continue;
            if (n.data.type != "long") continue;

            n.OnHoldMaintain(time);
        }
    }

    void TryReleaseLane(string lane, double time)
    {
        if (gm == null || gm.noteParent == null) return;

        foreach (Transform t in gm.noteParent)
        {
            R3_Note n = t.GetComponent<R3_Note>();
            if (n == null || n.data == null) continue;
            if (n.data.lane != lane) continue;
            if (n.data.type != "long") continue;

            n.OnHoldRelease(time);
        }
    }
}
