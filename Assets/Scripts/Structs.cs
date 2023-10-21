using System;
using DG.Tweening;
using JetBrains.Annotations;
using ScrollShop.Enums;
using UnityEngine;

namespace ScrollShop.Structs
{
    [Serializable]
    public struct Pose
    {
        [SerializeField] [CanBeNull]
        private string _name;
        [SerializeField]
        private uint _id;
        [SerializeField]
        private ATTRIBUTE _attribute;

        public string GetName => _name;
        public uint GetId => _id;
        public ATTRIBUTE GetAttribute => _attribute;
    }

    [Serializable]
    public struct PoseParticle
    {
        [SerializeField] private int _poseId;
        [SerializeField] private GameObject _particlesPrefab;

        public int GetPoseId => _poseId;
        public GameObject GetParticles => _particlesPrefab;
    }

    [Serializable]
    public struct WebcamAnimationProperty
    {
        public Ease ease;
        public float duration;
        public Vector3 vector;

        public WebcamAnimationProperty(Ease ease, float duration, Vector3 vector)
        {
            this.ease = ease;
            this.duration = duration;
            this.vector = vector;
        }
    }

    [Serializable]
    public struct WebcamAnimationTransform
    {
        public WebcamAnimationProperty position;
        public WebcamAnimationProperty rotation;
        public WebcamAnimationProperty scale;

        public WebcamAnimationTransform(
            WebcamAnimationProperty pos,
            WebcamAnimationProperty rot,
            WebcamAnimationProperty sca)
        {
            position = pos;
            rotation = rot;
            scale = sca;
        }

        public float GetMaxDuration() => Mathf.Max(position.duration, rotation.duration, scale.duration);
    }
}
