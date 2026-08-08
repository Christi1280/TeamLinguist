using TMPro;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    [Header("Objective UI")]
    public TMP_Text objectiveText;

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
        }
    }

    private void Start()
    {
        SetObjective(startingObjective);
    }

    public void SetObjective(string newObjective)
    {
        if (objectiveText != null)
        {
            objectiveText.text = newObjective;
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