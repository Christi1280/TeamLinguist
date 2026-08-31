using TMPro;
using UnityEngine;

public class ConversationSummaryManager : MonoBehaviour
{
    public static ConversationSummaryManager Instance { get; private set; }

    [Header("UI")]
    public GameObject summaryPanel;
    public Transform phraseContainer;
    public GameObject phrasePrefab;

    [Header("Mateo Dialogue")]
    public NPCDialogue mateoDialogue;

    [Header("Laura Dialogues")]
    public NPCDialogue lauraDialogue;
    public NPCDialogue lauraTableDialogue;

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

    // Shows the summary for one dialogue.
    public void ShowSummary(NPCDialogue dialogueData)
    {
        if (dialogueData == null)
        {
            return;
        }

        ClearSummary();

        AddDialoguePhrasesToSummary(dialogueData);

        summaryPanel.SetActive(true);

        PauseController.SetPause(true);
    }

    // Shows one combined summary using multiple dialogues.
    public void ShowCombinedSummary(
        NPCDialogue firstDialogue,
        NPCDialogue secondDialogue)
    {
        ClearSummary();

        AddDialoguePhrasesToSummary(firstDialogue);
        AddDialoguePhrasesToSummary(secondDialogue);

        summaryPanel.SetActive(true);

        PauseController.SetPause(true);
    }

    private void ClearSummary()
    {
        foreach (Transform child in phraseContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void AddDialoguePhrasesToSummary(
        NPCDialogue dialogueData)
    {
        if (dialogueData == null ||
            dialogueData.keyPhrases == null)
        {
            return;
        }

        foreach (KeyPhrase phrase in dialogueData.keyPhrases)
        {
            GameObject phraseObject =
                Instantiate(
                    phrasePrefab,
                    phraseContainer
                );

            TMP_Text spanishText =
                phraseObject.transform
                    .Find("SpanishText")
                    ?.GetComponent<TMP_Text>();

            TMP_Text englishText =
                phraseObject.transform
                    .Find("EnglishText")
                    ?.GetComponent<TMP_Text>();

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

    // --------------------
    // MATEO
    // --------------------

    public void ShowMateoSummary()
    {
        ShowSummary(mateoDialogue);
    }

    public void LearnMateoPhrases()
    {
        if (GameProgressManager.Instance == null)
        {
            Debug.LogWarning(
                "ConversationSummaryManager: " +
                "GameProgressManager was not found."
            );

            return;
        }

        GameProgressManager.Instance.LearnPhrases(
            mateoDialogue
        );
    }

    // --------------------
    // LAURA
    // --------------------

    public void ShowLauraSummary()
    {
        ShowCombinedSummary(
            lauraDialogue,
            lauraTableDialogue
        );
    }

    public void LearnLauraTablePhrases()
    {
        if (GameProgressManager.Instance == null)
        {
            Debug.LogWarning(
                "ConversationSummaryManager: " +
                "GameProgressManager was not found."
            );

            return;
        }

        // Laura's first dialogue phrases were already
        // learned after the entrance conversation.
        // We only need to learn the table phrases here.
        GameProgressManager.Instance.LearnPhrases(
            lauraTableDialogue
        );

        JournalManager journalManager =
            FindFirstObjectByType<JournalManager>();

        if (journalManager != null)
        {
            journalManager.RefreshJournal();
        }
    }

    public void CloseSummary()
    {
        summaryPanel.SetActive(false);

        PauseController.SetPause(false);
    }
}