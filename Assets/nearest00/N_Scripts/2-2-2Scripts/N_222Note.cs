using UnityEngine;

public class N_222Note : MonoBehaviour
{
    public KeyCode InputKey { get; private set; }
    public RectTransform Rect { get; private set; }

    void Awake()
    {
        Rect = GetComponent<RectTransform>();
    }

    public void Initialize(KeyCode key, Vector2 position)
    {
        InputKey = key;
        Rect.anchoredPosition = position;
        gameObject.SetActive(true);
    }

    public void OnPerfect()
    {
        Debug.Log($"[N_222] {InputKey} Perfect");
        gameObject.SetActive(false);
    }

    public void OnGood()
    {
        Debug.Log($"[N_222] {InputKey} Good");
        gameObject.SetActive(false);
    }

    public void OnMiss()
    {
        Debug.Log($"[N_222] {InputKey} Miss");
        gameObject.SetActive(false);
    }
}
