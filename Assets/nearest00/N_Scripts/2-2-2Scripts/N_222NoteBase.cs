using UnityEngine;
using UnityEngine.UI;

public abstract class N_222NoteBase : MonoBehaviour
{
    public enum NoteType { Tap, MultiTap, ManyTap, LongStart, LongHold, LongEnd }
    public NoteType noteType;

    public KeyCode inputKey;  // KeyCode 타입
    public KeyCode inputKey2; // 멀티탭용
    public int roundID;
    public bool IsFinished { get; set; } 
    public bool isJudged { get; set; }
    [Header("Image Component")]
    [SerializeField] protected Image noteImage;

    [Header("Normal Sprites")]
    [SerializeField] private Sprite imageLeft;
    [SerializeField] private Sprite imageRight;
    [SerializeField] private Sprite imageUp;
    [SerializeField] private Sprite imageDown;

    [Header("Multi Tap Sprites")]
    [SerializeField] private Sprite multiLeft;
    [SerializeField] private Sprite multiRight;
    [SerializeField] private Sprite multiUp;
    [SerializeField] private Sprite multiDown;

    [Header("Many Tap Sprites")]
    [SerializeField] private Sprite manyLeft;
    [SerializeField] private Sprite manyRight;
    [SerializeField] private Sprite manyUp;
    [SerializeField] private Sprite manyDown;

    public RectTransform RectTransform => GetComponent<RectTransform>();

    public void SetKeyAndVisual(string keyName) // 여기서 string을 받음
    {
        Debug.Log($"[데이터 확인] 전달된 키 텍스트: [{keyName}]");
        if (string.IsNullOrEmpty(keyName)) return;
        string lowerKey = keyName.ToLower().Trim();

        // 1. 롱노트 바디/테일은 이미지 안 바꾸고 키만 설정 후 종료(return)
        if (noteType == NoteType.LongHold || noteType == NoteType.LongEnd)
        {
            inputKey = ConvertToKeyCode(lowerKey);
            return;
        }

        if (noteImage == null) noteImage = GetComponent<Image>();

        // 2. 키 이름에 따라 KeyCode 할당 및 Sprite 선택
        Sprite selectedSprite = null;
        inputKey = ConvertToKeyCode(lowerKey); // 문자열을 KeyCode로 변환

        switch (lowerKey)
        {
            case "leftarrow": selectedSprite = GetSprite(imageLeft, multiLeft, manyLeft); break;
            case "rightarrow": selectedSprite = GetSprite(imageRight, multiRight, manyRight); break;
            case "uparrow": selectedSprite = GetSprite(imageUp, multiUp, manyUp); break;
            case "downarrow": selectedSprite = GetSprite(imageDown, multiDown, manyDown); break;
        }

        // 3. 최종 이미지 적용
        if (selectedSprite != null && noteImage != null)
        {
            noteImage.sprite = selectedSprite;
        }
    }

    // 문자열을 KeyCode로 바꿔주는 헬퍼 함수
    private KeyCode ConvertToKeyCode(string key)
    {
        switch (key)
        {
            case "leftarrow": return KeyCode.LeftArrow;
            case "rightarrow": return KeyCode.RightArrow;
            case "uparrow": return KeyCode.UpArrow;
            case "downarrow": return KeyCode.DownArrow;
            default: return KeyCode.None;
        }
    }

    private Sprite GetSprite(Sprite normal, Sprite multi, Sprite many)
    {
        if (noteType == NoteType.MultiTap) return multi;
        if (noteType == NoteType.ManyTap) return many;
        return normal;
    }

    public abstract void OnPerfect();
    public abstract void OnGreat();
    public abstract void OnGood();
    public abstract void OnBad();
    public abstract void OnMiss();
}