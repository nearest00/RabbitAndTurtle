using UnityEngine;

public enum NoteDirection { Up, Down }

[System.Serializable]
public class RBeatNote
    {
        public int bar;
        public float beat;
        public NoteDirection dir;
    }
