using UnityEngine;

// 모든 노트의 기본 클래스
public class RNote : MonoBehaviour
{
    // 노트 이동 속도 (픽셀/초)
    public float noteSpeed = 500f;

    // 노트의 타입 (0: 왼쪽, 1: 아래쪽)
    public int noteType;

    // 노트가 판정 라인에 도달해야 하는 정확한 시간
    public float exactTime;

    // RectTransform 캐시 (성능 최적화)
    private RectTransform rectTransform;

    void Start()
    {
        // RectTransform 컴포넌트 캐시
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // 노트를 아래로 이동
        // RectTransform의 anchoredPosition을 조정하여 노트를 움직임
        Vector2 currentPos = rectTransform.anchoredPosition;
        currentPos.y -= noteSpeed * Time.deltaTime;  // Y축을 감소시켜 아래로 이동
        rectTransform.anchoredPosition = currentPos;
    }

    // 현재 노트의 위치를 반환하는 메소드
    public Vector2 GetCurrentPosition()
    {
        return rectTransform.anchoredPosition;
    }

    // 노트의 타입을 설정하는 메소드
    public void SetNoteType(int type)
    {
        noteType = type;
    }
}