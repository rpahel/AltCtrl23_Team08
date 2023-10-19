using System.Collections;
using System.Collections.Generic;
using ScrollShop.AI;
using ScrollShop.CustomDebug;
using ScrollShop.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.UI;
using Pose = ScrollShop.Structs.Pose;

public class GameManager : MonoBehaviour, IDebug
{
    #region Fields
    
    [Header("Game Settings")]
    [SerializeField] private int _roundCount = 8;
    [SerializeField] private float _sensiCrsytalBall = 10f;
    [SerializeField] private float _timeToChargeEnergy = 3f;
    [SerializeField] private float _timeToTakePose = 7f;
    [SerializeField] private float _timeBeforePhotoHiding = 4f;
    [SerializeField] private float _timeBeforeNextRound = 10f;

    [Header("Score Values")]
    [SerializeField] private int _perfectScoreValue = 10;
    [SerializeField] private int _goodScoreValue = 8;
    [SerializeField] private int _mediumScoreValue = 5;
    [SerializeField] private int _badScoreValue = 2;
    
    [Header("Audio")]
    [SerializeField] private AudioClip _musicInGame;
    [SerializeField] private AudioClip _musicPose;
    [SerializeField] private AudioClip _musicEnd;
    
    [Header("Scriptable Objects")]
    [SerializeField] private Quest[] _quests;
    
    [Header("NPC assets")]
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

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _textZone;
    [SerializeField] private GameObject _npc;
    [SerializeField] private GameObject _dialogueZone;
    [SerializeField] private Image _characterBodyImage;
    [SerializeField] private Image _characterHeadImage;
    [SerializeField] private Image _characterEyesImage;
    [SerializeField] private GameObject _scorePanel;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private GameObject _webcamPanel;
    [SerializeField] private Webcam _webcam;
    [SerializeField] private Image _spellNameImage;
    

    [Header("Set Up")]
    [SerializeField] private AiManager _aiManager;
    [SerializeField] private Spell[] _spells;
    [SerializeField] private Spell _defaultSpell;

    private int _roundNum;
    private int _score;

    private readonly List<Quest> _questBuffer = new();
    private Quest _currentQuest;

    private readonly Dictionary<Quest, int> _questHistory = new();

    private Character _currentCharacter;

    private Sprite _lastBodySprite;
    private Sprite _lastHeadSprite;

    private float _crystalBallTimer;

    private bool _gameEnded;

    private readonly Dictionary<int, ScrollShop.Structs.Pose> _playerPoses = new();

    private Spell _currentSpell;

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
        
        _playerPoses.Add(0, default);
        _playerPoses.Add(1, default);

