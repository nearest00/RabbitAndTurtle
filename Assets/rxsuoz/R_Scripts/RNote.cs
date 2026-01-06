using UnityEngine;
using UnityEngine.EventSystems;
public enum NoteDirection
{
    Up,
    Down
}
public class RNote : MonoBehaviour
{
    public float speed = 400f;
    public NoteDirection direction; //  

    RectTransform rect;


    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        rect.anchoredPosition += Vector2.down * speed * Time.deltaTime;
    }

    public void Hit()
    {
        gameObject.SetActive(false);
    }
}