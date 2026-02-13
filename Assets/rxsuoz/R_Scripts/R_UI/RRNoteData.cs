using System;

[Serializable]
public class RRNoteData
{
    public double time; //    ()
    public string lane; // "up"  "down"
    public string type; // "tap"  "hold"
    public double holdDuration; //  ()   0
}
