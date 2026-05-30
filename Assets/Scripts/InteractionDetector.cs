using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null; //closest interactable object in range

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)) //for testing purposes, we can use the E key to interact with objects in range
        {
            if (interactableInRange != null)
            {
                interactableInRange.Interact();

                Debug.Log("in range");
            }

            Debug.Log("interact");
        }


    }


    private void OnTriggerEnter2D(Collider2D collision) //check if the object we collided with is interactable and if so, set it as the current interactable in range
    {
        if(collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;

            Debug.Log("interactable");
        }
    }

    private void OnTriggerExit2D(Collider2D collision) //clear the interactable in range if we exit its trigger area
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
        }
    }

}
