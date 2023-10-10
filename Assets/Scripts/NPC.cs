using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/NPC", fileName = "newNPC")]
public class NPC : ScriptableObject
{
    #region Fields
    
    [Header("Visuals")]
    

    [Header("NPC's requests")]
    [SerializeField] private Quest[] _quests;

    #endregion

    #region Properties

    public Quest[] Quests => _quests;

    #endregion
}
