using System;
using UnityEngine;
using DG.Tweening;

namespace ScrollShop.Interfaces
{
    public class SoundManager : MonoBehaviour, ISoundManager
    {
        #region Fields

        private float _time;

        #endregion
        
        #region Properties

        public AudioSource MusicAudioSource { get; set; }

        public AudioSource SfxAudioSource { get; set; }

        public AudioClip Music { get; set; }

        #endregion

        #region Public Methods

        public void ChangeMusic(AudioClip audioClip, bool mainMusic = false)
        {
            if (audioClip == null) return;
            
            if (MusicAudioSource == null)
            {
                MusicAudioSource = gameObject.AddComponent<AudioSource>();
                MusicAudioSource.loop = true;
            }

            if (!mainMusic)
            {
                _time = MusicAudioSource.time;   
            }

            MusicAudioSource.DOFade(0f, 0.75f).OnComplete(() =>
            {
                MusicAudioSource.clip = audioClip; 
                MusicAudioSource.time = mainMusic ? _time : 0f;
                MusicAudioSource.Play();
                Music = audioClip;

                MusicAudioSource.DOFade(1f, 0.75f);
            });
        }

        public void PlaySound(AudioClip audioClip)
        {
            if (audioClip == null) return;
            
            if (SfxAudioSource == null)
            {
                SfxAudioSource = gameObject.AddComponent<AudioSource>();   
                SfxAudioSource.loop = true;
            }
            
            SfxAudioSource.PlayOneShot(audioClip);
        }

        public void Reset()
        {
            _time = 0f;
        }

        #endregion
    }
}
