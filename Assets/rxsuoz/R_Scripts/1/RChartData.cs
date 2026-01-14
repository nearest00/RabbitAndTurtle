using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RhythmGame/ChartData")]
public class RChartData : ScriptableObject
{
    public float bpm;
    public List<RBeatNote> beatNotes;
}
