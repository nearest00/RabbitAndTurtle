using System.Collections;
using UnityEngine;

public class RRPanelFade : MonoBehaviour
{
    private CanvasGroup cg;
    public bool useScale = true;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        gameObject.SetActive(false);
    }

    public void FadeIn(float duration)
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(FadeInRoutine(duration));
    }

    private IEnumerator FadeInRoutine(float duration)
    {
        if (cg == null) yield break;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        Vector3 startScale = useScale ? Vector3.one * 0.95f : transform.localScale;
        Vector3 endScale = Vector3.one;
        if (useScale) transform.localScale = startScale;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            cg.alpha = Mathf.Lerp(0f, 1f, p);
            if (useScale) transform.localScale = Vector3.Lerp(startScale, endScale, p);
            yield return null;
        }

        cg.alpha = 1f;
        if (useScale) transform.localScale = endScale;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    public void FadeOut(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutRoutine(duration));
    }

    private IEnumerator FadeOutRoutine(float duration)
    {
        if (cg == null) yield break;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.one * 0.95f;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            cg.alpha = Mathf.Lerp(1f, 0f, p);
            transform.localScale = Vector3.Lerp(startScale, endScale, p);
            yield return null;
        }

        cg.alpha = 0f;
        gameObject.SetActive(false);
    }
}
