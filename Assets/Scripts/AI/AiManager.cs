using System;
using UnityEngine;

namespace ScrollShop.AI
{
    public class AiManager : MonoBehaviour
    {
        //== Fields ======================================
        [SerializeField] private AiManagerProxy _proxy;
        [SerializeField] private Bridge _bridgeSO;
        public Bridge GetBridge => _bridgeSO;

        //== Private Methods =============================
        private void Awake()
        {
            if(_proxy)
                _proxy.Manager = this;
        }

        private void Start()
        {
            if(_bridgeSO)
                _bridgeSO.Initialize();
        }

        private void OnDestroy()
        {
            if(_bridgeSO)
                _bridgeSO.Terminate();

            if(_proxy && _proxy.Manager == this)
                _proxy.Manager = null;
        }
    }
}
