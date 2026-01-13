using UnityEngine;

public abstract class N_222NoteBase : MonoBehaviour
{
    public enum NoteType { Tap, LongStart, LongHold, LongEnd, Many }

    [Header("Base Settings")]
    public KeyCode inputKey;
    public NoteType noteType;
    public int roundID; // 같은 묶음인지 확인용
    public bool IsFinished { get; protected set; }

    private RectTransform rectTransform;
    public RectTransform RectTransform => rectTransform ??= GetComponent<RectTransform>();

    public abstract void OnPerfect();
    public abstract void OnGood();
    public abstract void OnMiss();

    public virtual void OnInputDown() { }
    public virtual void OnInputUp() { }

    public void SetFinished(bool value)
    {
        IsFinished = value;
        if (!value) gameObject.SetActive(true);
    }
}