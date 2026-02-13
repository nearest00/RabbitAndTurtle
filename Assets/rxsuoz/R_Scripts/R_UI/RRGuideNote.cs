using UnityEngine;
using UnityEngine.UI;

public class RRGuideNote : MonoBehaviour
{
    private RRNoteData data;                 // CSV에서 읽은 노트 정보
    private RectTransform rt;              // 노트 UI RectTransform
    private RectTransform hitLine;         // 판정선 위치
    private float travelTime;              // 위에서 아래로 내려오는 시간(초)

    private float startY = 600f;           // 시작 위치 Y (PlayArea 위쪽)
    private float endY;                    // 도착 위치 Y (HitLine)
    private bool initialized = false;

    public void Init(RRNoteData noteData, RectTransform hitLineRect, float travel)
    {
        data = noteData;
        hitLine = hitLineRect;
        travelTime = travel;
        rt = GetComponent<RectTransform>();

        if (rt == null)
        {
            rt = gameObject.AddComponent<RectTransform>();
        }

        // HitLine의 y 좌표를 도착 지점으로 저장
        endY = hitLine.anchoredPosition.y;

        // 색상 설정 (투명도 낮게)
        Image img = GetComponent<Image>();
        if (img != null)
        {
            if (data.lane == "up")
                img.color = new Color(0.5f, 1f, 1f, 0.5f);  // 하늘색 느낌
            else
                img.color = new Color(1f, 0.5f, 1f, 0.5f);  // 분홍색 느낌
        }

        initialized = true;
    }

    void Update()
    {
        if (!initialized || rt == null || hitLine == null) return;

        // 현재 노래 진행 시간
        double curTime = FindObjectOfType<RRGameManager>().GetSongTime();

        // 현재 노트까지 남은 시간 (초)
        double diff = data.time - curTime;

        // progress = 0 -> 시작, 1 -> 판정선 도착
        float progress = 1f - (float)(diff / travelTime);
        progress = Mathf.Clamp01(progress);

        float y = Mathf.Lerp(startY, endY, progress);
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);

        // 판정선을 50픽셀 지나면 제거
        if (y < endY - 50f)
        {
            Destroy(gameObject);
        }
    }
}
