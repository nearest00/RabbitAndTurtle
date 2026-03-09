using UnityEngine;
using UnityEngine.UI;

public class N_44Note : MonoBehaviour
{
    public N_44GameManager.NoteInfo Data;
    public bool IsFailed = false;
    public bool IsHolding = false;
    public bool IsFinished = false;

    public RectTransform targetReceptor;
    public float noteSpeed;
    private float holdScoreTimer = 0f;
    private float lastCheckedBeat = 0f;
    private int longNoteTickCount = 0;

    private RectTransform rectTransform;
    private N_44GameManager gameManager;
    private N_44InputManager inputManager;
    private N_44JudgeEffectManager judgeEffectManager;
    private N_444SFXList sfx;

    private RectTransform bodyRect;
    private AudioSource holdSFXSource;

    public void Start()
    {
        sfx = Object.FindFirstObjectByType<N_444SFXList>();
    }

    public void Setup(N_44GameManager.NoteInfo info, float speed, RectTransform receptor, Sprite noteSprite)
    {
        gameManager = Object.FindFirstObjectByType<N_44GameManager>();
        inputManager = Object.FindFirstObjectByType<N_44InputManager>();
        judgeEffectManager = Object.FindFirstObjectByType<N_44JudgeEffectManager>();

        Data = info;
        noteSpeed = speed;
        targetReceptor = receptor;
        rectTransform = GetComponent<RectTransform>();

        if (targetReceptor != null)
            transform.SetParent(targetReceptor.parent, false);

        if (gameManager != null)
            UpdatePosition(Data.hitTime - gameManager.GetBeatTime());

        Image myImage = GetComponent<Image>();
        if (myImage != null && noteSprite != null)
            myImage.sprite = noteSprite;
    }

    public void SetupLongNote(N_44GameManager.NoteInfo info, float speed, RectTransform receptor, Sprite noteSprite, Sprite bodySprite, float durationInBeats)
    {
        Setup(info, speed, receptor, noteSprite);

        GameObject bodyObj = new GameObject("LongNoteBody", typeof(Image));
        bodyObj.transform.SetParent(this.transform, false);
        bodyObj.transform.SetAsFirstSibling();

        Image bodyImg = bodyObj.GetComponent<Image>();
        bodyImg.sprite = bodySprite;
        bodyImg.type = Image.Type.Sliced;

        bodyRect = bodyObj.GetComponent<RectTransform>();
        bodyRect.pivot = new Vector2(0.5f, 1f);
        bodyRect.anchorMin = new Vector2(0.5f, 1f);
        bodyRect.anchorMax = new Vector2(0.5f, 1f);

        float finalHeight = durationInBeats * speed;
        bodyRect.sizeDelta = new Vector2(100f, finalHeight);
        bodyRect.anchoredPosition = Vector2.zero;
    }

    void Update()
    {
        if (gameManager == null || IsFinished || IsFailed) return;

        float currentBeat = gameManager.GetBeatTime();
        float beatDiff = Data.hitTime - currentBeat;

        // --- 1. 가이드 노트 (Opponent) 전용 로직 ---
        if (!Data.isPlayerNote)
        {
            UpdateGuide(currentBeat, beatDiff);
            return; // 가이드는 여기서 로직 종료
        }

        // --- 2. 플레이어 노트 전용 로직 ---
        UpdatePlayer(currentBeat, beatDiff);
    }

    private void UpdateGuide(float currentBeat, float beatDiff)
    {
        // 판정선 도달 시 자동 히트 (단타/롱노트 공통)
        if (beatDiff <= 0 && !IsHolding)
        {
			var tiger = Object.FindFirstObjectByType<N_44TigerAnimation>();
			if (tiger != null)
			{
				tiger.TigerMove(Data.direction.ToString());
			}
			if (Data.type == N_44GameManager.NoteType.Long)
                StartHolding(); // 롱노트는 홀딩 시작
            else
            {
                IsFinished = true;
                Destroy(gameObject); // 단타는 즉시 삭제
                return;
            }
        }

        if (IsHolding)
        {
            HandleHoldingEffect(currentBeat);
            // 가이드 롱노트 종료 체크
            if (currentBeat >= (Data.hitTime + Data.duration))
            {
                IsFinished = true;
                Destroy(gameObject);
            }
        }
        else
        {
            UpdatePosition(beatDiff);
        }
    }

