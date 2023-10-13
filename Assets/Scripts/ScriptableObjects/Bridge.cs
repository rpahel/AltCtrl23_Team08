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
        private PosesSO posesSo;
        [SerializeField, BoxGroup("Debug")]
        private bool _ignoreAi;
        
        private event Action<int, Pose> OnPoseChanged; //Action<index, Pose>
        private Dictionary<string, Pose> _posesDatabase;
        private Pose[] _previousPoses; // Does NOT remember ALL previous poses, but the last previous pose of each player index.
        private Pose[] _currentPoses; // Stock current pose per player index
        
        //== Interface implementations =============
        public void SubscribeToDebugConsole()
        {
            if (DebugConsole.Instance == null)
                return;
            
            DebugConsole.Instance.AddToMethodDictionary(BridgeToggleIgnoreAI);
            DebugConsole.Instance.AddToMethodDictionary(BridgeSetPose);
        }
    
        public void UnsubscribeFromDebugConsole()
        {
            if (DebugConsole.Instance == null)
                return;
            
            DebugConsole.Instance.RemoveFromMethodDictionary(BridgeToggleIgnoreAI);
            DebugConsole.Instance.RemoveFromMethodDictionary(BridgeSetPose);
        }
        
        //== Public methods ========================
        public void Initialize()
        {
            SubscribeToDebugConsole();
            
            if (posesSo == null || posesSo.GetPoses == null)
            {
                Print("Bridge: _posesSO is null !", LOGTYPE.ERROR);
                return;
            }
    
            if (posesSo.GetPoses.Length <= 0)
            {
                Print("Bridge: _posesSO is empty !", LOGTYPE.WARNING);
                return;
            }
            
            // Previous & current poses initialiser
            _previousPoses = new Pose[_numberOfPlayers];
            _currentPoses = new Pose[_numberOfPlayers];
    
            // Database check;
            if (_posesDatabase == null)
            {
                Print("Bridge: _posesDatabase is null. Building it...");
                FillDatabase();
            }
            else if (_posesDatabase.Count != posesSo.GetPoses.Length)
            {
                // Database and poses array don't match.
                // Database needs to be rebuilt.
                Print("Bridge: _posesDatabase does not match _posesSO. Rebuilding database...", LOGTYPE.WARNING);
                FillDatabase();
            }
        }
        
        public void Terminate()
        {
            UnsubscribeFromDebugConsole();

            OnPoseChanged = null;
            _previousPoses = null;
            _currentPoses = null;
            _posesDatabase.Clear();
        }

        public void SetPose(int index, string poseName)
        {
            if (_posesDatabase is not {Count: > 0})
            {
                Print("Bridge: _posesDatabase is null or empty", LOGTYPE.ERROR);
                return;
            }

            if (index < 0 || index >= _numberOfPlayers)
            {
                Print($"Bridge: SetPose -> index is outside the bounds of the array (index = {index}).", LOGTYPE.ERROR);
                return;
            }

            string lowerCaseName = poseName.ToLower();

            if (!_posesDatabase.ContainsKey(lowerCaseName))
            {
                Print($"Bridge: SetPose -> could not find pose of name \"{poseName}\" in _posesDatabase.", LOGTYPE.ERROR);
                return;
            }

            Pose matchPose = _posesDatabase[lowerCaseName];

            if (_currentPoses[index].GetId == matchPose.GetId)
            {
                // Given pose is already current pose
                return;
            }

            // Every check has passed, lets gooo
            _previousPoses[index] = _currentPoses[index];
            _currentPoses[index] = matchPose;
            
            OnPoseChanged?.Invoke(index, matchPose);
        }

        public void SubscribeToPoseChangedEvent(Action<int, Pose> method)
        {
            OnPoseChanged += method;
        }
        
        public void UnsubscribeFromPoseChangedEvent(Action<int, Pose> method)
        {
            OnPoseChanged -= method;
        }
        
        //== Private methods =======================
        private void FillDatabase()
        {
            _posesDatabase?.Clear();
            _posesDatabase = new Dictionary<string, Pose>();
            foreach (var pose in posesSo.GetPoses)
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
