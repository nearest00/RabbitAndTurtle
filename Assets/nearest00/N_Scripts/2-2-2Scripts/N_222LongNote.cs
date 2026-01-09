using UnityEngine;

public class N_222LongNote : N_222NoteBase
{
    private enum LongState
    {
        Idle,
        Holding,
        Failed,
        Completed
    }

    private LongState state = LongState.Idle;

    // 롱노트 그룹 ID (같은 롱노트 묶음)
    [HideInInspector] public int longGroupId;

    // Start 노트만 상태를 관리
    private bool isMaster => noteType == NoteType.LongStart;

    protected override void Awake()
    {
        base.Awake();
    }

    public void SetLongType(NoteType type)
    {
        noteType = type;
    }

    /* =========================
     * Input
     * ========================= */

    public override void OnInputDown()
    {
        if (!isMaster) return;

        if (state != LongState.Idle)
            return;

        Debug.Log("Long Start Down");
        state = LongState.Holding;
    }

    public override void OnInputHold()
    {
        if (!isMaster) return;

        if (state != LongState.Holding)
        {
            OnMiss();
        }
    }

    public override void OnInputUp()
    {
        if (!isMaster) return;

        if (state != LongState.Holding)
        {
            OnMiss();
            return;
        }

        Debug.Log("Long End Up");
        state = LongState.Completed;
        OnPerfect();
    }

    /* =========================
     * Judge Result
     * ========================= */

    public override void OnPerfect()
    {
        Finish();
    }

    public override void OnGood()
    {
        Finish();
    }

    public override void OnMiss()
    {
        if (isFinished) return;

        Debug.Log("Long Miss");
        state = LongState.Failed;
        Finish();
    }

    private void Finish()
    {
        isFinished = true;
        gameObject.SetActive(false);

        // 같은 그룹의 롱노트 전부 제거
        judgeManager.FinishLongGroup(longGroupId);
    }
}
