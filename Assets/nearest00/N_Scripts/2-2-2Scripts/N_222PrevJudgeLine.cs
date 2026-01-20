using UnityEngine;

public class N_222PrevJudgeLine : MonoBehaviour
{
    private float speed;
    private RectTransform rect;
    private bool isMoving = false;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (isMoving && speed > 0)
        {
            rect.anchoredPosition += Vector2.right * speed * Time.deltaTime;
        }
    }

    public void SetSpeed(float bpm, float distancePerBeat)
    {
        speed = (distancePerBeat * bpm) / 60f;
    }

    public void ResetPosition(float startX)
    {
        if (rect == null) rect = GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(startX, rect.anchoredPosition.y);
        isMoving = false;
    }

    public void StartMoving() => isMoving = true;
    public void StopMoving() => isMoving = false;
}