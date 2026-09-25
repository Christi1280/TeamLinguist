using TMPro;
using UnityEngine;

public class ModuleProgressManager : MonoBehaviour
{
    public static ModuleProgressManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TMP_Text moduleProgressText;

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
        UpdateProgressUI();
    }

    public void SetProgress(int progress)
    {
        if (GameProgressManager.Instance == null)
        {
            Debug.LogWarning(
                "ModuleProgressManager: GameProgressManager was not found."
            );

            return;
        }

        GameProgressManager.Instance.SetModuleProgress(progress);

        UpdateProgressUI();
    }

    public void UpdateProgressUI()
    {
        if (GameProgressManager.Instance == null)
        {
            Debug.LogWarning(
                "ModuleProgressManager: GameProgressManager was not found."
            );

            return;
        }

        if (moduleProgressText != null)
        {
            moduleProgressText.text =
                GameProgressManager.Instance.ModuleProgress + "%";
        }
    }

    // Module 1 milestones

    public void CompleteMateo()
    {
        SetProgress(20);
    }

    public void ReachRestaurant()
    {
        SetProgress(40);
    }

    public void CompleteLaura()
    {
        SetProgress(60);
    }

    public void BeginDiegoScenario()
    {
        SetProgress(80);
    }

    public void CompleteModule()
    {
        SetProgress(100);
    }
}