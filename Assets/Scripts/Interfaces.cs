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
}