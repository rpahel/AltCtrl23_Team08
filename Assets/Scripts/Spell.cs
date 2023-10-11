using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Spell", fileName = "newSpell")]
public class Spell : ScriptableObject
{
    #region Fields
    
    [SerializeField] private int _id;
    
    [SerializeField] private Attribute _firstAttribute;
    [SerializeField] private Attribute _secondAttribute;
    
    #endregion
    
    #region Properties
    
    public int Id => _id;
    public Attribute FirstAttribute => _firstAttribute;
    public Attribute SecondAttribute => _secondAttribute;
    
    #endregion
}

public enum Attribute
{
    None,
    Fire,
    Water,
    Steel,
    Furniture,
    Plants,
    LivingBeing,
    Explosion,
    Wall,
    Rain,
    Cleaning,
    Growth,
    Move,
    Stop,
    Repair,
}
