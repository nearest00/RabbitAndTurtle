using UnityEngine;
using UnityEngine.UI;

public class UI_Fish : MonoBehaviour
{
    public int lane; // 0: 위, 1: 아래
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetUIPosition(int targetLane, float yPos, float xPos)
    {
        lane = targetLane;
        // UI는 anchoredPosition을 사용합니다.
        rectTransform.anchoredPosition = new Vector2(xPos, yPos);
    }

    public void MoveRight(float distance)
    {
        rectTransform.anchoredPosition += new Vector2(distance, 0);
    }
}