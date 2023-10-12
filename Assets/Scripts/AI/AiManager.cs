using System;
using UnityEngine;

namespace ScrollShop.AI
{
    public class AiManager : MonoBehaviour
    {
        [SerializeField] private Bridge _bridgeSO;

        public Bridge GetBridge => _bridgeSO;

        private void Start()
        {
            if(_bridgeSO)
                _bridgeSO.Initialize();
        }

        private void OnDestroy()
        {
            if(_bridgeSO)
                _bridgeSO.Terminate();
        }
    }
}
