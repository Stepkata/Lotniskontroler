using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class XRayPlate
{
    public Texture2D Image;
    public Rect Groundtruth;
}

public class ImageLoader : MonoBehaviour
{
    public XRayPlate CurrentPlate;
    public UnityEvent<XRayPlate> LoadingFinishedEvent;

    private string[] imageFiles;
    private int fileCount;

    void Start()
    {
        if (LoadingFinishedEvent == null) LoadingFinishedEvent = new UnityEvent<XRayPlate>();
        StartCoroutine(loadImageFiles());
        CurrentPlate = new XRayPlate();
    }

    public void LoadRandom()
    {
        StartCoroutine(loadRandomCoroutine());
    }

    IEnumerator loadImageFiles()
    {
        string manifest = Path.Combine(Application.streamingAssetsPath, "image_files.txt");
        using (UnityWebRequest req = UnityWebRequest.Get(manifest))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(req.error);
            }
            else
            {
                imageFiles = req.downloadHandler.text.Split(new string[] { System.Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries);
                fileCount = imageFiles.Length;
                Debug.Log($"Loaded {fileCount} files");
            }
        }
    }

    IEnumerator loadRandomCoroutine()
    {
        var rand = Random.Range(0, fileCount);
        var imageFile = Path.Combine(Application.streamingAssetsPath, "LuggageData", "images", imageFiles[rand]);
        Debug.Log($"Loading {imageFile}");
        var labelFile = Path.Combine(Application.streamingAssetsPath, "LuggageData", "labels", Path.GetFileName(Path.ChangeExtension(imageFile, ".txt")));
        Debug.Log($"Loading {labelFile}");
        using (UnityWebRequest req1 = UnityWebRequestTexture.GetTexture(imageFile), req2 = UnityWebRequest.Get(labelFile))
        {
            yield return req1.SendWebRequest();
            yield return req2.SendWebRequest();

            if (req1.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(req1.error);
            }
            else
            {
                Debug.Log("Loaded image");
                CurrentPlate.Image = DownloadHandlerTexture.GetContent(req1);
            }
            
            if (req2.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(req2.error);
            }
            else
            {
                Debug.Log($"Loaded labels");
                var text = req2.downloadHandler.text;
                string[] strings = text.Split();
                CurrentPlate.Groundtruth = new Rect(0, 0, 0, 0);
                CurrentPlate.Groundtruth.xMin = float.Parse(strings[2], System.Globalization.CultureInfo.InvariantCulture);
                CurrentPlate.Groundtruth.yMin = float.Parse(strings[3], System.Globalization.CultureInfo.InvariantCulture);
                CurrentPlate.Groundtruth.xMax = float.Parse(strings[0], System.Globalization.CultureInfo.InvariantCulture);
                CurrentPlate.Groundtruth.yMax = float.Parse(strings[1], System.Globalization.CultureInfo.InvariantCulture);
                LoadingFinishedEvent.Invoke(CurrentPlate);
            }
        }
    }
}
