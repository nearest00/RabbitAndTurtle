using UnityEngine;
using UnityEngine.UI;

public class InfiniteBackgroundUI : MonoBehaviour
{
    [Header("Settings")]
    public float scrollSpeed = 200f; // UI는 px 단위
    public float backgroundWidth = 0f;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        if (backgroundWidth <= 0)
        {
            backgroundWidth = rectTransform.rect.width;
        }
    }

    void Update()
    {
        // 왼쪽으로 이동
        rectTransform.anchoredPosition += Vector2.left * scrollSpeed * Time.deltaTime;

        // 화면 왼쪽으로 완전히 나갔으면 오른쪽으로 이동
        if (rectTransform.anchoredPosition.x <= -backgroundWidth)
        {
            rectTransform.anchoredPosition += new Vector2(backgroundWidth * 2f, 0f);
        }
    }
}
