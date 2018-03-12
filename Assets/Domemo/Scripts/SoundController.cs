using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBSL_DOMEMO
{
    public enum eSoundFX
    {
        INVALID = -1,
        ToggleSound,
        ConfirmSound,
        CorrectSound,
        WrongSound,
        PlayerTurnSound,
        TileFlipSound,
    }

    public class SoundController : MonoBehaviour
    {
        [SerializeField]
        private AudioClip toggleSound;
        [SerializeField]
        private AudioClip confirmSound;
        [SerializeField]
        private AudioClip correctSound;
        [SerializeField]
        private AudioClip wrongSound;
        [SerializeField]
        private AudioClip playerTurnSound;
        [SerializeField]
        private AudioClip[] tileFlip;

        private AudioSource audioSource;
        List<AudioSource> tempAudioSourceList = new List<AudioSource>();

        public static SoundController instance = null;
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }

            instance = this;

            audioSource = GetComponent<AudioSource>();
        }

        public void PlaySE(eSoundFX fxType, float volume = 1.0f)
        {
            if(GetAudioClip(fxType) != null)
            {
                if(audioSource.isPlaying)
                {
                    PlayNewAudioSource(GetAudioClip(fxType), volume);
                }
                else
                {
                    audioSource.PlayOneShot(GetAudioClip(fxType), volume);
                }
            }
        }

        void PlayNewAudioSource(AudioClip clip, float volume)
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.PlayOneShot(clip, volume);
            tempAudioSourceList.Add(newSource);
            Invoke("RemoveLastAudioSource", clip.length);
        }

        void RemoveLastAudioSource()
        {
            if(tempAudioSourceList.Count > 0)
            {
                Destroy(tempAudioSourceList[0]);
                tempAudioSourceList.RemoveAt(0);
            }
        }

        AudioClip GetAudioClip(eSoundFX fxType)
        {
            AudioClip clip = null;
            switch(fxType)
            {
                case eSoundFX.ToggleSound:
                    return toggleSound;
                case eSoundFX.ConfirmSound:
                    return confirmSound;
                case eSoundFX.CorrectSound:
                    return correctSound;
                case eSoundFX.WrongSound:
                    return wrongSound;
                case eSoundFX.PlayerTurnSound:
                    return playerTurnSound;
                case eSoundFX.TileFlipSound:
                    return tileFlip[0];
            }
            return clip;
        }
    }
}
