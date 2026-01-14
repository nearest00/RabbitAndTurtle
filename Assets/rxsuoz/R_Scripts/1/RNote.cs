using UnityEngine;

public class RNote : MonoBehaviour
{
    public RNoteData data;

    public RectTransform judgeLine;
    public float travelTime = 2f;

    RectTransform rect;

    float startY;
    double spawnSongTime;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        startY = rect.anchoredPosition.y;
        spawnSongTime = RGameManager.Instance.GetSongTime();
    }

    void Update()
    {
        double songTime = RGameManager.Instance.GetSongTime();

        float t = (float)((songTime - spawnSongTime) / travelTime);
        float y = Mathf.Lerp(startY, judgeLine.anchoredPosition.y, t);

        rect.anchoredPosition =
            new Vector2(rect.anchoredPosition.x, y);
    }
}
