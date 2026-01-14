using System.Collections.Generic;
using UnityEngine;

public class RNoteManager : MonoBehaviour //노트생성
{
    public GameObject upNotePrefab;
    public GameObject downNotePrefab;

    public List<RNoteData> notes = new List<RNoteData>();

    int index;

    void Start()
    {
        index = 0;
        RGameManager.Instance.totalNotes = notes.Count;
    }

    void Update()
    {
        double songTime = RGameManager.Instance.GetSongTime();

        if (index < notes.Count && songTime >= notes[index].time - 2f)
        {
            Spawn(notes[index]);
            index++;
        }
    }

    void Spawn(RNoteData data)
    {
        GameObject prefab =
            data.dir == NoteDirection.Up ? upNotePrefab : downNotePrefab;

        GameObject go = Instantiate(prefab, transform);
        go.GetComponent<RNote>().data = data;
    }

    float GetNoteTime(RBeatNote note, float bpm)
    {
        float beatLength = 60f / bpm;
        return (note.bar * 4f + note.beat) * beatLength;
    }

    public void LoadChart(RChartData chart)
    {
        notes.Clear();

        foreach (var b in chart.beatNotes)
        {
            notes.Add(new RNoteData
            {
                time = GetNoteTime(b, chart.bpm),
                dir = b.dir
            });
        }

        RGameManager.Instance.totalNotes = notes.Count;
        index = 0;
    }
}
