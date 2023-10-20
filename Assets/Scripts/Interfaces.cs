using UnityEngine;
using UnityEngine.Audio;

namespace ScrollShop.Interfaces
{
    interface IDebug
    {
        void SubscribeToDebugConsole();
        void UnsubscribeFromDebugConsole();
    }

    interface IBridgeDependent
    {
        void SubscribeToPoseChangedEvent();
        void UnsubscribeFromPoseChangedEvent();
    }
    
    public interface ISoundManager
    {
        public AudioSource MusicAudioSource { get; set; }
        public AudioSource SfxAudioSource { get; set; }
        public AudioClip Music { get; set; }
        public AudioMixerGroup MusicMixerGroupe { get; set; }
        public AudioMixerGroup SfxMixerGroupe { get; set; }
        
        void ChangeMusic(AudioClip audioClip, bool mainMusic = false);
        void PlaySound(AudioClip audioClip);
        void Reset();
    }
}