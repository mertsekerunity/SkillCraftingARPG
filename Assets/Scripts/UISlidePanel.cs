using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIPanelSlider : MonoBehaviour
{
    [Header("Panel Settings")]
    [SerializeField] private RectTransform panelRectTransform;
    [SerializeField] private float slideSpeed = 10f;
    [SerializeField] private KeyCode activationKey = KeyCode.Tab;

    [Header("Position Settings")]
    [SerializeField] private float hiddenPositionX = -300f;
    [SerializeField] private float visiblePositionX = 0f;

    [Header("Optional Animation Settings")]
    [SerializeField] private bool useEasing = true;
    [SerializeField] private AnimationCurve easingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Vector2 targetPosition;
    private Coroutine slideCoroutine;
    private bool isSliding = false;

    private void Start()
    {
        // If no panel is assigned, use this object's RectTransform
        if (panelRectTransform == null)
        {
            panelRectTransform = GetComponent<RectTransform>();
        }

        // Initialize the panel in the hidden position
        Vector2 initialPosition = panelRectTransform.anchoredPosition;
        initialPosition.x = hiddenPositionX;
        panelRectTransform.anchoredPosition = initialPosition;

        targetPosition = initialPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(activationKey))
        {
            SetTargetPosition(visiblePositionX);
        }
        else if (Input.GetKeyUp(activationKey))
        {
            SetTargetPosition(hiddenPositionX);
        }
    }

    private void SetTargetPosition(float xPosition)
    {
        targetPosition = panelRectTransform.anchoredPosition;
        targetPosition.x = xPosition;

        if (isSliding && slideCoroutine != null)
        {
            StopCoroutine(slideCoroutine);
        }

        slideCoroutine = StartCoroutine(SlidePanelToTarget());
    }

    private IEnumerator SlidePanelToTarget()
    {
        isSliding = true;

        Vector2 startPosition = panelRectTransform.anchoredPosition;
        float distance = Mathf.Abs(targetPosition.x - startPosition.x);

        float duration = distance / (slideSpeed * 100f);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / duration);

            // Apply easing if enabled
            float easedProgress = useEasing ? easingCurve.Evaluate(normalizedTime) : normalizedTime;

            // Calculate the current position
            Vector2 newPosition = startPosition;
            newPosition.x = Mathf.Lerp(startPosition.x, targetPosition.x, easedProgress);

            // Apply the new position
            panelRectTransform.anchoredPosition = newPosition;

            yield return null;
        }

        // Ensure the panel ends exactly at the target position
        panelRectTransform.anchoredPosition = targetPosition;
        isSliding = false;
    }
}
