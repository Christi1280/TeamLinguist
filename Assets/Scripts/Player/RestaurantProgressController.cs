using UnityEngine;

public class RestaurantProgressController : MonoBehaviour
{
    private void Start()
    {
        if (GameProgressManager.Instance == null)
        {
            Debug.LogWarning(
                "RestaurantProgressController: " +
                "GameProgressManager was not found."
            );

            return;
        }

        // The player has completed Mateo and has now
        // reached the Restaurant stage.
        if (GameProgressManager.Instance.ModuleProgress == 20)
        {
            GameProgressManager.Instance.SetObjective(
                "TALK  TO  LAURA"
            );

            GameProgressManager.Instance.SetModuleProgress(40);

            // Refresh the objective UI.
            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.UpdateObjectiveUI();
            }

            // Refresh the module progress UI.
            if (ModuleProgressManager.Instance != null)
            {
                ModuleProgressManager.Instance.UpdateProgressUI();
            }

            Debug.Log(
                "Restaurant reached. New objective: TALK TO LAURA. " +
                "Module Progress: 40%."
            );
        }
    }
}