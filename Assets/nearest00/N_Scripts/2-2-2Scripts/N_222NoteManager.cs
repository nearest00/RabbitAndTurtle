using UnityEngine;

public class N_222NoteManager : MonoBehaviour
{
    [SerializeField] private GameObject tapNotePrefab;
    [SerializeField] private GameObject multiTapNotePrefab;
    [SerializeField] private GameObject manyNotePrefab;

    [Header("Long Note Parts")]
    [SerializeField] private GameObject longStartPrefab; // 머리만 있는 프리팹
    [SerializeField] private GameObject longHoldPrefab;  // 몸통만 있는 프리팹
    [SerializeField] private GameObject longEndPrefab;   // 꼬리만 있는 프리팹

    [SerializeField] private RectTransform noteParent;
    [SerializeField] private N_222JudgeManager judgeManager;
    [Header("Layers")]
    [SerializeField] private RectTransform headLayer; // 머리/꼬리/일반노트용 (하단 배치)
    [SerializeField] private RectTransform bodyLayer; // 몸통용 (상단 배치)

    public void CreateNote(RoundNoteData data, Vector2 position, int rID)
    {
        GameObject prefab = GetPrefab(data.noteType);
        if (prefab == null) return;

        // [수정] 몸통(Hold)이면 bodyLayer에, 나머지는 headLayer에 생성
        RectTransform targetLayer = (data.noteType == N_222NoteBase.NoteType.LongHold) ? bodyLayer : headLayer;

        GameObject go = Instantiate(prefab, targetLayer);
        N_222NoteBase note = go.GetComponent<N_222NoteBase>();

        if (note != null)
        {
            note.inputKey = data.key;
            note.inputKey2 = data.key2;
            note.noteType = data.noteType;
            note.roundID = rID;
            note.RectTransform.anchoredPosition = position;

            judgeManager.RegisterNote(note);
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