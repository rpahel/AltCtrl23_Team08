using UnityEngine;

namespace ScrollShop.Interfaces
{
    public class SoundManagerConstructor : MonoBehaviour
    {
        #region Unity Event Function
        
        private void Awake()
        {
            ServiceLocator.Initialize();

            ISoundManager soundManager = gameObject.AddComponent<SoundManager>();
            gameObject.AddComponent<AudioSource>();
            ServiceLocator.Provide(soundManager);
        }

        #endregion
    }
}
