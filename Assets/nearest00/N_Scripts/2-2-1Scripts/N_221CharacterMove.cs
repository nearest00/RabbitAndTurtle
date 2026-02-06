using UnityEngine;

public class N221_CharacterMove : MonoBehaviour
{
    private RectTransform rect;
    private Vector2 targetPos;
    public float moveSpeed = 20f; // 캐릭터 이동 속도

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        // 현재 위치를 초기 목표 위치로 설정
        targetPos = rect.anchoredPosition;
    }

    // 매니저에서 호출할 함수: 캐릭터를 특정 레일의 Y값으로 이동시킴
    public void ChangeLane(float targetY)
    {
        targetPos = new Vector2(rect.anchoredPosition.x, targetY);
    }

    void Update()
    {
        // 부드럽게 레일 이동 연출
        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPos, Time.deltaTime * moveSpeed);
    }
}