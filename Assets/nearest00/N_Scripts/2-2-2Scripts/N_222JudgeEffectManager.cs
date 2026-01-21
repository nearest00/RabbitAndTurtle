using UnityEngine;
using System.Collections;

public class N_222JudgeEffectManager : MonoBehaviour
{
    public static N_222JudgeEffectManager Instance;

    [Header("Judge Images (Matching Order)")]
    [SerializeField] private GameObject perfectObj;
    [SerializeField] private GameObject greatObj;
    [SerializeField] private GameObject goodObj;
    [SerializeField] private GameObject badObj;
    [SerializeField] private GameObject missObj;

    [SerializeField] private float displayTime = 0.5f; // 이미지가 떠 있는 시간
    private Coroutine currentCoroutine;

    private void Awake()
    {
        Instance = this;
        // 시작할 때 모든 판정 이미지 끄기
        HideAll();
    }

    public void ShowJudge(string rating)
    {
        // 새로운 판정이 나오면 이전 코루틴은 중지 (이미지 교체)
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);

        HideAll();

        GameObject target = null;
        switch (rating.ToLower())
        {
            case "perfect": target = perfectObj; break;
            case "great": target = greatObj; break;
            case "good": target = goodObj; break;
            case "bad": target = badObj; break;
            case "miss": target = missObj; break;
        }

        if (target != null)
        {
            currentCoroutine = StartCoroutine(DisplayRoutine(target));
        }
    }

    private IEnumerator DisplayRoutine(GameObject obj)
    {
        obj.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        obj.SetActive(false);
    }

    private void HideAll()
    {
        perfectObj.SetActive(false);
        greatObj.SetActive(false);
        goodObj.SetActive(false);
        badObj.SetActive(false);
        missObj.SetActive(false);
    }
}