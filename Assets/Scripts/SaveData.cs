using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    // Scene the player was in when the game was saved.
    public string sceneName;

    // Player position.
    public Vector3 playerPosition;

    // Name of the active Cinemachine map boundary.
    public string mapBoundary;

    // Player progression.
    public int fluencyPoints;

    public string currentObjective;

    public int moduleProgress;

    // Journal phrases the player has learned.
    public List<KeyPhrase> learnedPhrases =
        new List<KeyPhrase>();
}