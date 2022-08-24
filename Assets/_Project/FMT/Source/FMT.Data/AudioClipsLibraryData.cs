using System;
using UnityEngine;

namespace FMT.Data
{
    [Serializable]
    public struct StepAudioClipGroup
    {
        public AudioClip[] AudioClips;

        public AudioClip GetRandomClip()
        {
            int randomNumber = UnityEngine.Random.Range(0, AudioClips.Length - 1);

            return AudioClips[randomNumber];
        }
    }

    [CreateAssetMenu(fileName = ("Audio Library"), menuName = ("new Audio Library"))]
    public class AudioClipsLibraryData : ScriptableObject
    {
        public AudioClip Music;

        public StepAudioClipGroup WoodStep;
        public StepAudioClipGroup MetalStep;
        public StepAudioClipGroup TileStep;
        public StepAudioClipGroup ConcreteStep;
        public StepAudioClipGroup GrassStep;
    }
}
