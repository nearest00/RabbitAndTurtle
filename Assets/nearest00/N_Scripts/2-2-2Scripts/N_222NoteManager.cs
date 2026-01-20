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
        // 1. 데이터에 맞는 프리팹 가져오기
        GameObject prefab = GetPrefab(data.noteType);
        if (prefab == null) return;

        // 2. 부모 레이어 결정
        RectTransform targetParent;
        if (isDecoration)
        {
            targetParent = (data.noteType == N_222NoteBase.NoteType.LongHold) ?
                N_222RoundManager.Instance.previewBodyLayer : N_222RoundManager.Instance.previewElseLayer;
        }
        else
        {
            targetParent = (data.noteType == N_222NoteBase.NoteType.LongHold) ? bodyLayer : elseLayer;
        }

        // 3. 노트 생성 및 컴포넌트 가져오기
        GameObject go = Instantiate(prefab, targetParent);
        N_222NoteBase note = go.GetComponent<N_222NoteBase>();

        if (note != null)
        {
            note.noteType = data.noteType;
            note.roundID = rID;
            note.RectTransform.anchoredPosition = position;

            // [수정] string 타입임을 확실하게 못 박아서 전달합니다.
            // data.key가 string이 맞다면 .ToString()을 붙여도 아무 문제 없습니다.
            string finalKey = data.key.ToString();
            // NoteManager.cs의 CreateNote 내부
            note.SetKeyAndVisual(finalKey);
            Debug.Log($"노트 생성됨: 타입={note.noteType}, 설정된 키={note.inputKey}"); // 이 로그가 KeyCode.None으로 뜨면 판정 불가

            if (!isDecoration) judgeManager.RegisterNote(note);

            if (!isDecoration) judgeManager.RegisterNote(note);
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