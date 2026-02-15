using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class R3_JudgePopup : MonoBehaviour
{
    public Image judgeImage;
    public Sprite perfectSprite, greatSprite, goodSprite, badSprite, missSprite;

    public void Play(string judge)
    {
        if (judgeImage == null) judgeImage = GetComponent<Image>();
        float delta = 0;
        switch (judge)
        {
            case "Perfect": judgeImage.sprite = perfectSprite; delta = 100; break;
            case "Great": judgeImage.sprite = greatSprite; delta = 7; break;
            case "Good": judgeImage.sprite = goodSprite; delta = 4; break;
            case "Bad": judgeImage.sprite = badSprite; delta = 1; break;
            default: judgeImage.sprite = missSprite; delta = -50; break;
        }
        judgeImage.SetNativeSize();
        RLifeSlider.Instance.AddValue(delta);
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        for (float i = 0; i < 0.6f; i += Time.deltaTime)
        {
            transform.localPosition += Vector3.up * 1f;
            cg.alpha = 1f - (i / 0.6f);
            yield return null;
        }
        Destroy(gameObject);
    }
}