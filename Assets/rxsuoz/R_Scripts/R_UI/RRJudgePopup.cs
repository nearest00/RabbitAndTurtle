// JudgePopup.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RRJudgePopup : MonoBehaviour
{
    public float moveUpDistance = 60f;
    public float duration = 0.9f;
    public AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    private Text txt;
    private RectTransform rt;

    void Awake()
    {
        txt = GetComponent<Text>();
        rt = GetComponent<RectTransform>();
    }

    public void Play(string label)
    {
        if (txt != null) txt.text = label;
        StartCoroutine(DoPopup());
    }

    IEnumerator DoPopup()
    {
        float t = 0f;
        Vector2 start = rt.anchoredPosition;
        Color baseColor = txt != null ? txt.color : Color.white;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            rt.anchoredPosition = start + new Vector2(0, moveUpDistance * p);
            if (txt != null)
            {
                Color c = baseColor;
                c.a = alphaCurve.Evaluate(p);
                txt.color = c;
            }
            yield return null;
        }
        Destroy(gameObject);
    }
}
