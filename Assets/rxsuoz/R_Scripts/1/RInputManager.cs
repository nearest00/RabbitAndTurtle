using UnityEngine;
using System.Linq;

public class RInputManager : MonoBehaviour
{
    public float missThresholdMs = 90f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            TryJudge(NoteDirection.Up);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            TryJudge(NoteDirection.Down);
    }

    void TryJudge(NoteDirection dir)
    {
        RNote[] notes = FindObjectsOfType<RNote>();

        RNote target = notes
            .Where(n => n.data.dir == dir)
            .OrderBy(n => Mathf.Abs(
                (float)(RGameManager.Instance.GetSongTime() - n.data.time)))
            .FirstOrDefault();

        if (target == null) return;

        double songTime = RGameManager.Instance.GetSongTime();
        double diffMs = Mathf.Abs(
            (float)(songTime - target.data.time)) * 1000.0;

        if (diffMs > missThresholdMs)
            return; // 아직 누르면 안 되는 타이밍

        Judge(diffMs, target);
    }

    void Judge(double diffMs, RNote note)
    {
        if (diffMs <= 15)
        {
            RGameManager.Instance.AddScore(10);
            RGameManager.Instance.ShowJudge("Perfect");
        }
        else if (diffMs <= 35)
        {
            RGameManager.Instance.AddScore(7);
            RGameManager.Instance.ShowJudge("Great");
        }
        else if (diffMs <= 60)
        {
            RGameManager.Instance.AddScore(4);
            RGameManager.Instance.ShowJudge("Good");
        }
        else
        {
            RGameManager.Instance.AddScore(1);
            RGameManager.Instance.ShowJudge("Bad");
        }

        Destroy(note.gameObject);
    }
}
