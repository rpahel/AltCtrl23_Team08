using System;
using ScrollShop.Enums;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Spell", fileName = "newSpell")]
public class Spell : ScriptableObject
{
    #region Fields

    [SerializeField] private uint _id;
    
    [SerializeField] private ATTRIBUTE _firstAttribute;
    [SerializeField] private ATTRIBUTE _secondAttribute;

    #endregion

    #region Properties

    public uint Id => _id;
    public ATTRIBUTE FirstAttribute => _firstAttribute;
    public ATTRIBUTE SecondAttribute => _secondAttribute;

    #endregion
}
