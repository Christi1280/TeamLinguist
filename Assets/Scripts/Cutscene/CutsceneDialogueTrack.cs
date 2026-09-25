using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// Defines a custom Timeline track that holds CutsceneDialogueClips.
/// </summary>

[TrackClipType(typeof(CutsceneDialogueClip))]
public class CutsceneDialogueTrack : TrackAsset
{
}