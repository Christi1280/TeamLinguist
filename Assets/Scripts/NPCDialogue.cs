using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    public string npcName;
    //maybe sprite for portrait variable
    public string[] dialogueLines;
    public bool[] autoProgressLines; //true if the line should auto-progress after typing, false if it should wait for player input
    public float autoProgressDelay = 1.5f; //delay before auto-progressing to the next line, if autoProgressLines is true
    public float typingSpeed = 0.05f;

}
