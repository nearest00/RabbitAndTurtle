using System;

[Serializable]
public class R3_NoteData
{
    public double time;          // 판정 타임 (초)
    public string noteDirection; // up, down, left, right
    public string noteType;      // tap, long
    public double holdDuration;  // 롱노트 지속 시간
}