        _currentSpell = _defaultSpell;
    }

    private void Start()
    {
        SubscribeToDebugConsole();

        InitializeRound();
        
        ServiceLocator.Get().ChangeMusic(_musicInGame, true);

        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            var speed = Mathf.Abs(Input.GetAxis("Mouse X")) + Mathf.Abs(Input.GetAxis("Mouse Y"));

            if (speed > _sensiCrsytalBall)
            {
                if (!_gameEnded)
                {
                    _crystalBallTimer += Time.deltaTime;
                    Debug.Log(_crystalBallTimer);
                    return;
                }

                StartCoroutine(ResetGame());
            }
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromDebugConsole();
        _aiManager.GetBridge.UnsubscribeFromPoseChangedEvent(UpdatePose);
    }

    #endregion

    #region Private Methods

    private void InitializeRound()
    {
        _roundNum++;
        _crystalBallTimer = 0f;

        if (_roundNum >= _roundCount + 1 || _questBuffer.Count == 0)
        {
            _npc.SetActive(false);
            _dialogueZone.SetActive(false);
            _scorePanel.SetActive(true);
            
            _scoreText.text += "\n" + $"TOTAL : {_score} golds";
            
            ServiceLocator.Get().ChangeMusic(_musicEnd);
            _gameEnded = true;
            
            return;
        }

        _npc.SetActive(true);
        _dialogueZone.SetActive(true);
        _webcamPanel.SetActive(false);
        _spellNameImage.gameObject.SetActive(false);

        _currentQuest = GenerateRequest();
        _currentCharacter = CreateRandomCharacter();

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

    private Character CreateRandomCharacter()
    {
        var character = ScriptableObject.CreateInstance<Character>();
        character.BodySprite = _lastBodySprite;
        character.HeadSprite = _lastHeadSprite;

        while (_lastBodySprite == character.BodySprite || _lastHeadSprite == character.HeadSprite)
        {
            if (Random.Range(0, 100) < 50)
            {
                var headIndex = Random.Range(0, _maleHeadAssets.Length);
                var bodyIndex = Random.Range(0, _maleBodyAssets.Length);
            
                character.BodySprite = _maleBodyAssets[bodyIndex];
                character.HeadSprite = _maleHeadAssets[headIndex];

                character.BadSprite = _maleBadEyesAssets[0];
                character.NeutralSprite = _maleNeutralEyesAssets[0];
                character.GoodSprite = _maleGoodEyesAssets[0];
                character.PerfectSprite = _malePerfectEyesAssets[0];

                character.IsMale = true;
            }
            else
            {
                var headIndex = Random.Range(0, _femaleHeadAssets.Length);
                var bodyIndex = Random.Range(0, _femaleBodyAssets.Length);
            
                character.BodySprite = _femaleBodyAssets[bodyIndex];
                character.HeadSprite = _femaleHeadAssets[headIndex];

                character.BadSprite = _femaleBadEyesAssets[0];
                character.NeutralSprite = _femaleNeutralEyesAssets[0];
                character.GoodSprite = _femaleGoodEyesAssets[0];
                character.PerfectSprite = _femalePerfectEyesAssets[0];
                
                character.IsMale = false;
            }
        }

        _lastBodySprite = character.BodySprite;
        _lastHeadSprite = character.HeadSprite;

        return character;
    }

    private IEnumerator PlayRound()
    {
        var poseId = 0;

        while (_crystalBallTimer < _timeToChargeEnergy)
        {
            yield return new WaitForEndOfFrame();
        }
        
        ServiceLocator.Get().ChangeMusic(_musicPose);
        
        _aiManager.GetBridge.SubscribeToPoseChangedEvent(UpdatePose);
        
        _webcamPanel.SetActive(true);
        _webcam.PlayWebcam();
        _webcam.DoBeginAnimation();

        yield return new WaitForSeconds(_timeToTakePose);
        
        _aiManager.GetBridge.UnsubscribeFromPoseChangedEvent(UpdatePose);

        for (int i = 0; i < _spells.Length; i++)
        {
            if (_spells[i].FirstAttribute == _playerPoses[0].GetAttribute
                && _spells[i].SecondAttribute == _playerPoses[1].GetAttribute)
            {
                _currentSpell = _spells[i];
            }
        }

        _webcam.PauseWebcam();
        _webcam.TakeScreenshot();

        RoundEnd(poseId);
        ServiceLocator.Get().ChangeMusic(_musicInGame, true);

        _spellNameImage.sprite = _currentSpell.NameSprite;
        _spellNameImage.gameObject.SetActive(true);
        
        ServiceLocator.Get().PlaySound(_currentSpell.AudioClip);

        yield return new WaitForSeconds(_timeBeforePhotoHiding);
        
        _webcam.DoEndAnimation();

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

            if (_currentQuest.SpecialScoreValue is >= 0 and < 30)
            {
                UpdateUI(_currentCharacter.BadSprite, _currentQuest.SpecialSentence);
            }
            else if (_currentQuest.SpecialScoreValue is >= 30 and < 59)
            {
                UpdateUI(_currentCharacter.NeutralSprite, _currentQuest.SpecialSentence);
            }
            else if (_currentQuest.SpecialScoreValue is >= 60 and < 79)
            {
                UpdateUI(_currentCharacter.GoodSprite, _currentQuest.SpecialSentence);
            }
            else
            {
                UpdateUI(_currentCharacter.PerfectSprite, _currentQuest.SpecialSentence);
            }
            
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

    private IEnumerator ResetGame()
    {
        _gameEnded = false;
        _roundNum = 0;
        
        _scorePanel.SetActive(false);
        _scoreText.text = null;
        
        _score = 0;
        _questHistory.Clear();
        
        ServiceLocator.Get().Reset();
        ServiceLocator.Get().ChangeMusic(_musicInGame, true);
                
        for (int i = 0; i < _quests.Length; i++)
        {
            _questBuffer.Add(_quests[i]);
        }

        yield return new WaitForSeconds(3f);
                
        InitializeRound();
    }

    private void UpdatePose(int index, ScrollShop.Structs.Pose pose)
    {
        _playerPoses[index] = pose;
    }

    #endregion
}
