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

    public void CreateNote(RoundNoteData data, Vector2 position, int rID)
    {
        GameObject prefab = GetPrefab(data.noteType);
        if (prefab == null) return;

        GameObject go = Instantiate(prefab, noteParent);
        N_222NoteBase note = go.GetComponent<N_222NoteBase>();

        if (note != null)
        {
            note.inputKey = data.key;
            note.inputKey2 = data.key2;
            note.noteType = data.noteType;
            note.roundID = rID;
            note.RectTransform.anchoredPosition = position;

            // 타입별 로그 출력
            Debug.Log($"<color=white>[Manager]</color> 생성: {data.noteType} | 위치: {position}");

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