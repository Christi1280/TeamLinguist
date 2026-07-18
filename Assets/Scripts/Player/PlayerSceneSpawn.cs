using System.Collections;
using UnityEngine;

public class PlayerSceneSpawn : MonoBehaviour
{
    private const string DestinationKey = "DestinationSpawnPoint";

    private IEnumerator Start()
    {
        // Wait until SaveController has finished its Start method.
        yield return null;

        if (!PlayerPrefs.HasKey(DestinationKey))
        {
            yield break;
        }

        string destinationName =
            PlayerPrefs.GetString(DestinationKey);

        GameObject destination =
            GameObject.Find(destinationName);

        if (destination == null)
        {
            Debug.LogError(
                $"PlayerSceneSpawn could not find a spawn point named " +
                $"'{destinationName}' in scene '{gameObject.scene.name}'."
            );

            PlayerPrefs.DeleteKey(DestinationKey);
            yield break;
        }

        Rigidbody2D playerRigidbody =
            GetComponent<Rigidbody2D>();

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.position =
                destination.transform.position;
        }
        else
        {
            transform.position =
                destination.transform.position;
        }

        PlayerPrefs.DeleteKey(DestinationKey);

        Debug.Log(
            $"Player moved to spawn point '{destinationName}'."
        );
    }
}