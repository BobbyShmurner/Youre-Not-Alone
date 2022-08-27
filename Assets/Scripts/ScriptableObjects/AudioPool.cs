using UnityEngine;

using System.Collections.Generic;

[CreateAssetMenu(fileName = "AudioPool", menuName = "ScriptableObjects/AudioPool")]
public class AudioPool : ScriptableObject
{
    public List<AudioClip> Clips;
}