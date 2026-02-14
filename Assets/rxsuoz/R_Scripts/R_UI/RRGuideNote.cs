using UnityEngine;

public class RRGuideNote : MonoBehaviour
{
    [HideInInspector] public float fallSpeed = 500f; // 내려오는 속도 (UI 기준)
    [HideInInspector] public string lane;             // "up" or "down"
    [HideInInspector] public bool isActive = true;    // 화면에 표시 중인지

    private RectTransform rect;
    private RectTransform targetLine; // 판정선 위치 기준점

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    // 초기화
    public void Init(string laneType, RectTransform judgeLine, float speed)
    {
        lane = laneType;
        targetLine = judgeLine;
        fallSpeed = speed;
        isActive = true;
    }

    void Update()
    {
        if (!isActive) return;

        // 시간에 따라 내려감
        rect.anchoredPosition -= new Vector2(0, fallSpeed * Time.deltaTime);

        // 판정선 아래로 지나가면 제거
        if (rect.anchoredPosition.y < targetLine.anchoredPosition.y - 200f)
        {
            Destroy(gameObject);
        }
    }

    // 사라지기 (Perfect 시)
    public void Hide()
    {
        if (!isActive) return;
        isActive = false;
        Destroy(gameObject);
    }
}
