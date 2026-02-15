using UnityEngine;

public class R3_InputHandler : MonoBehaviour
{
    public Transform playArea;

    void Update()
    {
        if (Time.timeScale <= 0) return;
        double cur = R3_GameManager.Instance.GetCurrentTime();

        HandleKey(KeyCode.UpArrow, "up", cur);
        HandleKey(KeyCode.DownArrow, "down", cur);
        HandleKey(KeyCode.LeftArrow, "left", cur);
        HandleKey(KeyCode.RightArrow, "right", cur);
    }

    void HandleKey(KeyCode key, string dir, double cur)
    {
        if (Input.GetKeyDown(key)) TryHit(dir, cur);
        if (Input.GetKeyUp(key)) TryRelease(dir, cur);
    }

    void TryHit(string dir, double cur)
    {
        R3_Note best = null;
        double min = 0.2;
        foreach (Transform t in playArea)
        {
            R3_Note n = t.GetComponent<R3_Note>();
            if (n != null && n.Data.noteDirection == dir)
            {
                double d = System.Math.Abs(cur - n.Data.time);
                if (d < min) { min = d; best = n; }
            }
        }
        if (best != null) best.OnHit(cur);
    }

    void TryRelease(string dir, double cur)
    {
        foreach (Transform t in playArea)
        {
            R3_Note n = t.GetComponent<R3_Note>();
            if (n != null && n.Data.noteDirection == dir && n.isBeingHeld)
            {
                n.OnRelease(cur);
            }
        }
    }
}