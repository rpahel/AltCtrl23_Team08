using UnityEngine;

namespace ScrollShop.Interfaces
{
    public class NullSoundManager : ISoundManager
    {
        public AudioSource MusicAudioSource
        {
            get => throw new System.NotImplementedException();
            set => throw new System.NotImplementedException();
        }

        public AudioSource SfxAudioSource
        {
            get => throw new System.NotImplementedException();
            set => throw new System.NotImplementedException();
        }

        public AudioClip Music
        {
            get => throw new System.NotImplementedException();
            set => throw new System.NotImplementedException();
        }

        public void ChangeMusic(AudioClip audioClip, bool mainMusic = false){}

        public void PlaySound(AudioClip audioClip){}
    }
}
