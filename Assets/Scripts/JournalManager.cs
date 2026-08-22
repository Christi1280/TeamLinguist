using TMPro;
using UnityEngine;

public class JournalManager : MonoBehaviour
{
    [Header("Journal UI")]
    [SerializeField] private Transform phraseContainer;
    [SerializeField] private GameObject phrasePrefab;

    private void Start()
    {
        RefreshJournal();
    }

    public void RefreshJournal()
    {
        if (phraseContainer == null ||
            phrasePrefab == null)
        {
            Debug.LogWarning(
                "JournalManager: Journal UI references are missing."
            );

            return;
        }

        // Remove currently displayed rows.
        foreach (Transform child in phraseContainer)
        {
            Destroy(child.gameObject);
        }

        if (GameProgressManager.Instance == null)
        {
            Debug.LogWarning(
                "JournalManager: GameProgressManager was not found."
            );

            return;
        }

        // Create one row for every learned phrase.
        foreach (KeyPhrase phrase
                 in GameProgressManager.Instance.LearnedPhrases)
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

        Debug.Log(
            "Journal refreshed. Phrase count: " +
            GameProgressManager.Instance.LearnedPhrases.Count
        );
    }
}