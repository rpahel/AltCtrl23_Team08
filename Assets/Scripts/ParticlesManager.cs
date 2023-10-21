using System.Collections.Generic;
using ScrollShop.AI;
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
    private PoseParticle[] _poseParticles;

    private List<GameObject> _sourceListPlayer0 = new List<GameObject>(4);
    private List<GameObject> _sourceListPlayer1 = new List<GameObject>(4);
    private List<GameObject> _particlesTrash = new List<GameObject>(4);

    private void OnEnable()
    {
        ClearParticleSystem();
        
        if(_bridge)
            _bridge.SubscribeToPoseChangedEvent(PoseChanged);
        
        InvokeRepeating(nameof(ClearTrashHandler), 1f, 1f);
    }

    private void OnDisable()
    {
        ClearParticleSystem();
        
        if(_bridge)
            _bridge.UnsubscribeFromPoseChangedEvent(PoseChanged);
        
        CancelInvoke();
    }

    private void PoseChanged(int index, Pose pose)
    {
        if (!gameObject.activeSelf)
            return;
        
        GameObject go = Instantiate(GetParticleFromPose(pose), index == 0 ? _particleSourcePlayer0 : _particleSourcePlayer1);
        
        if (index == 0)
        {
            foreach (var element in _sourceListPlayer0)
                element.GetComponent<VisualEffect>().Stop();
            
            EmptyListToTrash(_sourceListPlayer0);

            _sourceListPlayer0.Add(go);
        }
        else
        {
            foreach (var element in _sourceListPlayer1)
                element.GetComponent<VisualEffect>().Stop();
            
            EmptyListToTrash(_sourceListPlayer1);
            
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

    private void ClearParticleSystem()
    {
        EmptyListToTrash(_sourceListPlayer0);
        EmptyListToTrash(_sourceListPlayer1);
        ClearTrash(forceClear: true);
    }
    
    private void ClearTrash(bool forceClear = false)
    {
        for (int i = _particlesTrash.Count - 1; i >= 0; i--)
        {
            if (!forceClear && _particlesTrash[i].GetComponent<VisualEffect>().aliveParticleCount > 0)
                continue;
            
            GameObject go = _particlesTrash[i];
            _particlesTrash.Remove(go);
            Destroy(go);
        }
    }

    private void ClearTrashHandler() => ClearTrash();

    private void EmptyListToTrash(List<GameObject> list)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            _particlesTrash.Add(list[i]);
            list.RemoveAt(i);
        }
    }
}
