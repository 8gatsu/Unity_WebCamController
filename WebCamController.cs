using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebCamController : MonoBehaviour {
    public int targetCamID = 0;
    public int width = 640;
    public int height = 480;
    public int fps = 30;
    public bool autoRun = false;

    public Texture2D outputTexture;     // 出力結果 RawImageに張り付ける

    // 取得した画像に適用する画像処理メソッド
    // 引数は Color32[] ピクセルデータ
    //        int       width
    //        int       height
    [System.Serializable]
    public class ImgProcEvent : UnityEngine.Events.UnityEvent<Color32[], int, int> { };

    [SerializeField]
    ImgProcEvent imageProc = new ImgProcEvent();

    static WebCamTexture[] webcamTexture = null;
    static Texture2D[] outputTextureArray = null;
    Color32[] colors = null;

    // Use this for initialization
    void Start () {
        if (autoRun) { On(); }
    }

    public void camChange() {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length-1 > targetCamID && targetCamID>=0) { targetCamID++; }
        else { targetCamID = 0; }
        On(targetCamID);
    }

    public void On(int camID) {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length <= camID) return;


        if (webcamTexture == null) webcamTexture = new WebCamTexture[devices.Length];
        if (outputTextureArray == null) outputTextureArray = new Texture2D[devices.Length];

        if (webcamTexture[camID] == null)
        {
            webcamTexture[camID] = new WebCamTexture(devices[camID].name, this.width, this.height, this.fps);
        }
        else
        {
            if (webcamTexture[camID].width != this.width || webcamTexture[camID].height != this.height || (int)webcamTexture[camID].requestedFPS != this.fps)
            {
                if(webcamTexture[camID].isPlaying)webcamTexture[camID].Stop();
                webcamTexture[camID] = new WebCamTexture(devices[camID].name, this.width, this.height, this.fps);
            }
        }

        if (!webcamTexture[camID].isPlaying) {
            webcamTexture[camID].Play();
        }


        colors = new Color32[webcamTexture[camID].width * webcamTexture[camID].height];
        outputTexture = outputTextureArray[camID] = new Texture2D(webcamTexture[camID].width, webcamTexture[camID].height, TextureFormat.RGBA32, false);
        targetCamID = camID;

        if (GetComponent<Renderer>() != null)
        {
            GetComponent<Renderer>().material.mainTexture = outputTexture;
        }
        else if (GetComponent<RawImage>() != null)
        {
            GetComponent<RawImage>().texture = outputTexture;
        }
    }

    public void On() {
        On(targetCamID);
    }

    public void Off(int camID)
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length <= camID) return;
        if (webcamTexture[camID] == null)
        {
            return;
        }
        else {
            webcamTexture[camID].Stop();
            webcamTexture[camID] = null;
        }
    }

    public void Off() {
        Off(targetCamID);
    }


    // Update is called once per frame
    void Update () {
 
        if (colors != null)
        {
            webcamTexture[targetCamID].GetPixels32(colors);

            imageProc.Invoke(colors, width, height);

            outputTexture.SetPixels32(colors);
            outputTexture.Apply();
        }

    }

}
