using UnityEngine;
using System.Collections;
using System;

public class CamreJunk : MonoBehaviour
{


    public Transform TopLeftCoord = null;
    public Transform TopRightCoord = null;
    public Transform BottomLeftCoord = null;
    public Transform BottomRightCoord = null;


    public int resWidth = 2550;
    public int resHeight = 3300;

    public static void ScreenShotName(int width, int height, out string imagePath, out string coordsPath)
    {
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        imagePath = string.Format(@"C:\Users\Mike\Desktop\dumpMaps\screen_{1}x{2}_{3}.png",
                             Application.dataPath,
                             width, height,
                             timestamp);
        coordsPath = string.Format(@"C:\Users\Mike\Desktop\dumpMaps\screen_{1}x{2}_{3}.txt",
                     Application.dataPath,
                     width, height,
                     timestamp);
    }

    private bool hasTakenShot = false;

    DateTime lastSnapShot = DateTime.Now;
    float waitTime = 2;
    float currWait = 0;

    void LateUpdate()
    {
        if (hasTakenShot)
        {
            currWait = 0;
            return;
        }

        currWait += Time.deltaTime;

        if (currWait < waitTime)
        {
            return;
        }


        hasTakenShot = true;

            string textFilePath = null;
            string imageFilePath = null;
            ScreenShotName(resWidth, resHeight, out imageFilePath, out textFilePath);

            Debug.Log("making screen shot " + imageFilePath);

            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            GetComponent<Camera>().targetTexture = rt;

            Vector3 TopLeftScreen = GetComponent<Camera>().WorldToScreenPoint(TopLeftCoord.position);
            Vector3 TopRightScreen = GetComponent<Camera>().WorldToScreenPoint(TopRightCoord.position);
            Vector3 BottomLeftScreen = GetComponent<Camera>().WorldToScreenPoint(BottomLeftCoord.position);
            Vector3 BottomRightScreen = GetComponent<Camera>().WorldToScreenPoint(BottomRightCoord.position);

            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            GetComponent<Camera>().Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            GetComponent<Camera>().targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            System.IO.File.WriteAllBytes(imageFilePath, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", imageFilePath));

            string content = string.Format("\n{0} -> {1}\n{2} -> {3}\n{4} -> {5}\n{6} -> {7}", 
                                            TopLeftScreen, TopLeftCoord.position, 
                                            TopRightScreen, TopRightCoord.position,
                                            BottomLeftScreen, BottomLeftCoord.position,
                                            BottomRightScreen, BottomRightCoord.position);

            System.IO.File.WriteAllText(textFilePath, content);

    }





    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

