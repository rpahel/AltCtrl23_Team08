using System;
using UnityEngine;
using NaughtyAttributes;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Quest", fileName = "newQuest")]

public class Quest : ScriptableObject
{
    #region Fields

    [SerializeField] private bool _isRandomNPC;

    [SerializeField] [EnableIf("_isRandomNPC")] private NPC _character;
    [SerializeField] [TextArea] private string _requestSentence;

    [Header("Wrong Outcomes")]
    [SerializeField] private Spell[] _wrongSpells;
    [SerializeField] [TextArea] private string _wrongSentence;
    
    [Header("Medium Outcomes")]
    [SerializeField] private Spell[] _mediumSpells;
    [SerializeField] [TextArea] private string _mediumSentence;
    
    [Header("Good Outcomes")]
    [SerializeField] private Spell[] _goodSpells;
    [SerializeField] [TextArea] private string _goodSentence;
    
    [Header("Perfect Outcomes")]
    [SerializeField] private Spell[] _perfectSpells;
    [SerializeField] [TextArea] private string _perfectSentence;
    
    [Header("Special Outcomes")]
    [SerializeField] private Spell[] _specialSpells;
    [SerializeField] [TextArea] private string _specialSentence;

    #endregion

    #region Properties

    public NPC Character => _character;

    public string RequestSentence => _requestSentence;

    public Spell[] WrongSpells => _wrongSpells;

    public string WrongSentence => _wrongSentence;
    
    public Spell[] MediumSpells => _mediumSpells;

    public string MediumSentence => _mediumSentence;
    
    public Spell[] GoodSpells => _goodSpells;

    public string GoodSentence => _goodSentence;
    
    public Spell[] PerfectSpells => _perfectSpells;

    public string PerfectSentence => _perfectSentence;
    
    public Spell[] SpecialSpells => _specialSpells;

    public string SpecialSentence => _specialSentence;

    #endregion

    #region Public Methods

    public void Initialize()
    {
        if (_isRandomNPC)
        {
            //définir les visuels du NPC
        }
    }

    #endregion
}
