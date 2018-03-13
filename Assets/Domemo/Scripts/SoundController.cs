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
        private AudioClip tileFlip;


        [SerializeField]
        private AudioClip gameBGM;
        [SerializeField]
        private AudioClip menuBGM;

        private AudioSource audioSourceSFX;
        private AudioSource audioSourceBGM;
        List<AudioSource> tempAudioSourceList = new List<AudioSource>();

        public static SoundController instance = null;
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }

            instance = this;

            audioSourceSFX = GetComponent<AudioSource>();
            audioSourceBGM = gameObject.AddComponent<AudioSource>();
            audioSourceBGM.volume = 0.75f;
            audioSourceBGM.loop = true;
            PlayBGM(true);
        }

        public void PlayBGM(bool menu)
        {
            if(menu)
            {
                audioSourceBGM.Stop();
                audioSourceBGM.clip = menuBGM;
                audioSourceBGM.Play();
            }
            else
            {
                audioSourceBGM.Stop();
                audioSourceBGM.clip = gameBGM;
                audioSourceBGM.Play();
            }
        }

        public void PlaySE(eSoundFX fxType, float volume = 1.0f)
        {
            if(GetAudioClip(fxType) != null)
            {
                if(audioSourceSFX.isPlaying)
                {
                    PlayNewAudioSource(GetAudioClip(fxType), volume / 2);
                }
                else
                {
                    audioSourceSFX.PlayOneShot(GetAudioClip(fxType), volume / 2);
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
                    return tileFlip;
            }
            return clip;
        }
    }
}
