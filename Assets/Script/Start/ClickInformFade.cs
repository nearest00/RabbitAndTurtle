using UnityEngine;
using System.Collections;

public class FadeInImage : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float delay = 2f;
    [SerializeField] private float fadeDuration = 1f;

    private void Start()
    {
        // 시작 시 완전히 안 보이게
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        // 2초 대기
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
}
