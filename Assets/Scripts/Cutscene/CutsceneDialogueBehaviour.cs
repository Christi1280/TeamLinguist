using UnityEngine;
using UnityEngine.Playables;

public class CutsceneDialogueBehaviour : PlayableBehaviour
{
    public NPCDialogue dialogueData;
    public int dialogueIndex;

    public bool closeDialogue;

    private bool hasStarted;

    public bool usePhoneEffect;

    public override void OnBehaviourPlay(
        Playable playable,
        FrameData info)
    {
        if (hasStarted)
        {
            return;
        }

        hasStarted = true;

        if (DialogueController.Instance == null)
        {
            return;
        }

        if (dialogueData == null)
        {
            return;
        }

        DialogueController.Instance.ShowCutsceneDialogue(
            dialogueData,
            dialogueIndex,
            usePhoneEffect
        );
    }

    public override void OnBehaviourPause(
        Playable playable,
        FrameData info)
    {
        if (closeDialogue && DialogueController.Instance != null)
        {
            DialogueController.Instance.HideCutsceneDialogue();
        }

        hasStarted = false;
    }
}