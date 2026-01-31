using UnityEngine;
using UnityEngine.UI;

public abstract class N_222NoteBase : MonoBehaviour
{
    public enum NoteType { Tap, MultiTap, ManyTap, LongStart, LongHold, LongEnd }
    public NoteType noteType;

    public KeyCode inputKey;
    public KeyCode inputKey2;
    public int roundID;
    public bool IsFinished { get; set; }
    public bool isJudged { get; set; }

    [Header("Image Component")]
    [SerializeField] protected Image noteImage;

    [Header("Sprites (Single Tap)")]
    [SerializeField] private Sprite imageLeft;
    [SerializeField] private Sprite imageRight;
    [SerializeField] private Sprite imageUp;
    [SerializeField] private Sprite imageDown;

    // --- 이 부분을 아래 조합형으로 교체하세요 ---
    [Header("Multi Tap Combined Sprites")]
    [SerializeField] private Sprite multiLeftRight; // 좌 + 우
    [SerializeField] private Sprite multiUpDown;    // 상 + 하
    [SerializeField] private Sprite multiLeftUp;    // 좌 + 상
    [SerializeField] private Sprite multiLeftDown;  // 좌 + 하
    [SerializeField] private Sprite multiRightUp;   // 우 + 상
    [SerializeField] private Sprite multiRightDown; // 우 + 하

    [Header("Many Tap Sprites")]
    [SerializeField] private Sprite manyLeft;
    [SerializeField] private Sprite manyRight;
    [SerializeField] private Sprite manyUp;
    [SerializeField] private Sprite manyDown;

    public RectTransform RectTransform => GetComponent<RectTransform>();

    public void SetKeyAndVisual(string k1, string k2)
    {
        // 1. 키 데이터 설정
        inputKey = ConvertToKeyCode(k1);
        inputKey2 = (noteType == NoteType.MultiTap) ? ConvertToKeyCode(k2) : KeyCode.None;

        if (noteImage == null) noteImage = GetComponent<Image>();
        if (noteType == NoteType.LongHold || noteType == NoteType.LongEnd) return;

        // 2. 비주얼 결정 로직
        Sprite selectedSprite = null;

        if (noteType == NoteType.MultiTap)
        {
            // [중요] 조합형 이미지 찾기 로직 실행
            selectedSprite = GetMultiTapCombinedSprite(inputKey, inputKey2);
        }
        else if (noteType == NoteType.ManyTap)
        {
            selectedSprite = GetManyTapSprite(inputKey);
        }
        else
        {
            selectedSprite = GetNormalSprite(inputKey);
        }

        if (selectedSprite != null) noteImage.sprite = selectedSprite;
    }

    // [새로 추가] 멀티탭 조합 판별 함수
    private Sprite GetMultiTapCombinedSprite(KeyCode k1, KeyCode k2)
    {
        // 순서에 상관없이 어떤 키들이 눌렸는지 체크
        bool hasLeft = (k1 == KeyCode.LeftArrow || k2 == KeyCode.LeftArrow);
        bool hasRight = (k1 == KeyCode.RightArrow || k2 == KeyCode.RightArrow);
        bool hasUp = (k1 == KeyCode.UpArrow || k2 == KeyCode.UpArrow);
        bool hasDown = (k1 == KeyCode.DownArrow || k2 == KeyCode.DownArrow);

        // 조합 케이스별 이미지 리턴
        if (hasLeft && hasRight) return multiLeftRight;
        if (hasUp && hasDown) return multiUpDown;
        if (hasLeft && hasUp) return multiLeftUp;
        if (hasLeft && hasDown) return multiLeftDown;
        if (hasRight && hasUp) return multiRightUp;
        if (hasRight && hasDown) return multiRightDown;

        return null;
    }

    private Sprite GetNormalSprite(KeyCode k)
    {
        if (k == KeyCode.LeftArrow) return imageLeft;
        if (k == KeyCode.RightArrow) return imageRight;
        if (k == KeyCode.UpArrow) return imageUp;
        if (k == KeyCode.DownArrow) return imageDown;
        return null;
    }

    private Sprite GetManyTapSprite(KeyCode k)
    {
        if (k == KeyCode.LeftArrow) return manyLeft;
        if (k == KeyCode.RightArrow) return manyRight;
        if (k == KeyCode.UpArrow) return manyUp;
        if (k == KeyCode.DownArrow) return manyDown;
        return null;
    }

    private KeyCode ConvertToKeyCode(string key)
    {
        if (string.IsNullOrEmpty(key)) return KeyCode.None;
        string k = key.ToLower();
        if (k.Contains("left")) return KeyCode.LeftArrow;
        if (k.Contains("right")) return KeyCode.RightArrow;
        if (k.Contains("up")) return KeyCode.UpArrow;
        if (k.Contains("down")) return KeyCode.DownArrow;
        return KeyCode.None;
    }

    public abstract void OnPerfect();
    public abstract void OnGreat();
    public abstract void OnGood();
    public abstract void OnBad();
    public abstract void OnMiss();
}