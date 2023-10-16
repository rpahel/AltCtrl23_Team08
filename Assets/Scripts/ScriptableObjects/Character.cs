using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/NPC", fileName = "newNPC")]
public class Character : ScriptableObject
{
    #region Fields

    [Header("Visuals")] 
    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _badSprite;
    [SerializeField] private Sprite _goodSprite;
    [SerializeField] private Sprite _perfectSprite;
    [SerializeField] private Sprite _specialSprite;
    
    [Header("Characteristics")] 
    [SerializeField] private bool _isMale;

    #endregion

    #region Properties

    public Sprite NormalSprite => _normalSprite;
    public Sprite BadSprite => _badSprite;
    public Sprite GoodSprite => _goodSprite;
    public Sprite PerfectSprite => _perfectSprite;
    public Sprite SpecialSprite => _specialSprite;
    public bool IsMale => _isMale;

    #endregion
}
