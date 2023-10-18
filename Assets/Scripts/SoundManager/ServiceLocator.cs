namespace ScrollShop.Interfaces
{
    public static class ServiceLocator
    {
        #region Fields

        private static ISoundManager _soundManager;
        private static NullSoundManager _nullSoundManager;

        #endregion

        #region private Methods

        public static void Initialize()
        {
            _soundManager = _nullSoundManager;
        }

        public static void Provide(ISoundManager soundManager)
        {
            if (soundManager == null)
            {
                _soundManager = _nullSoundManager;
            }
            
            _soundManager = soundManager;
        }

        public static ISoundManager Get()
        {
            return _soundManager;
        }

        #endregion
    }
}
