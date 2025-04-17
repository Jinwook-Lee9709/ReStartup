using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioSO", menuName = "ScriptableObejcts/AudioSO", order = 1)]
public class AudioSO : ScriptableObject
{
    [SerializedDictionary, SerializeField] public SerializedDictionary<string, AudioClip> AudioClips;
}