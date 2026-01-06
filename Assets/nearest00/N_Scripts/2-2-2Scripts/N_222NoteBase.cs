using UnityEngine;
using UnityEngine.UI;

public abstract class N_222NoteBase : MonoBehaviour
{
    public KeyCode InputKey { get; protected set; }
    protected RectTransform rect;

    protected virtual void Awake()
    {
        rect = GetComponent<RectTransform>();
    }


    public virtual void Initialize(KeyCode key, Sprite sprite, Vector2 pos)
    {
        InputKey = key;
        rect.anchoredPosition = pos;
        GetComponentInChildren<Image>().sprite = sprite;
        gameObject.SetActive(true);
    }

    public abstract void OnPerfect();
    public abstract void OnGood();
    public abstract void OnMiss();
}
