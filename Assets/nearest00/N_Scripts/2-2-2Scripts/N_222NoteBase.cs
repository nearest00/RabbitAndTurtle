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

    [Header("Sprites (Left, Right, Up, Down)")]
    [SerializeField] private Sprite imageLeft; [SerializeField] private Sprite imageRight;
    [SerializeField] private Sprite imageUp; [SerializeField] private Sprite imageDown;

    [Header("Multi Tap Sprites")]
    [SerializeField] private Sprite multiLeft; [SerializeField] private Sprite multiRight;
    [SerializeField] private Sprite multiUp; [SerializeField] private Sprite multiDown;

    [Header("Many Tap Sprites")]
    [SerializeField] private Sprite manyLeft; [SerializeField] private Sprite manyRight;
    [SerializeField] private Sprite manyUp; [SerializeField] private Sprite manyDown;

    public RectTransform RectTransform => GetComponent<RectTransform>();

    // [수정] 매개변수를 두 개(k1, k2) 받는 버전으로 변경하여 데이터 누락 방지
    public void SetKeyAndVisual(string k1, string k2) // k2를 필수 인자로 변경
    {
        // 1. 첫 번째 키 설정
        inputKey = ConvertToKeyCode(k1);

        // 2. 두 번째 키 설정 (라운드 매니저에서 넘어온 k2를 직접 사용)
        if (noteType == NoteType.MultiTap)
        {
            inputKey2 = ConvertToKeyCode(k2);

            // 만약 인스펙터 Key2가 비어있을 때를 대비한 안전장치 (선택 사항)
            if (inputKey2 == KeyCode.None && k1.Contains(","))
            {
                string[] split = k1.Split(',');
                inputKey2 = ConvertToKeyCode(split[1]);
            }
        }
        else
        {
            inputKey2 = KeyCode.None;
        }

        // 디버깅: 이제 인스펙터 설정값이 로그에 찍혀야 합니다.
        if (noteType == NoteType.MultiTap)
            Debug.Log($"<color=cyan>[NoteBase]</color> 멀티탭 키 설정 완료: {inputKey} + {inputKey2}");

        // 3. 비주얼 설정 (기존 로직 유지)
        if (noteType == NoteType.LongHold || noteType == NoteType.LongEnd) return;
        if (noteImage == null) noteImage = GetComponent<Image>();

        Sprite selectedSprite = null;
        string mainKey = k1.ToLower().Trim();

        if (mainKey.Contains("left")) selectedSprite = GetSprite(imageLeft, multiLeft, manyLeft);
        else if (mainKey.Contains("right")) selectedSprite = GetSprite(imageRight, multiRight, manyRight);
        else if (mainKey.Contains("up")) selectedSprite = GetSprite(imageUp, multiUp, manyUp);
        else if (mainKey.Contains("down")) selectedSprite = GetSprite(imageDown, multiDown, manyDown);

        if (selectedSprite != null && noteImage != null) noteImage.sprite = selectedSprite;
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