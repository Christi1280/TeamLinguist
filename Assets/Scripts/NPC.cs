using System.Collections;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText, nameText;
    //maybe add portrait image variable here later

    private int dialogueIndex; //tracks which line of dialogue we're on
    private bool isTyping, isDialogueActive;

    public bool CanInteract()
    {
        return !isDialogueActive; //can only interact if dialogue isn't already active
    }

    public void Interact()
    {
        if (dialogueData == null)
            return;

        if (isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;
        nameText.SetText(dialogueData.npcName);
        //set portrait image if decide to add later

        dialoguePanel.SetActive(true);
        //pause game, add later when we have a game manager or something to handle that

        StartCoroutine(TypeLine());
    }

    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]); //auto-complete the line if player tries to skip while it's still typing
            isTyping = false;
        }
        else if(++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            StartCoroutine(TypeLine()); //if another line, type it out
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");

        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        if(dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        } 
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
        //unpause game, add later when we have a game manager or something to handle that
    }

    public void RightAnswer() //add script to right answer button
    {
        //add functionality for right answer for dialogue choices
        NextLine();
    }

    public void WrongAnswer() //add script to wrong answer button
    {
        //repeat line
        if (dialogueIndex > 0)
            dialogueIndex--;
        NextLine();
    }
}
