using System;
using UnityEngine;

[Serializable]
public class N_222EasyRoundPattern
{
    public NoteSpawnData[] notes;
}

[Serializable]
public class NoteSpawnData
{
    public KeyCode key;
    public Vector2 anchoredPosition;
}
