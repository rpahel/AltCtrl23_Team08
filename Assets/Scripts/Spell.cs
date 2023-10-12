using System;
using ScrollShop.Enums;

[Serializable]
public struct Spell
{
    #region Fields

    private ATTRIBUTE _firstAttribute;
    private ATTRIBUTE _secondAttribute;

    #endregion

    #region Properties

    public ATTRIBUTE FirstAttribute => _firstAttribute;
    public ATTRIBUTE SecondAttribute => _secondAttribute;

    #endregion
}
