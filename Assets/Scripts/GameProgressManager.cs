using System.Collections.Generic;
using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance { get; private set; }

    [Header("Fluency Points")]
    [SerializeField] private int fluencyPoints = 0;

    [Header("Objective")]
    [SerializeField] private string currentObjective = "SPEAK  TO  MATEO";

    [Header("Module Progress")]
    [Range(0, 100)]
    [SerializeField] private int moduleProgress = 0;

    [Header("Journal")]
    [SerializeField]
    private List<KeyPhrase> learnedPhrases =
        new List<KeyPhrase>();

    public int FluencyPoints => fluencyPoints;
    public string CurrentObjective => currentObjective;
    public int ModuleProgress => moduleProgress;
    public IReadOnlyList<KeyPhrase> LearnedPhrases => learnedPhrases;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    // --------------------
    // FLUENCY POINTS
    // --------------------

    public void AddFluencyPoint()
    {
        fluencyPoints++;

        Debug.Log(
            "Fluency Points: " + fluencyPoints
        );
    }

    public void SetFluencyPoints(int points)
    {
        fluencyPoints = Mathf.Max(0, points);
    }

    // --------------------
    // OBJECTIVE
    // --------------------

    public void SetObjective(string newObjective)
    {
        currentObjective = newObjective;
    }

    // --------------------
    // MODULE PROGRESS
    // --------------------

    public void SetModuleProgress(int progress)
    {
        int newProgress =
            Mathf.Clamp(progress, 0, 100);

        // Normal gameplay should never move progress backward.
        if (newProgress <= moduleProgress)
        {
            return;
        }

        moduleProgress = newProgress;

        Debug.Log(
            "Module Progress: " +
            moduleProgress +
            "%"
        );
    }

    // --------------------
    // JOURNAL
    // --------------------

    public void LearnPhrases(NPCDialogue dialogueData)
    {
        if (dialogueData == null ||
            dialogueData.keyPhrases == null)
        {
            return;
        }

        foreach (KeyPhrase phrase in dialogueData.keyPhrases)
        {
            if (phrase == null ||
                string.IsNullOrEmpty(phrase.spanish))
            {
                continue;
            }

            if (HasLearnedPhrase(phrase.spanish))
            {
                continue;
            }

            KeyPhrase learnedPhrase = new KeyPhrase
            {
                spanish = phrase.spanish,
                english = phrase.english
            };

            learnedPhrases.Add(learnedPhrase);

            Debug.Log(
                "Journal phrase learned: " +
                learnedPhrase.spanish
            );
        }
    }

    private bool HasLearnedPhrase(string spanish)
    {
        foreach (KeyPhrase phrase in learnedPhrases)
        {
            if (phrase != null &&
                phrase.spanish == spanish)
            {
                return true;
            }
        }

        return false;
    }

    // --------------------
    // SAVE / LOAD
    // --------------------

    public void LoadProgress(
        int savedFluencyPoints,
        string savedObjective,
        int savedModuleProgress)
    {
        fluencyPoints =
            Mathf.Max(0, savedFluencyPoints);

        currentObjective =
            string.IsNullOrEmpty(savedObjective)
                ? "SPEAK  TO  MATEO"
                : savedObjective;

        moduleProgress =
            Mathf.Clamp(savedModuleProgress, 0, 100);

        Debug.Log(
            "Game progress loaded."
        );
    }

    // --------------------
    // NEW GAME
    // --------------------

    public void ResetProgress()
    {
        fluencyPoints = 0;
        currentObjective = "SPEAK  TO  MATEO";
        moduleProgress = 0;

        learnedPhrases.Clear();
    }
}