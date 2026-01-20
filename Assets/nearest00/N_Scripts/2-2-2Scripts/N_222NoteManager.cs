using UnityEngine;

public class N_222NoteManager : MonoBehaviour
{
    [Header("Note Prefabs")]
    [SerializeField] private GameObject tapNotePrefab;
    [SerializeField] private GameObject multiTapNotePrefab;
    [SerializeField] private GameObject manyNotePrefab;

    [Header("Long Note Parts")]
    [SerializeField] private GameObject longStartPrefab;
    [SerializeField] private GameObject longHoldPrefab;
    [SerializeField] private GameObject longEndPrefab;

    [Header("Main Game Layers")]
    [SerializeField] private RectTransform bodyLayer;
    [SerializeField] private RectTransform elseLayer;

    [Header("External References")]
    [SerializeField] private N_222JudgeManager judgeManager;

    public void CreateNote(RoundNoteData data, Vector2 position, int rID, bool isDecoration = false)
    {
        GameObject prefab = GetPrefab(data.noteType);
        if (prefab == null) return;

        // 1. 부모 레이어 결정
        RectTransform targetParent;
        if (isDecoration)
        {
            // 미리보기용 레이어 (RoundManager의 Instance 참조)
            targetParent = (data.noteType == N_222NoteBase.NoteType.LongHold) ?
                N_222RoundManager.Instance.previewBodyLayer :
                N_222RoundManager.Instance.previewElseLayer;
        }
        else
        {
            // 실제 게임용 레이어
            targetParent = (data.noteType == N_222NoteBase.NoteType.LongHold) ? bodyLayer : elseLayer;
        }

        // 2. 생성
        GameObject go = Instantiate(prefab, targetParent);
        N_222NoteBase note = go.GetComponent<N_222NoteBase>();

        if (note != null)
        {
            note.inputKey = data.key;
            note.inputKey2 = data.key2;
            note.noteType = data.noteType;
            note.roundID = rID;
            note.RectTransform.anchoredPosition = position;

            // 판정 등록 (데코레이션이 아닐 때만)
            if (!isDecoration) judgeManager.RegisterNote(note);

            // 3. 레이어 내 순서 정렬 (겹침 방지)
            if (data.noteType == N_222NoteBase.NoteType.LongHold)
                go.transform.SetAsFirstSibling();
            else
                go.transform.SetAsLastSibling();
        }
    }

    private GameObject GetPrefab(N_222NoteBase.NoteType type)
    {
        switch (type)
        {
            case N_222NoteBase.NoteType.Tap: return tapNotePrefab;
            case N_222NoteBase.NoteType.MultiTap: return multiTapNotePrefab;
            case N_222NoteBase.NoteType.ManyTap: return manyNotePrefab;
            case N_222NoteBase.NoteType.LongStart: return longStartPrefab;
            case N_222NoteBase.NoteType.LongHold: return longHoldPrefab;
            case N_222NoteBase.NoteType.LongEnd: return longEndPrefab;
            default: return null;
        }
    }
}