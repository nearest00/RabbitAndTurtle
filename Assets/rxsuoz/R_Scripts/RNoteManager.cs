using UnityEngine;

public class RNoteManager : MonoBehaviour
{
    public int bpm = 120;
    double currentTime = 0d;

    [SerializeField] RectTransform tfNoteAppear;
    [SerializeField] GameObject goNote;
    [SerializeField] Transform notesParent;

    RTimingManager theTimingManager;

    void Start()
    {
        theTimingManager = GetComponentInChildren<RTimingManager>();
    }

    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= 60d / bpm)
        {
            SpawnNote();
            currentTime -= 60d / bpm;
        }
    }

    void SpawnNote()
    {
        GameObject noteObj = Instantiate(goNote, notesParent);
        RectTransform noteRect = noteObj.GetComponent<RectTransform>();

        noteRect.anchoredPosition = tfNoteAppear.anchoredPosition;

        //  TimingManager RNote 
        theTimingManager.notes.Add(noteObj.GetComponent<RNote>());
    }
}