    private void UpdatePlayer(float currentBeat, float beatDiff)
    {
        // 1. 홀딩 중 처리 (롱노트 진행 중)
        if (IsHolding)
        {
            HandleHoldingEffect(currentBeat);
            HandleLongNoteScoring(currentBeat);

            // 롱노트 끝 지점을 한참 지나쳤을 때 (자동 미스 처리)
            if (currentBeat > (Data.hitTime + Data.duration + 0.25f))
            {
                ProcessLongNoteEndMiss();
            }
            return;
        }

        // 2. 판정선을 한참 지나쳤을 때 (입력 안 해서 발생하는 미스)
        if (beatDiff < -0.25f)
        {
            IsFinished = true;
            if (judgeEffectManager != null) judgeEffectManager.ShowJudge("miss");
            N_44LifeSlider.Instance.AddValue(-50);
            FailNote();
            return;
        }

        // 3. 일반적인 노트 이동
        UpdatePosition(beatDiff);
    }

    private void HandleHoldingEffect(float currentBeat)
    {
        // 머리 고정 및 꼬리 줄어드는 연출 (가이드/플레이어 공통)
        rectTransform.anchoredPosition = targetReceptor.anchoredPosition;
        if (bodyRect != null)
        {
            float remainingBeat = Mathf.Max(0, (Data.hitTime + Data.duration) - currentBeat);
            bodyRect.sizeDelta = new Vector2(bodyRect.sizeDelta.x, remainingBeat * noteSpeed);
        }
    }

    private void HandleLongNoteScoring(float currentBeat)
    {
        float beatDelta = currentBeat - lastCheckedBeat;
        holdScoreTimer += beatDelta;
        lastCheckedBeat = currentBeat;

        if (holdScoreTimer >= 0.5f) // 0.5박자마다 점수 (더 촘촘하게)
        {
            holdScoreTimer -= 0.5f;
            longNoteTickCount++;
            N_44LifeSlider.Instance.AddValue(5); // 틱당 점수
            if (judgeEffectManager != null) judgeEffectManager.ShowJudge("perfect");
        }
    }

    public void ProcessLongNoteEndMiss()
    {
        // 롱노트 끝까지 못 채우고 지나갔을 때
        float fillRatio = (float)longNoteTickCount / (Data.duration * 2); // 0.5틱 기준
        if (fillRatio >= 0.8f) 
            N_44LifeSlider.Instance.AddValue(-40);
        else 
            N_44LifeSlider.Instance.AddValue(-50);

        if (judgeEffectManager != null) judgeEffectManager.ShowJudge("miss");
        FailNote();
    }

    public void StartHolding()
    {
        IsHolding = true;
        lastCheckedBeat = gameManager.GetBeatTime();

        // 플레이어용 사운드/이미지 처리
        if (Data.isPlayerNote)
        {
            if (sfx != null && sfx.HoldSound != null && SoundManager.Instance != null)
                holdSFXSource = SoundManager.Instance.PlayLoopingSFX(sfx.HoldSound);

            Image headImage = GetComponent<Image>();
            if (headImage != null) headImage.enabled = false;
        }
    }

    private void UpdatePosition(float beatDiff)
    {
        if (targetReceptor != null)
        {
            float yOffset = beatDiff * noteSpeed;
            rectTransform.anchoredPosition = targetReceptor.anchoredPosition - new Vector2(0, yOffset);
        }
    }

    public int GetTickCount() => longNoteTickCount;

    public void StopHoldSFX()
    {
        if (holdSFXSource != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.StopLoopingSFX(holdSFXSource);
            holdSFXSource = null;
        }
    }

    public void FailNote()
    {
        IsFailed = true;
        StopHoldSFX();
        // InputManager 리스트에서 제거 요청
        inputManager.RemoveNote(this, (int)Data.direction);
    }

    private void OnDestroy() => StopHoldSFX();
}