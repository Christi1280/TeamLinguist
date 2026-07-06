using TMPro;
using UnityEngine;

public class ObjectivesController : MonoBehaviour
{
    public static ObjectivesController Instance { get; private set; }

    public GameObject objectivesPanel;
    public TextMeshProUGUI objectivesText;
    public string[] objectivesList; // Array of objectives

    public static int currentObjective = 0;

    void Awake()
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
        // Initialize the objectives text
        UpdateObjectivesText();

        objectivesPanel.SetActive(true); // Show the objectives panel at the start

    }

    public void UpdateObjectivesText()
    {
        if (objectivesList[currentObjective] != null)
            objectivesText.text = "New  Objective:  " + objectivesList[currentObjective];
    }

}
