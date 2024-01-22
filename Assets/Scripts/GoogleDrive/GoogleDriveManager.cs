using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ScrollShop.CustomDebug;
using ScrollShop.Enums;
using UnityEngine;
using UnityGoogleDrive;
using File = UnityGoogleDrive.Data.File;

public class GoogleDriveManager : MonoBehaviour
{
    [SerializeField]
    private RenderTexture _renderTexture;
    [SerializeField]
    private Camera _screenshotCamera;
    
    //==== Public Methods ====
    public void TakeScreenShot()
    {
        RenderTexture mRt = new RenderTexture(
            _renderTexture.width, 
            _renderTexture.height,
            _renderTexture.depth, 
            RenderTextureFormat.ARGB32, 
            RenderTextureReadWrite.sRGB);
        
        mRt.antiAliasing = _renderTexture.antiAliasing;

        _screenshotCamera.targetTexture = mRt;
        _screenshotCamera.Render();
        RenderTexture.active = mRt;
        
        Texture2D destinationTexture = new Texture2D(mRt.width, mRt.height, TextureFormat.ARGB32, false);
        Rect regionToReadFrom = new Rect(0, 0, _renderTexture.width, _renderTexture.height);
        destinationTexture.ReadPixels(regionToReadFrom, 0, 0);
        destinationTexture.Apply();

        // Local
        LocalSave(destinationTexture);

        // Internet
        //StartCoroutine(UploadImageCoroutine(destinationTexture));
    }

    //==== Private Methods ====
    private IEnumerator UploadImageCoroutine(Texture2D image)
    {
        byte[] png = image.EncodeToJPG();
        File file = new File{Name = "Test upload image", Content = png};
        var request = GoogleDriveFiles.Create(file);
        request.Fields = new List<string> {"id", "name"};
        yield return request.Send();

        if (!DebugConsole.Instance)
            yield break;
        
        if (request.IsError)
        {
            DebugConsole.Instance.Print("GDrive : Error !", LOGTYPE.ERROR);
            DebugConsole.Instance.Print("GDrive : " + request.ResponseData.Content, LOGTYPE.ERROR);
        }
        else
        {
            DebugConsole.Instance.Print("GDrive : Image upload successful.");
            DebugConsole.Instance.Print("GDrive : Image ID = " + request.ResponseData.Id);
            DebugConsole.Instance.Print("GDrive : Image Name = " + request.ResponseData.Name);
        }
    }

    public void LocalSave(Texture2D image)
    {
        DateTime Now = DateTime.Now;
        string Path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures) + "\\SGMSS";
        string FileName =
            Now.Year.ToString("0000") +
            Now.Month.ToString("00") +
            Now.Day.ToString("00") +
            '_' +
            Now.Hour.ToString("00") +
            Now.Minute.ToString("00") +
            Now.Second.ToString("00") +
            ".jpg";

        if(!Directory.Exists(Path))
            Directory.CreateDirectory(Path);

        System.IO.File.WriteAllBytes(Path + "\\" + FileName, image.EncodeToJPG());
    }
}
