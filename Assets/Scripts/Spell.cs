using System;

[Serializable]
public struct Spell
{
    #region Fields

    private Attribute _firstAttribute;
    private Attribute _secondAttribute;

    #endregion

    #region Properties

    public Attribute FirstAttribute => _firstAttribute;
    public Attribute SecondAttribute => _secondAttribute;

    #endregion
}

public enum Attribute
{
    None,
    Fire,
    Water,
    Lightning,
    Furniture,
    Plants,
    LivingBeing,
    Explosion,
    Wall,
    Rain,
    Cleaning,
    Growth,
    Throw,
}
