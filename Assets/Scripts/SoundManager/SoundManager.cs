using UnityEngine;

namespace ScrollShop.Interfaces
{
    public class SoundManager : MonoBehaviour, ISoundManager
    {
        #region Properties

        public AudioSource MusicAudioSource { get; set; }

        public AudioSource SfxAudioSource { get; set; }

        public AudioClip Music { get; set; }

        #endregion

        #region Public Methods

        public void ChangeMusic(AudioClip audioClip)
        {
            if (audioClip == null) return;
            
            if (MusicAudioSource == null)
            {
                MusicAudioSource = gameObject.AddComponent<AudioSource>();
            }

            if (Music != null)
            {
                MusicAudioSource.Stop();
            }
            
            MusicAudioSource.clip = audioClip;
            MusicAudioSource.Play();

            Music = audioClip;
        }

        public void PlaySound(AudioClip audioClip)
        {
            if (audioClip == null) return;
            
            if (SfxAudioSource == null)
            {
                SfxAudioSource = gameObject.AddComponent<AudioSource>();   
            }
            
            SfxAudioSource.PlayOneShot(audioClip);
        }

        #endregion
    }
}
