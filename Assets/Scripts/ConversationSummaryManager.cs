using TMPro;
using UnityEngine;

public class ConversationSummaryManager : MonoBehaviour
{
    public static ConversationSummaryManager Instance { get; private set; }

    [Header("UI")]
    public GameObject summaryPanel;
    public Transform phraseContainer;
    public GameObject phrasePrefab;

    [Header("Dialogue")]
    public NPCDialogue mateoDialogue;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        summaryPanel.SetActive(false);
    }

    public void ShowSummary(NPCDialogue dialogueData)
    {
        if (dialogueData == null)
        {
            return;
        }

        // Remove phrases from the previous conversation.
        foreach (Transform child in phraseContainer)
        {
            Destroy(child.gameObject);
        }

        // Generate a row for each key phrase.
        if (dialogueData.keyPhrases != null)
        {
            foreach (KeyPhrase phrase in dialogueData.keyPhrases)
            {
                GameObject phraseObject =
                    Instantiate(phrasePrefab, phraseContainer);

                // Find the Spanish and English text objects.
                TMP_Text spanishText =
                    phraseObject.transform
                        .Find("SpanishText")
                        ?.GetComponent<TMP_Text>();

                TMP_Text englishText =
                    phraseObject.transform
                        .Find("EnglishText")
                        ?.GetComponent<TMP_Text>();

                // Fill in the phrase.
                if (spanishText != null)
                {
                    spanishText.text = phrase.spanish;
                }

                if (englishText != null)
                {
                    englishText.text = phrase.english;
                }
            }
        }

        summaryPanel.SetActive(true);

        PauseController.SetPause(true);
    }

    public void ShowMateoSummary()
    {
        ShowSummary(mateoDialogue);
    }

    public void LearnMateoPhrases()
    {
        if (GameProgressManager.Instance == null)
        {
            Debug.LogWarning(
                "ConversationSummaryManager: GameProgressManager was not found."
            );

            return;
        }

        GameProgressManager.Instance.LearnPhrases(mateoDialogue);
    }

    public void CloseSummary()
    {
        summaryPanel.SetActive(false);

        PauseController.SetPause(false);
    }
}