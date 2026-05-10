using System.Threading.Tasks;
using UnityEngine;
using PrimeTween;

public class MenuPage : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField]
    private float enterDuration = 0.5f;

    [SerializeField]
    private float exitDuration = 0.35f;

    [SerializeField]
    private float stagger = 0.08f;


    [Header("Buttons")]
    [SerializeField]
    private RectTransform[] buttonArray;


    private Vector2[] originalPositionArray;
    private CanvasGroup[] canvasGroupArray;
    private bool isInitialized;


    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (isInitialized)
            return;

        if (buttonArray == null || buttonArray.Length == 0)
        {
            Debug.LogWarning($"{name}: No buttons assigned.", this);
            return;
        }

        originalPositionArray = new Vector2[buttonArray.Length];
        canvasGroupArray = new CanvasGroup[buttonArray.Length];

        for (int i = 0; i < buttonArray.Length; i++)
        {
            RectTransform button = buttonArray[i];

            if (button == null)
            {
                originalPositionArray[i] = Vector2.zero;
                Debug.LogWarning($"{name}: Button at index {i} is null.", this);
                continue;
            }

            originalPositionArray[i] = button.anchoredPosition;
            button.TryGetComponent(out canvasGroupArray[i]);
        }

        gameObject.SetActive(false);
        isInitialized = true;

        OnInit();
    }

    public async Task Enter()
    {
        Initialize();

        gameObject.SetActive(true);
        PrepareEnterState();

        Sequence sequence = BuildEnterSequence();
        await sequence;

        FinalizeEnterState();
    }

    public async Task Exit()
    {
        Initialize();
        PrepareExitState();

        Sequence sequence = BuildExitSequence();
        await sequence;

        ResetVisualState();
        gameObject.SetActive(false);
    }

    private void PrepareEnterState()
    {
        for (int i = 0; i < buttonArray.Length; i++)
        {
            RectTransform button = buttonArray[i];

            if (button == null)
                continue;

            Rect rect = button.rect;
            button.anchoredPosition = originalPositionArray[i] - new Vector2(rect.width, 0.0f);

            CanvasGroup cg = canvasGroupArray[i];

            if (cg != null)
            {
                cg.alpha = 0.0f;
                cg.blocksRaycasts = false;
            }
        }
    }

    private void FinalizeEnterState()
    {
        for (int i = 0; i < buttonArray.Length; i++)
        {
            RectTransform button = buttonArray[i];

            if (button == null)
                continue;

            button.anchoredPosition = originalPositionArray[i];

            CanvasGroup cg = canvasGroupArray[i];

            if (cg != null)
            {
                cg.alpha = 1.0f;
                cg.blocksRaycasts = true;
            }
        }
    }

    private void PrepareExitState()
    {
        for (int i = 0; i < canvasGroupArray.Length; i++)
        {
            CanvasGroup cg = canvasGroupArray[i];

            if (cg == null)
                continue;

            cg.blocksRaycasts = false;
        }
    }

    private void ResetVisualState()
    {
        for (int i = 0; i < buttonArray.Length; i++)
        {
            RectTransform button = buttonArray[i];

            if (button == null)
                continue;

            button.anchoredPosition = originalPositionArray[i];

            CanvasGroup cg = canvasGroupArray[i];

            if (cg != null)
            {
                cg.alpha = 1.0f;
                cg.blocksRaycasts = true;
            }
        }
    }

    private Sequence BuildEnterSequence()
    {
        Sequence sequence = Sequence.Create();

        for (int i = 0; i < buttonArray.Length; i++)
        {
            RectTransform button = buttonArray[i];

            if (button == null)
                continue;

            float delay = i * stagger;

            _ = sequence.Group(
                Tween.UIAnchoredPosition(
                    button,
                    originalPositionArray[i],
                    enterDuration,
                    ease: Ease.OutSine,
                    startDelay: delay
                )
            );

            CanvasGroup cg = canvasGroupArray[i];

            if (cg != null)
            {
                _ = sequence.Group(
                    Tween.Alpha(
                        cg,
                        1.0f,
                        enterDuration,
                        ease: Ease.InSine,
                        startDelay: delay
                    )
                );
            }
        }

        return sequence;
    }

    private Sequence BuildExitSequence()
    {
        Sequence sequence = Sequence.Create();

        for (int i = buttonArray.Length - 1; i >= 0; i--)
        {
            RectTransform button = buttonArray[i];

            if (button == null)
                continue;

            float delay = (buttonArray.Length - 1 - i) * stagger;

            Rect rect = button.rect;

            _ = sequence.Group(
                Tween.UIAnchoredPosition(
                    button,
                    originalPositionArray[i] + new Vector2(rect.width, 0.0f),
                    exitDuration,
                    ease: Ease.InSine,
                    startDelay: delay
                )
            );

            CanvasGroup cg = canvasGroupArray[i];

            if (cg != null)
            {
                _ = sequence.Group(
                    Tween.Alpha(
                        cg,
                        0.0f,
                        exitDuration,
                        ease: Ease.OutSine,
                        startDelay: delay
                    )
                );
            }
        }

        return sequence;
    }

    protected virtual void OnInit() { }
}
