using UnityEngine;
using System.Collections.Generic;

public class N_222NoteManager : MonoBehaviour
{
    [System.Serializable]
    public class KeySpritePair
    {
        public KeyCode key;
        public Sprite sprite;
    }

    [SerializeField] N_222TapNote tapNotePrefab;
    [SerializeField] Transform noteParent;
    [SerializeField] List<KeySpritePair> keySpritePairs;

    N_222JudgeManager judgeManager;

    void Awake()
    {
        judgeManager = GetComponent<N_222JudgeManager>();
    }

    public void SpawnRound(N_222RoundManager.RoundPattern pattern)
    {
        foreach (var data in pattern.notes)
        {
            SpawnNote(data);
        }
    }

    void SpawnNote(N_222RoundManager.NoteSpawnData data)
    {
        var note = Instantiate(tapNotePrefab, noteParent);

        note.Initialize(
            data.key,
            GetSprite(data.key),
            data.anchoredPosition
        );

        judgeManager.RegisterNote(note);
    }

    Sprite GetSprite(KeyCode key)
    {
        foreach (var pair in keySpritePairs)
        {
            if (pair.key == key)
                return pair.sprite;
        }

        Debug.LogWarning($"Sprite not found for key: {key}");
        return null;
    }
    //여기서부터 판정선 초기화
    public void ClearAllNotes()
    {
        foreach (Transform child in noteParent)
        {
            child.gameObject.SetActive(false);
        }
    }

}
