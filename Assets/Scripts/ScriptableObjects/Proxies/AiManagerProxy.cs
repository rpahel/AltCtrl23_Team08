using UnityEngine;

namespace ScrollShop.AI
{
    [CreateAssetMenu(fileName = "AiManagerProxy", menuName = "Proxies/AiManagerProxy")]
    public class AiManagerProxy : ScriptableObject
    {
        private AiManager _realAiManager;
        
        public AiManager Manager
        {
            get => _realAiManager;
            set => _realAiManager = value;
        }
    }
}
