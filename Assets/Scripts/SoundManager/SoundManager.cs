using UnityEngine;

namespace ScrollShop.Interfaces
{
    public class SoundManager : MonoBehaviour, ISoundManager
    {
        public void PlaySound(AudioClip audioClip)
        {
            if (audioClip == null) return;
            
            if (gameObject.GetComponent<AudioSource>() == null)
            {
                gameObject.AddComponent<AudioSource>();   
            }

            var audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.PlayOneShot(audioClip);
        }
    }
}
