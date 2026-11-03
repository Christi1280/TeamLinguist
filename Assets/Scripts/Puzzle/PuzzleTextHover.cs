
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleTextHover : MonoBehaviour
{
    [Header("Puzzle Text")]
    [SerializeField] private TMP_Text journalText;

    [Header("Tooltip")]
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TMP_Text tooltipText;
    [SerializeField] private Canvas tooltipCanvas;

    [Header("Definitions")]
    [SerializeField] private HoverDefinition[] hoverDefinitions;

    [Header("Position")]
    [SerializeField] private float verticalOffset = 25f;

    private RectTransform tooltipRect;
    private RectTransform canvasRect;

    private void Start()
    {
        if (tooltipPanel != null)
        {
            tooltipRect = tooltipPanel.GetComponent<RectTransform>();
            tooltipPanel.SetActive(false);
        }

        if (tooltipCanvas != null)
        {
            canvasRect = tooltipCanvas.GetComponent<RectTransform>();
        }
    }

    private void Update()
    {
        if (journalText == null ||
            tooltipPanel == null ||
            tooltipText == null ||
            tooltipCanvas == null ||
            tooltipRect == null ||
            canvasRect == null ||
            hoverDefinitions == null ||
            Mouse.current == null ||
            Camera.main == null)
        {
            return;
        }

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(
            journalText,
            Mouse.current.position.ReadValue(),
            Camera.main
        );

        if (linkIndex == -1)
        {
            tooltipPanel.SetActive(false);
            return;
        }

        TMP_LinkInfo linkInfo =
            journalText.textInfo.linkInfo[linkIndex];

        string key = linkInfo.GetLinkID();

        foreach (HoverDefinition def in hoverDefinitions)
        {
            if (def.key != key)
                continue;

            tooltipText.text = def.definition;
            tooltipPanel.SetActive(true);

            // Get the first and last characters of the phrase.
            int firstIndex = linkInfo.linkTextfirstCharacterIndex;
            int lastIndex = firstIndex + linkInfo.linkTextLength - 1;

            TMP_CharacterInfo first =
                journalText.textInfo.characterInfo[firstIndex];

            TMP_CharacterInfo last =
                journalText.textInfo.characterInfo[lastIndex];

            // Find the horizontal center of the phrase.
            float centerX =
                (first.topLeft.x + last.topRight.x) / 2f;

            // Find the top of the phrase.
            float topY = Mathf.Max(
                first.topLeft.y,
                last.topRight.y
            );

            Vector3 localTextPosition =
                new Vector3(centerX, topY, 0f);

            // Convert the phrase position into world coordinates.
            Vector3 worldPosition =
                journalText.transform.TransformPoint(
                    localTextPosition
                );

            // Convert world coordinates into screen coordinates.
            Vector3 screenPosition =
                Camera.main.WorldToScreenPoint(worldPosition);

            // Convert screen coordinates into Canvas coordinates.
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPosition,
                null,
                out Vector2 localPosition
            );

            // Position the tooltip above the hovered phrase.
            tooltipRect.anchoredPosition =
                localPosition +
                new Vector2(0f, verticalOffset);

            return;
        }

        tooltipPanel.SetActive(false);
    }

    private void OnDisable()
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }
}
