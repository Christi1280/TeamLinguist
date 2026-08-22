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

    public string CurrentObjective { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetObjective(startingObjective);
    }

    public void SetObjective(string newObjective)
    {
        CurrentObjective = newObjective;

        UpdateObjectiveUI();
    }

    private void UpdateObjectiveUI()
    {
        // Existing objective display.
        if (objectiveText != null)
        {
            objectiveText.text = CurrentObjective;
        }

        // Player page objective display.
        if (playerPageObjectiveText != null)
        {
            playerPageObjectiveText.text = CurrentObjective;
        }
    }

    public void SetVisitRestaurantObjective()
    {
        SetObjective("VISIT  THE  RESTAURANT");
    }

    public void SetFollowLauraObjective()
    {
        SetObjective("FOLLOW  LAURA");
    }

    public void SetTakeASeatObjective()
    {
        SetObjective("TAKE  A  SEAT");
    }
}