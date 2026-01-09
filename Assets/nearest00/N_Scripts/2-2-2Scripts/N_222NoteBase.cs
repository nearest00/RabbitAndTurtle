using UnityEngine;

public abstract class N_222NoteBase : MonoBehaviour
{
    public enum NoteType
    {
        Tap,
        LongStart,
        LongHold,
        LongEnd
    }

    [HideInInspector] public KeyCode inputKey;
    [HideInInspector] public int slotIndex;
    [HideInInspector] public NoteType noteType;

    protected RectTransform rectTransform;
    protected N_222JudgeManager judgeManager;

    protected bool isFinished;
    public bool IsFinished => isFinished;


    public RectTransform RectTransform => rectTransform;

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(
        KeyCode key,
        int slot,
        N_222JudgeManager judge
    )
    {
        inputKey = key;
        slotIndex = slot;
        judgeManager = judge;
    }

    public virtual void OnInputDown() { }
    public virtual void OnInputHold() { }
    public virtual void OnInputUp() { }

    public abstract void OnPerfect();
    public abstract void OnGood();
    public abstract void OnMiss();
}
