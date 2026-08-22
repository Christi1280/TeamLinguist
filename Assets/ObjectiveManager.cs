using TMPro;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    [Header("Objective UI")]
    public TMP_Text objectiveText;
    public TMP_Text playerPageObjectiveText;

    [Header("Starting Objective")]
    public string startingObjective = "SPEAK  TO  MATEO";

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
    }

    private void Start()
    {
        if (GameProgressManager.Instance == null)
        {
            Debug.LogWarning(
                "ObjectiveManager: GameProgressManager was not found."
            );

            return;
        }

        // Only initialize the objective if one does not already exist.
        if (string.IsNullOrEmpty(
                GameProgressManager.Instance.CurrentObjective))
        {
            GameProgressManager.Instance.SetObjective(
                startingObjective
            );
        }

        UpdateObjectiveUI();
    }

    public void SetObjective(string newObjective)
    {
        if (GameProgressManager.Instance == null)
        {
            Debug.LogWarning(
                "ObjectiveManager: GameProgressManager was not found."
            );

            return;
        }

        GameProgressManager.Instance.SetObjective(
            newObjective
        );

        UpdateObjectiveUI();
    }

    public void UpdateObjectiveUI()
    {
        if (GameProgressManager.Instance == null)
        {
            return;
        }

        string objective =
            GameProgressManager.Instance.CurrentObjective;

        if (objectiveText != null)
        {
            objectiveText.text = objective;
        }

        if (playerPageObjectiveText != null)
        {
            playerPageObjectiveText.text = objective;
        }
    }

    public void SetVisitRestaurantObjective()
    {
        SetObjective(
            "VISIT  THE  RESTAURANT"
        );
    }

    public void SetFollowLauraObjective()
    {
        SetObjective(
            "FOLLOW  LAURA"
        );
    }

    public void SetTakeASeatObjective()
    {
        SetObjective(
            "TAKE  A  SEAT"
        );
    }
}