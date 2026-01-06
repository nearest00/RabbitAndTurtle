using UnityEngine;

public class N_222JudgeLine : MonoBehaviour
{
    [SerializeField] float speed = 400f;

    RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        rect.anchoredPosition += Vector2.right * speed * Time.deltaTime;
    }

    public void ResetPosition(float startX)
    {
        rect.anchoredPosition =
            new Vector2(startX, rect.anchoredPosition.y);
    }
}
