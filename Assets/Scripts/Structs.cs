using System;
using ScrollShop.Enums;
using UnityEngine;

namespace ScrollShop.Structs
{
    [Serializable]
    public struct Pose
    {
        [SerializeField] private string _name;
        [SerializeField] private uint _id;
        [SerializeField] private ATTRIBUTE _attribute;

        public string GetName => _name;
        public uint GetId => _id;
        public ATTRIBUTE GetAttribute => _attribute;
    }
}
