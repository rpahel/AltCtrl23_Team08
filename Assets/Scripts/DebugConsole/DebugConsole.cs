using System;
using System.Collections.Generic;
using ScrollShop.Enums;
using TMPro;
using UnityEngine;

namespace ScrollShop.CustomDebug
{
    public class DebugConsole : MonoBehaviour
    {
        //== Singleton ====================================================
        private static DebugConsole _instance = null;
        public static DebugConsole Instance => _instance;

        //== Fields =======================================================
        [SerializeField] private KeyCode _consoleKey;
        [SerializeField] private GameObject _consoleDebugLine;
        [SerializeField] private RectTransform _consoleContent;
        [SerializeField] private TMP_InputField _consoleInput;
        private Dictionary<string, Action<string>> _methodsDic = new Dictionary<string, Action<string>>();

        //== Public methods ===============================================
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
           if(color == Color.red)
               Debug.LogError(message);
           else if(color == Color.yellow)
               Debug.LogWarning(message);
           else
               Debug.Log(message);
            
           CreateDebugLine(message, color);
           ResizeContentBox();
        }
        
        public void AddToMethodDictionary(Action<string> method)
        {
            _methodsDic.Add(method.Method.Name.ToLower(), method);
        }
        
        public void RemoveFromMethodDictionary(Action<string> method)
        {
            _methodsDic.Remove(method.Method.Name.ToLower());
        }
    
        //== Private methods ==============================================
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            
            AddToMethodDictionary(Help);
            AddToMethodDictionary(ConsoleClose);
            AddToMethodDictionary(ConsoleClear);
        }

        private void Start()
        {
            _consoleInput.onSubmit.AddListener(ClearInputField);
            _consoleInput.onSubmit.AddListener(DoCommand);
        }

        private void Update()
        {
            if(Input.GetKeyDown(_consoleKey))
                ToggleConsole();
        }

        private void OnDestroy()
        {
            _consoleInput.onSubmit.RemoveListener(DoCommand);
            _consoleInput.onSubmit.RemoveListener(ClearInputField);
            _methodsDic.Clear();
        }

        private void ToggleConsole()
        {
            GameObject child = transform.GetChild(0).gameObject;
            child.SetActive(!child.activeSelf);
        }

        private void DoCommand(string command)
        {
            Print("Input : " + command);
            string[] arguments = command.Split(' ');
            
            if (!_methodsDic.ContainsKey(arguments[0]))
            {
                Print("DebugConsole: \"" + arguments[0] + "\" is not a valid command. " +
                      "Write \"help\" to see all available commands.", Color.white);
                return;
            }
            
            _methodsDic[arguments[0]]?.Invoke(command);
        }

        private void ResizeContentBox()
        {
            _consoleContent.sizeDelta = new Vector2(0, 50 * (_consoleContent.childCount + 1));
        }

        private void MoveContentBoxChildrenUp()
        {
            for (int i = 0; i < _consoleContent.childCount; i++)
            {
                _consoleContent.GetChild(i).position += Vector3.up * 50;
            }
        }
        
        private void CreateDebugLine(string message, Color color)
        {
            TMP_Text text = Instantiate(_consoleDebugLine, _consoleContent).GetComponent<TMP_Text>();
            RectTransform rt = text.GetComponent<RectTransform>();
            text.text = message;
            text.color = color;

            MoveContentBoxChildrenUp();
            
            rt.position =
                new Vector3(
                x: rt.position.x,
                y: _consoleContent.position.y
                );
            
            ResizeContentBox();
        }

        private void ClearInputField(string s)
        {
            _consoleInput.text = string.Empty;
            _consoleInput.ActivateInputField();
        }

        private void Help(string s = "")
        {
            foreach (var command in _methodsDic)
            {
                Print(command.Key);
            }
        }

        private void ConsoleClear(string s = "")
        {
            for (int i = _consoleContent.childCount - 1; i >= 0; i--)
            {
                Destroy(_consoleContent.GetChild(i).gameObject);
            }
            
            _consoleContent.sizeDelta = Vector2.zero;
        }

        private void ConsoleClose(string s = "")
        {
            ToggleConsole();
        }
    }
}