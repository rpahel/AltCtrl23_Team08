using UnityEngine;
using UnityEngine.Audio;

namespace ScrollShop.Interfaces
{
    public class SoundManagerConstructor : MonoBehaviour
    {
        #region Fields

        [SerializeField] private AudioMixerGroup _sfxMixerGroup;
        [SerializeField] private AudioMixerGroup _musicMixerGroup;

        #endregion
        
        #region Unity Event Function
        
        private void Awake()
        {
            ServiceLocator.Initialize();

            ISoundManager soundManager = gameObject.AddComponent<SoundManager>();
            soundManager.MusicMixerGroupe = _musicMixerGroup;
            soundManager.SfxMixerGroupe = _sfxMixerGroup;
            
            ServiceLocator.Provide(soundManager);
        }

        #endregion
    }
}
