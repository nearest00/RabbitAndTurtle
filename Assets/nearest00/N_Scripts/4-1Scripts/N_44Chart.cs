using UnityEngine;
using System.Collections.Generic;

public enum NoteType { Single, Long }
public enum NoteDirection { Left, Down, Up, Right }

[System.Serializable]
public class NoteInfo
{
    public float hitTime;           // 노트를 쳐야 하는 시간
    public NoteType type;           // 단타 vs 롱노트
    public NoteDirection direction; // 방향
    public float duration;          // 롱노트일 경우의 길이
    public bool isPlayerNote;       // 플레이어용인지 가이드용(왼쪽)인지 구분
}

[CreateAssetMenu(fileName = "NewChart", menuName = "N_44/Chart Data")]
public class N_44Chart : ScriptableObject
{
    public string songName;
    public float bpm;

    // 변수명을 notes에서 noteList로 변경 (모호성 방지)
    public List<NoteInfo> noteList = new List<NoteInfo>();

    [ContextMenu("Sort Notes")]
    public void SortNotes()
    {
        noteList.Sort((a, b) => a.hitTime.CompareTo(b.hitTime));
        Debug.Log($"{songName} 채보 정렬 완료!");
    }
}