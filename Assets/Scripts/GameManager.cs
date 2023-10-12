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

    // [Header("NPC Assets")]
    // [SerializeField] private Sprite[] _headAssets;
    // [SerializeField] private Sprite[] _bodyAssets;
    
    [Header("Scriptable Objects")]
    [SerializeField] private Quest[] _quests;
    [SerializeField] private Character[] _characters;

    [Header("UI")] 
    [SerializeField] private TextMeshProUGUI _textZone;
    [SerializeField] private Image _characterImage;
    
    [Header("Game Settings")]
    [SerializeField] private int _roundCount = 8;
    [SerializeField] private float _timeToChargeEnergy = 3f;
    [SerializeField] private float _timeToTakePose = 7f;
    [SerializeField] private float _timeBeforeNextRound = 10f;

    private int _roundNum = 0;

    private readonly List<Quest> _questBuffer = new();
    private Quest _currentQuest;
    
    private readonly List<Character> _characterBuffer = new();
    private Character _currentCharacter;

    private float _crystalBallTimer;

    #endregion

    #region Public Methods

    public void AddDebugMethodsToDebugConsole()
    {
        if (DebugConsole.Instance == null) return;
            
        DebugConsole.Instance.AddToMethodDictionary(ChargeCrystalBall);
    }

    public void RemoveDebugMethodsFromDebugConsole()
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
        
        AddDebugMethodsToDebugConsole();
    }

    private void Start()
    {
        InitializeRound();
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            var speed = Mathf.Abs(Input.GetAxis("Mouse X")) + Mathf.Abs(Input.GetAxis("Mouse Y"));

            if (speed > 15f)
            {
                _crystalBallTimer += Time.deltaTime;
            }
        }
    }

    private void OnDestroy()
    {
        RemoveDebugMethodsFromDebugConsole();
    }

    #endregion
    
    #region Private Methods

    private void InitializeRound()
    {
        _roundNum++;
        _crystalBallTimer = 0f;

        if (_roundNum >= _roundCount || _questBuffer.Count == 0 || _characterBuffer.Count == 0)
        {
            Debug.Log("finish game");
            return;
        }
        
        _currentQuest = GenerateRequest();
        _currentCharacter = GenerateCharacter();

        _textZone.text = _currentQuest.RequestSentence;
        _characterImage.sprite = _currentCharacter.NormalSprite;

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

    private IEnumerator PlayRound()
    {
        var poseId = 0;

        while (_crystalBallTimer < _timeToChargeEnergy)
        {
            yield return new WaitForEndOfFrame();
        }
        
        yield return new WaitForSeconds(_timeToTakePose);
        
        UpdateCharacter(poseId);

        yield return new WaitForSeconds(_timeBeforeNextRound);
        
        InitializeRound();
    }

    private void UpdateCharacter(int id)
    {
        for (int i = 0; i < _currentQuest.MediumSpells.Length; i++)
        {
            if (id != _currentQuest.MediumSpells[i].Id) continue;
            
            _characterImage.sprite = _currentCharacter.NormalSprite;
            _textZone.text = _currentQuest.MediumSentence;
            return;
        }
        
        for (int i = 0; i < _currentQuest.GoodSpells.Length; i++)
        {
            if (id != _currentQuest.GoodSpells[i].Id) continue;
            
            _characterImage.sprite = _currentCharacter.GoodSprite;
            _textZone.text = _currentQuest.GoodSentence;
            return;
        }
        
        for (int i = 0; i < _currentQuest.PerfectSpells.Length; i++)
        {
            if (id != _currentQuest.PerfectSpells[i].Id) continue;
            
            _characterImage.sprite = _currentCharacter.PerfectSprite;
            _textZone.text = _currentQuest.PerfectSentence;
            return;
        }
        
        for (int i = 0; i < _currentQuest.SpecialSpells.Length; i++)
        {
            if (id != _currentQuest.SpecialSpells[i].Id) continue;
            
            _characterImage.sprite = _currentCharacter.SpecialSprite;
            _textZone.text = _currentQuest.SpecialSentence;
            return;
        }
        
        _characterImage.sprite = _currentCharacter.BadSprite;
        _textZone.text = _currentQuest.BadSentence;
    }
    
    private void ChargeCrystalBall(string s)
    {
        _crystalBallTimer += 100f;
    }

    #endregion
}
