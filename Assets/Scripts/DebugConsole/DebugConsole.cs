using System;
using System.Linq;
using TMPro;
using UnityEngine;
using ScrollShop.Interfaces;

namespace ScrollShop.Debug
{
    public class DebugConsole : MonoBehaviour
    {
        // Singleton
        private static DebugConsole _instance = null;
        public static DebugConsole Instance => _instance;

        // Fields
        [SerializeField] GameObject _consoleDebugLine;
        [SerializeField] RectTransform _consoleContent;
        [SerializeField] TMP_InputField _consoleInput;
        [SerializeField] string[] _commands;
        
        IDebug[] _debugables;

        // Public methods
        public void Print(string message, LOGTYPE logType = LOGTYPE.LOG)
        {
            switch (logType)
            {
                case LOGTYPE.LOG:
                    Print(message, Color.white);
                    break;
                
                case LOGTYPE.WARNING:
                    Print(message, Color.yellow);
                    break;
                
                case LOGTYPE.ERROR:
                    Print(message, Color.red);
                    break;
                
                default:
                    Print(message, Color.white);
                    break;
            }
        }
        
        public void Print(string message, Color color)
        {
            CreateDebugLine(message, color);
            ResizeContentBox();
        }
    
        // Private methods
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                _instance = this;
            }
            
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            _consoleInput.onEndEdit.AddListener(DoCommand);
        }

        private void OnDestroy()
        {
            _consoleInput.onEndEdit.RemoveListener(DoCommand);
        }

        private void ToggleConsole()
        {
            GameObject child = transform.GetChild(0).gameObject;
            child.SetActive(!child.activeSelf);
        }

        private void DoCommand(string command)
        {
            if (!_commands.Contains(command))
            {
                Print("DebugConsole: \"" + command + "\" is not a valid command.", Color.white);
                return;
            }
        }
        
        private void ResizeContentBox(){}

        private void CreateDebugLine(string message, Color color)
        {
            
        }
    }

    public enum LOGTYPE
    {
        LOG,
        WARNING,
        ERROR
    }
}