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

    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private AudioClip _sfxType;
    [SerializeField] private AudioClip _sfxEffect;

    [SerializeField] private Sprite _nameSprite;

    #endregion

    #region Properties

    public uint Id => _id;
    public ATTRIBUTE FirstAttribute => _firstAttribute;
    public ATTRIBUTE SecondAttribute => _secondAttribute;
    public AudioClip AudioClip => _audioClip;
    public Sprite NameSprite => _nameSprite;

    #endregion
}
