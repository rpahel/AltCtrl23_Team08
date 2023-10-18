using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/NPC", fileName = "newNPC")]
public class Character : ScriptableObject
{
    #region Fields

    [Header("Visuals")] 
    [SerializeField] private Sprite _headSprite;
    [SerializeField] private Sprite _badSprite;
    [SerializeField] private Sprite _neutralSprite;
    [SerializeField] private Sprite _goodSprite;
    [SerializeField] private Sprite _perfectSprite;
    [SerializeField] private Sprite _bodySprite;
    
    [Header("Characteristics")] 
    [SerializeField] private bool _isMale;

    #endregion

    #region Properties

    public Sprite HeadSprite
    {
        get => _headSprite;

        set => _headSprite = value;
    }
    public Sprite BadSprite
    {
        get => _badSprite;

        set => _badSprite = value;
    }
    public Sprite GoodSprite
    {
        get => _goodSprite;

        set => _goodSprite = value;
    }
    public Sprite PerfectSprite
    {
        get => _perfectSprite;

        set => _perfectSprite = value;
    }
    public Sprite NeutralSprite
    {
        get => _neutralSprite;

        set => _neutralSprite = value;
    }
    
    public Sprite BodySprite
    {
        get => _bodySprite;

        set => _bodySprite = value;
    }
    public bool IsMale
    {
        get => _isMale;

        set => _isMale = value;
    }

    #endregion
}
