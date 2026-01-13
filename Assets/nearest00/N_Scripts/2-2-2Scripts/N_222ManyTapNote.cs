using UnityEngine;
using UnityEngine.UI;

public class N_222ManyTapNote : N_222NoteBase
{
    [Header("Many Note Settings")]
    [SerializeField] private int targetTapCount = 10;
    private int currentTapCount = 0;

    [SerializeField] private Text countText;

    private void Awake()
    {
        noteType = NoteType.Many;
        UpdateCountUI();
    }

    public override void OnPerfect()
    {
        if (IsFinished) return;
        IsFinished = true;
        Debug.Log("<color=cyan>[Many]</color> Perfect! Clear");
        gameObject.SetActive(false);
    }

    public override void OnGood() => OnPerfect();

    public override void OnMiss()
    {
        if (IsFinished) return;
        IsFinished = true;
        Debug.Log("<color=red>[Many]</color> Miss!");
        gameObject.SetActive(false);
    }

    public void OnManyTapInput()
    {
        if (IsFinished) return;

        currentTapCount++;
        UpdateCountUI();

        if (currentTapCount >= targetTapCount)
        {
            OnPerfect();
        }
    }

    private void UpdateCountUI()
    {
        if (countText != null)
            countText.text = (targetTapCount - currentTapCount).ToString();
    }
}