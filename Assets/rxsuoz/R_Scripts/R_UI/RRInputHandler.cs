// InputHandler.cs
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RRInputHandler : MonoBehaviour
{
    public RRGameManager gm;

    void Update()
    {
        if (gm == null) return;
        double cur = gm.GetSongTime();

        if (Input.GetKeyDown(KeyCode.UpArrow))
            TryHitLane("up", cur);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            TryHitLane("down", cur);

        if (Input.GetKeyUp(KeyCode.UpArrow))
            TryReleaseLane("up", cur);
        if (Input.GetKeyUp(KeyCode.DownArrow))
            TryReleaseLane("down", cur);
    }

    void TryHitLane(string lane, double time)
    {
        RRNote best = null;
        double bestDiff = double.MaxValue;

        foreach (Transform t in gm.noteParent) // noteParent는 playArea의 transform (GameManager에서 제공)
        {
            RRNote n = t.GetComponent<RRNote>();
            if (n == null) continue;
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

    void TryReleaseLane(string lane, double time)
    {
        foreach (Transform t in gm.noteParent)
        {
            RRNote n = t.GetComponent<RRNote>();
            if (n == null) continue;
            if (n.data.lane != lane) continue;
            if (n.data.type != "long") continue;
            n.OnHoldRelease(time);
        }
    }
}
