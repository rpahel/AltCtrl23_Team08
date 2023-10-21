using System.Collections.Generic;
using ScrollShop.AI;
using ScrollShop.ScriptableObjects;
using ScrollShop.Structs;
using UnityEngine;
using UnityEngine.VFX;
using Pose = ScrollShop.Structs.Pose;

public class ParticlesManager : MonoBehaviour
{
    [SerializeField]
    private Bridge _bridge;
    
    [SerializeField]
    private Transform _particleSourcePlayer0, _particleSourcePlayer1;

    [SerializeField]
    private PosesSO _posesSO;
    
    [SerializeField]
    private PoseParticle[] _poseParticles;

    private List<GameObject> _sourceListPlayer0 = new List<GameObject>(4);
    private List<GameObject> _sourceListPlayer1 = new List<GameObject>(4);
    private List<GameObject> _particlesTrash = new List<GameObject>(4);

    private void Start()
    {
        if(_bridge)
            _bridge.SubscribeToPoseChangedEvent(PoseChanged);
        
        InvokeRepeating(nameof(ClearParticleSystems), 1f, 1f);
    }

    private void OnDisable()
    {
        if(_bridge)
            _bridge.UnsubscribeFromPoseChangedEvent(PoseChanged);
    }
    
    private void OnDestroy()
    {
        if(_bridge)
            _bridge.UnsubscribeFromPoseChangedEvent(PoseChanged);
    }

    private void PoseChanged(int index, Pose pose)
    {
        GameObject go = Instantiate(GetParticleFromPose(pose), index == 0 ? _particleSourcePlayer0 : _particleSourcePlayer1);
        
        if (index == 0)
        {
            for (int i = _sourceListPlayer0.Count - 1; i >= 0; i--)
            {
                _sourceListPlayer0[i].GetComponent<VisualEffect>().Stop();
                _particlesTrash.Add(_sourceListPlayer0[i]);
                _sourceListPlayer0.RemoveAt(i);
            }
            
            _sourceListPlayer0.Add(go);
        }
        else
        {
            for (int i = _sourceListPlayer1.Count - 1; i >= 0; i--)
            {
                _sourceListPlayer1[i].GetComponent<VisualEffect>().Stop();
                _particlesTrash.Add(_sourceListPlayer1[i]);
                _sourceListPlayer1.RemoveAt(i);
            }
            
            _sourceListPlayer1.Add(go);
        }
        
        go.GetComponent<VisualEffect>().Play();
    }

    private GameObject GetParticleFromPose(Pose pp)
    {
        foreach (var pose in _poseParticles)
            if (pose.GetPoseId == pp.GetId)
                return pose.GetParticles;

        throw new KeyNotFoundException();
    }

    private void ClearParticleSystems()
    {
        for (int i = _particlesTrash.Count - 1; i >= 0; i--)
        {
            if (_particlesTrash[i].GetComponent<VisualEffect>().aliveParticleCount > 0)
                continue;
            
            GameObject go = _particlesTrash[i];
            _particlesTrash.Remove(go);
            Destroy(go);
        }
    }
}
