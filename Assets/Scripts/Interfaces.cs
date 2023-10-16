using UnityEngine;

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
        void PlaySound(AudioClip audioClip);
    }
}