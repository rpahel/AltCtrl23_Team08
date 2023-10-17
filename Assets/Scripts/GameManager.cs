using System;
using System.Collections;
using System.Collections.Generic;
using ScrollShop.CustomDebug;
using ScrollShop.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, IDebug
{
    #region Fields

    [Header("NPC Assets")]
    [SerializeField] private Sprite[] _maleHeadAssets;
    [SerializeField] private Sprite[] _maleBodyAssets;
    [SerializeField] private Sprite[] _maleNeutralEyesAssets;
    [SerializeField] private Sprite[] _maleGoodEyesAssets;
    [SerializeField] private Sprite[] _malePerfectEyesAssets;
    [SerializeField] private Sprite[] _maleBadEyesAssets;
    
    [SerializeField] private Sprite[] _femaleHeadAssets;
    [SerializeField] private Sprite[] _femaleBodyAssets;
    [SerializeField] private Sprite[] _femaleNeutralEyesAssets;
    [SerializeField] private Sprite[] _femaleGoodEyesAssets;
    [SerializeField] private Sprite[] _femalePerfectEyesAssets;
    [SerializeField] private Sprite[] _femaleBadEyesAssets;
    

    [Header("Scriptable Objects")]
    [SerializeField] private Quest[] _quests;
    [SerializeField] private Character[] _characters;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _textZone;
    [SerializeField] private Image _characterBodyImage;
    [SerializeField] private Image _characterHeadImage;
    [SerializeField] private Image _characterEyesImage;
    [SerializeField] private GameObject _scorePanel;
    [SerializeField] private TextMeshProUGUI _scoreText;

    [Header("Game Settings")]
    [SerializeField] private int _roundCount = 8;
    [SerializeField] private float _timeToChargeEnergy = 3f;
    [SerializeField] private float _timeToTakePose = 7f;
    [SerializeField] private float _timeBeforeNextRound = 10f;
    [SerializeField] private float _sensiCrsytalBall = 10f;

    [Header("Score Values")]
    [SerializeField] private int _perfectScoreValue = 10;
    [SerializeField] private int _goodScoreValue = 8;
    [SerializeField] private int _mediumScoreValue = 5;
    [SerializeField] private int _badScoreValue = 2;

    private int _roundNum;
    private int _score;

    private readonly List<Quest> _questBuffer = new();
    private Quest _currentQuest;

    private Dictionary<Quest, int> _questHistory = new();

    private readonly List<Character> _characterBuffer = new();
    private Character _currentCharacter;

    private float _crystalBallTimer;

    #endregion

    #region Public Methods

    public void SubscribeToDebugConsole()
    {
        if (DebugConsole.Instance == null) return;

        DebugConsole.Instance.AddToMethodDictionary(ChargeCrystalBall);
    }

    public void UnsubscribeFromDebugConsole()
    {
        if (DebugConsole.Instance == null) return;

        DebugConsole.Instance.RemoveFromMethodDictionary(ChargeCrystalBall);
    }

    #endregion

    #region Unity Event Function

    private void Awake()
    {
        for (int i = 0; i < _quests.Length; i++)
        {
            _questBuffer.Add(_quests[i]);
        }

        for (int i = 0; i < _characters.Length; i++)
        {
            _characterBuffer.Add(_characters[i]);
        }
    }

    private void Start()
    {
        SubscribeToDebugConsole();

        InitializeRound();

        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            var speed = Mathf.Abs(Input.GetAxis("Mouse X")) + Mathf.Abs(Input.GetAxis("Mouse Y"));

            if (speed > _sensiCrsytalBall)
            {
                _crystalBallTimer += Time.deltaTime;
                Debug.Log(_crystalBallTimer);
            }
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromDebugConsole();
    }

    #endregion

    #region Private Methods

    private void InitializeRound()
    {
        _roundNum++;
        _crystalBallTimer = 0f;

        if (_roundNum >= _roundCount + 1 || _questBuffer.Count == 0 || _characterBuffer.Count == 0)
        {
            _scorePanel.SetActive(true);
            _scoreText.text += $"TOTAL : {_score} golds";
            return;
        }

        _currentQuest = GenerateRequest();
        _currentCharacter = GenerateCharacter();

        _textZone.text = _currentQuest.RequestSentence;
        _characterBodyImage.sprite = _currentCharacter.BodySprite;
        _characterHeadImage.sprite = _currentCharacter.HeadSprite;
        _characterEyesImage.sprite = _currentCharacter.NeutralSprite;
        
        ServiceLocator.Get().PlaySound(_currentCharacter.IsMale ? _currentQuest.MaleQuestSound : _currentQuest.FemaleQuestSound);

        StartCoroutine(PlayRound());
    }

    private Quest GenerateRequest()
    {
        var random = Random.Range(0, _questBuffer.Count);

        var quest = _questBuffer[random];

        _questBuffer.Remove(quest);

        return quest;
    }

    private Character GenerateCharacter()
    {
        var random = Random.Range(0, _characterBuffer.Count);

        var character = _characterBuffer[random];

        //_characterBuffer.Remove(character);

        return character;
    }

    private Character CreateRandomCharacter()
    {
        var character = ScriptableObject.CreateInstance<Character>();

        if (Random.Range(0, 100) < 50)
        {
            var index = Random.Range(0, _maleHeadAssets.Length);
            
            character.BodySprite = _maleBodyAssets[index];
            character.HeadSprite = _maleHeadAssets[index];

            character.BadSprite = _maleBadEyesAssets[index];
            character.NeutralSprite = _maleNeutralEyesAssets[index];
            character.GoodSprite = _maleGoodEyesAssets[index];
            character.PerfectSprite = _malePerfectEyesAssets[index];
        }
        else
        {
            var index = Random.Range(0, _femaleHeadAssets.Length);
            
            character.BodySprite = _femaleBodyAssets[index];
            character.HeadSprite = _femaleHeadAssets[index];
            
            character.BadSprite = _femaleBadEyesAssets[index];
            character.NeutralSprite = _femaleNeutralEyesAssets[index];
            character.GoodSprite = _femaleGoodEyesAssets[index];
            character.PerfectSprite = _femalePerfectEyesAssets[index];
        }
        
        return character;
    }

    private IEnumerator PlayRound()
    {
        var poseId = 0;

        while (_crystalBallTimer < _timeToChargeEnergy)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(_timeToTakePose);

        RoundEnd(poseId);

        yield return new WaitForSeconds(_timeBeforeNextRound);

        InitializeRound();
    }

    private void RoundEnd(int id)
    {
        for (int i = 0; i < _currentQuest.MediumSpells.Length; i++)
        {
            if (id != _currentQuest.MediumSpells[i].Id) continue;

            UpdateUI(_currentCharacter.NeutralSprite, _currentQuest.MediumSentence);
            UpdateScore(_mediumScoreValue);;
            return;
        }

        for (int i = 0; i < _currentQuest.GoodSpells.Length; i++)
        {
            if (id != _currentQuest.GoodSpells[i].Id) continue;

            UpdateUI(_currentCharacter.GoodSprite, _currentQuest.GoodSentence);
            UpdateScore(_goodScoreValue);
            return;
        }

        for (int i = 0; i < _currentQuest.PerfectSpells.Length; i++)
        {
            if (id != _currentQuest.PerfectSpells[i].Id) continue;

            UpdateUI(_currentCharacter.PerfectSprite, _currentQuest.PerfectSentence);
            UpdateScore(_perfectScoreValue);
            return;
        }

        for (int i = 0; i < _currentQuest.SpecialSpells.Length; i++)
        {
            if (id != _currentQuest.SpecialSpells[i].Id) continue;

            UpdateUI(_currentCharacter.NeutralSprite, _currentQuest.SpecialSentence);
            UpdateScore(_currentQuest.SpecialScoreValue);
            return;
        }

        UpdateUI(_currentCharacter.BadSprite, _currentQuest.BadSentence);
        UpdateScore(_badScoreValue);
    }

    private void UpdateUI(Sprite image, string characterSentence)
    {
        _characterEyesImage.sprite = image;
        _textZone.text = characterSentence;
    }

    private void UpdateScore(int score)
    {
        _scoreText.text += $"Quest n°{_questHistory.Count + 1} : {score} golds" + "\n";
        _score += score;
        _questHistory.Add(_currentQuest, score);
    }

    private void ChargeCrystalBall(string s)
    {
        _crystalBallTimer += 100f;
    }

    #endregion
}
