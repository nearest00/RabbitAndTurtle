using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RRJudgePopup : MonoBehaviour
{
    public Image image; // assign via inspector (Image component)
    public Sprite perfectSprite;
    public Sprite greatSprite;
    public Sprite goodSprite;
    public Sprite badSprite;
    public Sprite missSprite;

    public float moveUp = 40f;
    public float duration = 0.7f;

    public void Play(string judge)
    {
        if (image == null) image = GetComponent<Image>();

        switch (judge)
        {
            case "Perfect":
                image.sprite = perfectSprite;
                break;
            case "Great":
                image.sprite = greatSprite;
                break;
            case "Good":
                image.sprite = goodSprite;
                break;
            case "Bad":
                image.sprite = badSprite;
                break;
            default:
                image.sprite = missSprite;
                break;
        }

        image.SetNativeSize();
        StartCoroutine(PopupRoutine());
    }

    IEnumerator PopupRoutine()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();

        float t = 0f;
        Vector3 start = transform.localPosition;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            // move up and fade out
            transform.localPosition = start + Vector3.up * (moveUp * p);
            cg.alpha = 1.0f - p;
            yield return null;
        }

        Destroy(gameObject);
    }
}
