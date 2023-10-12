using System;
using System.Collections.Generic;
using NaughtyAttributes;
using ScrollShop.CustomDebug;
using ScrollShop.Enums;
using ScrollShop.Interfaces;
using ScrollShop.ScriptableObjects;
using UnityEngine;
using Pose = ScrollShop.Structs.Pose;

namespace ScrollShop.AI
{
    [CreateAssetMenu(fileName = "BridgeSO", menuName = "ScriptableObjects/BridgeSO")]
    public class Bridge : ScriptableObject, IDebug
    {
        //== Fields ================================
        
        [SerializeField, BoxGroup("Poses")]
        private int _numberOfPlayers = 2;
        [SerializeField, BoxGroup("Poses")]
        private Poses _posesSO;
        [SerializeField, BoxGroup("Debug")]
        private bool _ignoreAi;
        
        private event Action<int, Pose> OnPoseChanged; //Action<index, Pose>
        private Dictionary<string, Pose> _posesDatabase;
        private Pose[] _previousPoses; // Does NOT remember ALL previous poses, but the last previous pose of each player index.
        
        //== Interface implementations =============
        
        public void AddDebugMethodsToDebugConsole()
        {
            if (DebugConsole.Instance == null)
                return;
            
            DebugConsole.Instance.AddToMethodDictionary(BridgeToggleIgnoreAI);
            DebugConsole.Instance.AddToMethodDictionary(BridgeSetPose);
        }
    
        public void RemoveDebugMethodsFromDebugConsole()
        {
            if (DebugConsole.Instance == null)
                return;
            
            DebugConsole.Instance.RemoveFromMethodDictionary(BridgeToggleIgnoreAI);
            DebugConsole.Instance.RemoveFromMethodDictionary(BridgeSetPose);
        }
        
        //== Public methods ========================
        
        public void Initialize()
        {
            AddDebugMethodsToDebugConsole();
            
            if (_posesSO == null || _posesSO.GetPoses == null)
            {
                Print("Bridge: _posesSO is null !", LOGTYPE.ERROR);
                return;
            }
    
            if (_posesSO.GetPoses.Length <= 0)
            {
                Print("Bridge: _posesSO is empty !", LOGTYPE.WARNING);
                return;
            }
            
            // Previous poses initialiser
            _previousPoses = new Pose[_numberOfPlayers];
    
            // Database check;
            if (_posesDatabase == null)
            {
                Print("Bridge: _posesDatabase is null. Building it...");
                FillDatabase();
            }
            else if (_posesDatabase.Count != _posesSO.GetPoses.Length)
            {
                // Database and poses array don't match.
                // Database needs to be rebuilt.
                Print("Bridge: _posesDatabase does not match _posesSO. Rebuilding database...", LOGTYPE.WARNING);
                FillDatabase();
            }
        }
        
        public void Terminate()
        {
            RemoveDebugMethodsFromDebugConsole();

            OnPoseChanged = null;
            _previousPoses = null;
            _posesDatabase.Clear();
        }
        
        //== Private methods =======================
    
        private void FillDatabase()
        {
            _posesDatabase?.Clear();
            _posesDatabase = new Dictionary<string, Pose>();
            foreach (var pose in _posesSO.GetPoses)
                _posesDatabase.Add(pose.GetName.ToLower(), pose);
        }
    
        //== Debug methods =========================
        
        private void BridgeToggleIgnoreAI(string s)
        {
            string[] arguments = s.Split(' ');
            int secondArgument = -1;
            if (arguments.Length > 1 && arguments[1] != null)
                int.TryParse(arguments[1], out secondArgument);
    
            _ignoreAi = secondArgument switch // what the fuck
            {
                0 => false,
                1 => true,
                _ => !_ignoreAi
            };
    
            Print("Bridge: _ignoreAI is now set to " + _ignoreAi + '.');
        }
        
        private void BridgeSetPose(string s)
        {
            Print("Bridge: BridgeSetPose() is not implemented.", LOGTYPE.ERROR);
        }
    
        private void Print(string message, LOGTYPE logtype = LOGTYPE.LOG)
        {
            if(DebugConsole.Instance != null)
                DebugConsole.Instance.Print(message, logtype);
        }
    }
}
