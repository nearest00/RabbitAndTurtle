using UnityEngine;

public abstract class N_222NoteBase : MonoBehaviour
{
    public enum NoteType { Tap, MultiTap, ManyTap, LongStart, LongHold, LongEnd }
    public NoteType noteType;
    public KeyCode inputKey;
    public KeyCode inputKey2;
    public int roundID;

    public bool IsFinished { get; set; }
    public bool isJudged { get; set; }

    public RectTransform RectTransform => GetComponent<RectTransform>();

    public abstract void OnPerfect();
    public abstract void OnGood();
    public abstract void OnMiss();
}