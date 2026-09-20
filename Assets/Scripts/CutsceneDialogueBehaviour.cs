using UnityEngine;
using UnityEngine.Playables;

public class CutsceneDialogueBehaviour : PlayableBehaviour
{
    public NPCDialogue dialogueData;
    public int dialogueIndex;

    private bool hasStarted;

    public override void OnBehaviourPlay(
        Playable playable,
        FrameData info)
    {
        if (hasStarted)
        {
            return;
        }

        hasStarted = true;

        if (dialogueData == null)
        {
            return;
        }

        if (DialogueController.Instance == null)
        {
            return;
        }

        DialogueController.Instance.ShowCutsceneDialogue(
            dialogueData,
            dialogueIndex
        );
    }

    public override void OnBehaviourPause(
        Playable playable,
        FrameData info)
    {
        hasStarted = false;
    }
}