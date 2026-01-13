using UnityEngine;

public class N_222NoteManager : MonoBehaviour
{
    [SerializeField] private GameObject tapNotePrefab;
    [SerializeField] private GameObject longNotePrefab;
    [SerializeField] private GameObject manyNotePrefab;
    [SerializeField] private RectTransform noteParent;
    [SerializeField] private N_222JudgeManager judgeManager;
    [SerializeField] private float noteSpacing = 200f;

    private int globalRoundCounter = 0; // 전체 게임 동안 올라가는 카운터

    public void SpawnRound(N_222RoundManager.RoundPattern pattern)
    {
        globalRoundCounter++; // 라운드마다 고유 번호 생성

        for (int i = 0; i < pattern.notes.Length; i++)
        {
            var data = pattern.notes[i];
            if (data == null || data.key == KeyCode.None) continue;
            CreateNote(data.noteType, data.key, i * noteSpacing, globalRoundCounter);
        }
    }

    private void CreateNote(N_222NoteBase.NoteType type, KeyCode key, float xPos, int rID)
    {
        GameObject prefab = (type == N_222NoteBase.NoteType.Many) ? manyNotePrefab :
                          (type == N_222NoteBase.NoteType.Tap ? tapNotePrefab : longNotePrefab);

        GameObject go = Instantiate(prefab, noteParent);
        N_222NoteBase note = go.GetComponent<N_222NoteBase>();

        if (note != null)
        {
            note.inputKey = key;
            note.noteType = type;
            note.roundID = rID; // [할당] 같은 라운드 노지는 같은 ID를 가짐
            note.RectTransform.anchoredPosition = new Vector2(xPos, 0);

            if (note is N_222LongNote longNote) longNote.UpdateVisual();
            if (judgeManager != null) judgeManager.RegisterNote(note);
        }
    }
}