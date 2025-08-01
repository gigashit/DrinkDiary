using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class FloatInputFieldToTop : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private RectTransform rectTransform;

    // Original layout values
    private RectTransform originalParent;
    private int originalSiblingIndex;
    private Vector2 originalAnchorMin;
    private Vector2 originalAnchorMax;
    private Vector2 originalPivot;
    private Vector2 originalAnchoredPosition;
    private Vector2 originalSizeDelta;
    private Vector3 originalScale;

    public RectTransform floatingCanvasTarget; // Top-level UI target
    public GameObject dimmerPanel;
    public Vector2 floatingPosition = new Vector2(0, 0); // Center + above keyboard

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        dimmerPanel.SetActive(true);

        // Save original layout values
        originalParent = (RectTransform)rectTransform.parent;
        originalSiblingIndex = rectTransform.GetSiblingIndex();
        originalAnchorMin = rectTransform.anchorMin;
        originalAnchorMax = rectTransform.anchorMax;
        originalPivot = rectTransform.pivot;
        originalAnchoredPosition = rectTransform.anchoredPosition;
        originalSizeDelta = rectTransform.sizeDelta;
        originalScale = rectTransform.localScale;

        // Move to floating canvas
        rectTransform.SetParent(floatingCanvasTarget, false); // keep local pos

        // Set anchor to center
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Position in center
        rectTransform.anchoredPosition = floatingPosition;
        rectTransform.localScale = new Vector3(2f, 2f, 2f);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        dimmerPanel.SetActive(false);

        // Restore parent and layout values
        rectTransform.SetParent(originalParent, false);
        rectTransform.SetSiblingIndex(originalSiblingIndex);

        rectTransform.anchorMin = originalAnchorMin;
        rectTransform.anchorMax = originalAnchorMax;
        rectTransform.pivot = originalPivot;
        rectTransform.anchoredPosition = originalAnchoredPosition;
        rectTransform.sizeDelta = originalSizeDelta;
        rectTransform.localScale = originalScale;
    }
}
