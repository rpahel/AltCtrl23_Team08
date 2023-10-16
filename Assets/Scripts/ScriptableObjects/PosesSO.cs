using System;
using UnityEngine;
using Pose = ScrollShop.Structs.Pose;

namespace ScrollShop.ScriptableObjects
{
    [CreateAssetMenu(fileName = "PosesSO", menuName = "ScriptableObjects/PosesSO")]
    public class PosesSO : ScriptableObject
    {
        [SerializeField] private Pose[] _poses;

        public Pose[] GetPoses => _poses;
    }
}