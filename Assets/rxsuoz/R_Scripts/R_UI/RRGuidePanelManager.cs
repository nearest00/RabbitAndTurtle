using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RRGuidePanelManager : MonoBehaviour
{
    [Header("Guide Panels (in order)")]
    public GameObject[] panels; // 순서대로 연결 (1페이지, 2페이지, 3페이지...)

    [Header("Settings")]
    public float fadeDuration = 0.4f;
    public bool allowClickAnywhere = true;

    private int currentIndex = -1;
    public bool IsFinished { get; private set; } = false;

    void Start()
    {
        // 모든 패널 비활성화
        foreach (var p in panels)
            if (p != null) p.SetActive(false);
    }

    public void StartGuide()
    {
        IsFinished = false;
        currentIndex = -1;
        ShowNext();
    }

    public void ShowNext()
    {
        StartCoroutine(ShowNextRoutine());
    }

    private IEnumerator ShowNextRoutine()
    {
        // 이전 패널 닫기
        if (currentIndex >= 0 && currentIndex < panels.Length)
        {
            GameObject prev = panels[currentIndex];
            if (prev != null)
            {
                var fade = prev.GetComponent<RRPanelFade>();
                if (fade != null) fade.FadeOut(fadeDuration);
                else prev.SetActive(false);
                yield return new WaitForSeconds(fadeDuration);
            }
        }

        currentIndex++;

        // 모든 패널 종료 시
        if (currentIndex >= panels.Length)
        {
            IsFinished = true;
            yield break;
        }

        // 현재 패널 열기
        GameObject cur = panels[currentIndex];
        if (cur != null)
        {
            var fade = cur.GetComponent<RRPanelFade>();
            if (fade != null) fade.FadeIn(fadeDuration);
            else cur.SetActive(true);
        }

        // 클릭 시 다음으로 넘기기 (옵션)
        if (allowClickAnywhere)
        {
            AddClickListener(cur);
        }
    }

    private void AddClickListener(GameObject panel)
    {
        Button btn = panel.GetComponentInChildren<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(ShowNext);
            return;
        }

        // 버튼이 없으면 클릭 전체 영역 감지 버튼 자동 추가
        var overlay = panel.transform.Find("ClickOverlay");
        if (overlay == null)
        {
            GameObject ov = new GameObject("ClickOverlay", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            ov.transform.SetParent(panel.transform, false);
            RectTransform rt = ov.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            ov.GetComponent<Image>().color = new Color(0, 0, 0, 0); // 투명 클릭 영역
            Button b = ov.GetComponent<Button>();
            b.onClick.AddListener(ShowNext);
        }
    }
}
