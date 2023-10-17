using System.Collections;
using DG.Tweening;
using NaughtyAttributes;
using ScrollShop.CustomDebug;
using ScrollShop.Enums;
using ScrollShop.Structs;
using UnityEngine;
using UnityEngine.UI;

public class Webcam : MonoBehaviour
{
    //== Fields =============================================
    [SerializeField, BoxGroup("Dependencies")]
    private string _webcamNameOverride = "";
    
    [SerializeField, BoxGroup("Dependencies")]
    private Vector2Int _webcamResolution = new Vector2Int(1280, 720);

    [SerializeField, BoxGroup("Dependencies")]
    private int _webcamFps = 30;
    
    [SerializeField, BoxGroup("Dependencies")]
    private Image _image;

#if UNITY_EDITOR
    [SerializeField, BoxGroup("Animations")]
    private bool _debugPlayBeginAnimationOnStart = false;
#endif

    [SerializeField, BoxGroup("Animations")]
    private WebcamAnimationTransform _beginTransform;
    
#if UNITY_EDITOR
    [SerializeField, BoxGroup("Animations")]
    private bool _debugPlayEndAnimationOnStart = false;
#endif
    
    [SerializeField, BoxGroup("Animations")]
    private WebcamAnimationTransform _endTransform; 
    
    private WebCamTexture _wTexture;
    
    //== Public methods =====================================
    public void DoBeginAnimation()
    {
        // Setup
        Transform t = _image.transform;
        Vector2 targetPos = t.localPosition;
        Vector3 targetRot = t.localRotation.eulerAngles;
        Vector3 targetScale = t.localScale;
        t.localPosition = _beginTransform.position.vector;
        t.localRotation = Quaternion.Euler(_beginTransform.rotation.vector);
        t.localScale = _beginTransform.scale.vector;
        
        // Animation
        _image.transform.DOLocalMove(targetPos, _beginTransform.position.duration, true).SetEase(_beginTransform.position.ease);
        _image.transform.DOLocalRotate(targetRot, _beginTransform.rotation.duration, RotateMode.FastBeyond360).SetEase(_beginTransform.rotation.ease);
        _image.transform.DOScale(targetScale, _beginTransform.scale.duration).SetEase(_beginTransform.scale.ease);
    }
    
    public void DoEndAnimation()
    {
        // Setup
        Vector2 targetPos = _endTransform.position.vector;
        Vector3 targetRot = _endTransform.rotation.vector;
        Vector3 targetScale = _endTransform.scale.vector;
        
        // Animation
        _image.transform.DOLocalMove(targetPos, _endTransform.position.duration, true).SetEase(_endTransform.position.ease);
        _image.transform.DOLocalRotate(targetRot, _endTransform.rotation.duration, RotateMode.FastBeyond360).SetEase(_endTransform.rotation.ease);
        _image.transform.DOScale(targetScale, _endTransform.scale.duration).SetEase(_endTransform.scale.ease);
    }

    public void PauseWebcam()
    {
        _wTexture.Pause();
    }

    public void StopWebcam()
    {
        _wTexture.Stop();
    }
    
    //== Private methods ====================================
    private void Awake()
    {
        string webcamName;
        
        if (DebugConsole.Instance)
        {
            if (WebCamTexture.devices.Length <= 0)
            {
                DebugConsole.Instance.Print("Webcam : No Webcam devices detected !", LOGTYPE.ERROR);
                return;
            }
            
            DebugConsole.Instance.Print("Webcam : Detected webcams (" + WebCamTexture.devices.Length + ")");
        
            foreach (var device in WebCamTexture.devices)
                DebugConsole.Instance.Print("Webcam : " + device.name + " (" + device.kind + ").");
        }
        else
        {
            if (WebCamTexture.devices.Length <= 0)
            {
                Debug.LogError("Webcam : No Webcam devices detected !");
                return;
            }
            
            Debug.Log("Webcam : Detected webcams (" + WebCamTexture.devices.Length + ")");
        
            foreach (var device in WebCamTexture.devices)
                Debug.Log("Webcam : " + device.name + " (" + device.kind + ").");
        }

        webcamName = _webcamNameOverride == "" ? WebCamTexture.devices[0].name : _webcamNameOverride;
        _wTexture = new(webcamName, _webcamResolution.x, _webcamResolution.y, _webcamFps);
    }

    private IEnumerator Start()
    {
        // Webcam stuff
        _image.material.mainTexture = _wTexture;
        _wTexture.Play();
        _image.sprite = null;

        #if UNITY_EDITOR
        if (_debugPlayBeginAnimationOnStart)
        {
            yield return new WaitForSeconds(1f);
            DoBeginAnimation();
            yield return new WaitForSeconds(_beginTransform.GetMaxDuration());
        }

        if (_debugPlayEndAnimationOnStart)
        {
            yield return new WaitForSeconds(1f);
            _wTexture.Pause();
            DoEndAnimation();
            yield return new WaitForSeconds(_endTransform.GetMaxDuration());
        }
        #endif
    }

    private void OnDisable()
    {
        _wTexture.Pause();
        DOTween.Kill(this);
    }

    private void OnDestroy()
    {
        _wTexture.Stop();
        DOTween.Kill(this);
    }
    
#if UNITY_EDITOR
    #region Editor
    //== Editor only methods ================================
    [Button]
    private void BeginAnimationSaveCurrentImageTransform()
    {
        Transform t = _image.transform;
        
        WebcamAnimationProperty wpos = new WebcamAnimationProperty(
            _beginTransform.position.ease,
            _beginTransform.position.duration,
            t.localPosition);
        
        WebcamAnimationProperty wrot = new WebcamAnimationProperty(
            _beginTransform.rotation.ease,
            _beginTransform.rotation.duration,
            t.localRotation.eulerAngles);
        
        WebcamAnimationProperty wsca = new WebcamAnimationProperty(
            _beginTransform.scale.ease,
            _beginTransform.scale.duration,
            t.localScale);

        _beginTransform = new WebcamAnimationTransform(wpos, wrot, wsca);
    }

    [Button]
    private void EndAnimationSaveCurrentImageTransform()
    {
        Transform t = _image.transform;
        
        WebcamAnimationProperty wpos = new WebcamAnimationProperty(
            _endTransform.position.ease,
            _endTransform.position.duration,
            t.localPosition);
        
        WebcamAnimationProperty wrot = new WebcamAnimationProperty(
            _endTransform.rotation.ease,
            _endTransform.rotation.duration,
            t.localRotation.eulerAngles);
        
        WebcamAnimationProperty wsca = new WebcamAnimationProperty(
            _endTransform.scale.ease,
            _endTransform.scale.duration,
            t.localScale);

        _endTransform = new WebcamAnimationTransform(wpos, wrot, wsca);
    }
    #endregion
#endif
}
