using UnityEngine;

public class N_222LongNote : N_222NoteBase
{
    [Header("Long Note Visual Objects")]
    [SerializeField] private GameObject headObject;
    [SerializeField] private GameObject bodyObject;
    [SerializeField] private GameObject tailObject;

    public void UpdateVisual()
    {
        if (headObject != null) headObject.SetActive(noteType == NoteType.LongStart);
        if (bodyObject != null) bodyObject.SetActive(noteType == NoteType.LongHold);
        if (tailObject != null) tailObject.SetActive(noteType == NoteType.LongEnd);
    }

    private void Start() => UpdateVisual();

    public override void OnInputDown()
    {
        if (noteType == NoteType.LongStart) Debug.Log("<color=green>Long Press Start</color>");
    }

    public override void OnInputUp()
    {
        if (noteType == NoteType.LongEnd) OnPerfect();
    }

    public override void OnPerfect()
    {
        IsFinished = true;
        gameObject.SetActive(false);
        Debug.Log("<color=cyan>Long Note Success!</color>");
    }

    public override void OnGood() => OnPerfect();

    public override void OnMiss()
    {
        IsFinished = true;
        gameObject.SetActive(false);
        Debug.Log("<color=red>Long Note Missed & Deleted</color>");
    }
}