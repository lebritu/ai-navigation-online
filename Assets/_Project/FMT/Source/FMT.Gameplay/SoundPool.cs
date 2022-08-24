using System.Collections;
using System.Collections.Generic;
using FMT.Data;
using System;
using UnityEngine;

namespace FMT.Gameplay
{
    public class SoundPool : MonoBehaviour
    {
        private static AudioSource _musicOutput;

        [SerializeField]
        private AudioClipsLibraryData _audioClipsLibraryData;

        public static float Volume { get; set; }

        public static AudioClipsLibraryData AudioLibrary { get; private set; }
        public static StepAudioClipGroup WoodStep { get; private set; }
        public static StepAudioClipGroup MetalStep { get; private set; }
        public static StepAudioClipGroup TileStep { get; private set; }
        public static StepAudioClipGroup ConcreteStep { get; private set; }
        public static StepAudioClipGroup GrassStep { get; private set; }

        private static bool IsPlaying { get; set; }

        protected void Start()
        {
            if (IsPlaying)
            {
                Destroy(gameObject);
            }
            else
            {
                AudioLibrary = _audioClipsLibraryData;
                IsPlaying = true;
                Volume = 0.2f;
                _musicOutput = GetComponent<AudioSource>();
                SetupAudioClips();
                DontDestroyOnLoad(gameObject);
            }
        }

        public static void PlayMusicClip(AudioClip clip)
        {
            _musicOutput.clip = clip;
            _musicOutput.volume = Volume;
            _musicOutput.Play();
        }

        public static AudioClip GetStepSound(MaterialEnum material)
        {
            switch (material)
            {
                case MaterialEnum.Wood:
                    return WoodStep.GetRandomClip();

                case MaterialEnum.Metal:
                    return MetalStep.GetRandomClip();

                case MaterialEnum.Tile:
                    return TileStep.GetRandomClip();

                case MaterialEnum.Grass:
                    return GrassStep.GetRandomClip();

                case MaterialEnum.Concrete:
                    return ConcreteStep.GetRandomClip();

                default:
                    return GrassStep.GetRandomClip();
            }
        }

        private void SetupAudioClips()
        {
            WoodStep = _audioClipsLibraryData.WoodStep;
            MetalStep = _audioClipsLibraryData.MetalStep;
            TileStep = _audioClipsLibraryData.TileStep;
            ConcreteStep = _audioClipsLibraryData.ConcreteStep;
            GrassStep = _audioClipsLibraryData.GrassStep;
        }
    }
}
