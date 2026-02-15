using System;

[Serializable]
public class R3_NoteData
{
    public double time;          // 판정 타이밍(초)
    public string lane;          // "up","down","left","right"
    public string type;          // "tap" 또는 "long"
    public double holdDuration;  // 롱노트 유지시간(초), tap은 0
}
