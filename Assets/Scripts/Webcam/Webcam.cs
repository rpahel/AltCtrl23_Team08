using System.Collections;
using DG.Tweening;
using NaughtyAttributes;
using ScrollShop.CustomDebug;
using ScrollShop.Structs;
using UnityEngine;
using UnityEngine.UI;

public class Webcam : MonoBehaviour
{
    //== Fields =============================================
    [SerializeField, BoxGroup("Dependencies")]
    private string _webcamName = "Full HD webcam";
    
    [SerializeField, BoxGroup("Dependencies")]
    private Vector2Int _webcamResolution = new Vector2Int(1280, 720);

    [SerializeField, BoxGroup("Dependencies")]
    private int _webcamFps = 30;
    
    [SerializeField, BoxGroup("Dependencies")]
    private Image _image;

    [SerializeField, BoxGroup("Animations")]
    private bool _debugPlayAnimations = false;
    
    [SerializeField, BoxGroup("Animations")]
    private WebcamAnimationTransform _beginTransform;
    
    [SerializeField, BoxGroup("Animations")]
    private WebcamAnimationTransform _endTransform; 
    
    private WebCamTexture _wTexture;


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
    
    //== Private methods ====================================
    private void Awake()
    {
        _wTexture = new(_webcamName, _webcamResolution.x, _webcamResolution.y, _webcamFps);
        
        if (DebugConsole.Instance)
        {
            DebugConsole.Instance.Print("Webcam : Detected webcams (" + WebCamTexture.devices.Length + ")");
        
            foreach (var device in WebCamTexture.devices)
                DebugConsole.Instance.Print("Webcam : " + device.name + " (" + device.kind + ").");
        }
        else
        {
            Debug.Log("Webcam : Detected webcams (" + WebCamTexture.devices.Length + ")");
        
            foreach (var device in WebCamTexture.devices)
                Debug.Log("Webcam : " + device.name + " (" + device.kind + ").");
        }
    }

    private IEnumerator Start()
    {
        // Webcam stuff
        _image.material.mainTexture = _wTexture;
        _wTexture.Play();
        _image.sprite = null;

        if (_debugPlayAnimations)
        {
            yield return new WaitForSeconds(0.5f);
            DoBeginAnimation();
            yield return new WaitForSeconds(_beginTransform.GetMaxDuration());
            DoEndAnimation();
        }
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
