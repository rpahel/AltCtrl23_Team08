using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;
using UnityGoogleDrive.Data;

public class GoogleDriveManager : MonoBehaviour
{
    public RenderTexture renderTex;
    
    public void TakeScreenShot()
    {
        RenderTexture.active = renderTex;
        Texture2D destinationTexture = new Texture2D(renderTex.width, renderTex.height, TextureFormat.RGB24, false);
        Rect regionToReadFrom = new Rect(0, 0, renderTex.width, renderTex.height);
        destinationTexture.ReadPixels(regionToReadFrom, 0, 0);
        destinationTexture.Apply();
        StartCoroutine(UploadImageCoroutine(destinationTexture));
    }

    private IEnumerator UploadImageCoroutine(Texture2D image)
    {
        byte[] png = image.EncodeToJPG();
        File file = new File{Name = "Test upload image", Content = png};
        var request = GoogleDriveFiles.Create(file);
        request.Fields = new List<string> {"id"};
        yield return request.Send();
        print(request.IsError);
        print(request.ResponseData.Content);
        print(request.ResponseData.Id);
    }
}
