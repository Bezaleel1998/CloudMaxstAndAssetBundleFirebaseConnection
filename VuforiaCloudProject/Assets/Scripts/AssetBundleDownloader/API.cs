using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Net;
using System.IO;
using UnityEngine.UI;

public class API : MonoBehaviour {

    #region Get_Asset_Bundle_Cloud_FireBase_Google 

    [Header("Connection To Database Firebase")]
    [SerializeField]
    internal string BundleFolder = "https://storage.googleapis.com/unityassetbundleproject123.appspot.com/AssetBundles/";
    public GameManager gm;
    internal float m_CurrentValue;
    internal string assetN;
    internal bool isDone;
    //public string savePath;

    public void GetBundleObject(string assetName, UnityAction<GameObject> callback, Transform bundleParent)
    {
        StartCoroutine(GetDisplayBundleRoutine(assetName, callback, bundleParent));
    }

    IEnumerator GetDisplayBundleRoutine(string assetName, UnityAction<GameObject> callback, Transform bundleParent)
    {

        string bundleURL = BundleFolder + assetName + "-";

        //append platform to asset bundle name
#if UNITY_ANDROID
        bundleURL += "Android";
#else
        bundleURL += "IOS";
#endif

        Debug.Log("Requesting bundle at " + bundleURL);

        //request asset bundle
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(bundleURL);

        StartCoroutine(WaitForResponse(www));
        assetN = assetName;

        yield return www.SendWebRequest();

        //www.downloadHandler = new DownloadHandlerFile(savePath + fileName);

        if (www.isNetworkError)
        {
            Debug.Log("Network error");
        }
        else
        {

            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
            if (bundle != null)
            { 

                string rootAssetPath = bundle.GetAllAssetNames()[0];
                GameObject arObject = Instantiate(bundle.LoadAsset(rootAssetPath) as GameObject, bundleParent);
                bundle.Unload(false);

                gm.model3D.Add(arObject);

                callback(arObject);
                isDone = true;

            }
            else
            {
                Debug.Log("Not a valid asset bundle");
            }

        }
    }


    IEnumerator WaitForResponse(UnityWebRequest request)
    {
        
        while (!request.isDone)
        {

            m_CurrentValue = request.downloadProgress;
            //Debug.Log("Download Progress = " + m_CurrentValue);

            yield return new WaitForSeconds(0.2f);

        }

    }

    #endregion

    #region Download_Tracker(Image)_From_Websites

    [SerializeField] private string urlDataBaseFirebase = "https://storage.googleapis.com/unityassetbundleproject123.appspot.com/AssetBundles/TrackerImage/";
    [SerializeField] Image textureImage;
    [SerializeField] string[] fileName;
    //

    /// <summary>
    /// click event - load image from web URL 
    /// </summary>
    public void OnLoadImageFromWebButtonClick()
    {

        for (int i = 0; i < fileName.Length; i++)
        {

            string addURL = fileName[i] + ".jpeg";
            StartCoroutine(LoadTextureFromWeb(urlDataBaseFirebase, addURL));

        }

    }

    // enumerator to load texture from web URL
    IEnumerator LoadTextureFromWeb(string url, string assetName)
    {
        string fullURL = url + assetName;
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(fullURL);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            Texture2D loadedTexture = DownloadHandlerTexture.GetContent(www);
            textureImage.sprite = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), Vector2.zero);
            textureImage.SetNativeSize();
        }
    }

    /// <summary>
    /// click event - load image from disk 
    /// </summary>
    public void OnLoadImageFromDiskButtonClick()
    {
        if (!File.Exists(Application.persistentDataPath + fileName))
        {
            Debug.LogError("File Not Exist!");
            return;
        }

        LoadImageFromDisk();
    }

    // load texture bytes from disk and compose sprite from bytes
    private void LoadImageFromDisk()
    {
        byte[] textureBytes = File.ReadAllBytes(Application.persistentDataPath + fileName);
        Texture2D loadedTexture = new Texture2D(0, 0);
        loadedTexture.LoadImage(textureBytes);
        textureImage.sprite = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), Vector2.zero);
        textureImage.SetNativeSize();

    }

    /// <summary>
    /// click event - save image to disk 
    /// </summary>
    public void OnSaveImageButtonClick()
    {
        if (textureImage.sprite == null)
        {
            Debug.LogError("No Image to Save!");
            return;
        }

        WriteImageOnDisk();
    }

    // generate texture bytes and save to disk
    private void WriteImageOnDisk()
    {
        byte[] textureBytes = textureImage.sprite.texture.EncodeToPNG();
        File.WriteAllBytes(Application.persistentDataPath + fileName, textureBytes);
        Debug.Log("File Written On Disk!");
    }

    /// <summary>
    /// click event - make image blank 
    /// </summary>
    public void OnBlankImageButtonClick()
    {
        textureImage.sprite = null;
    }


    #endregion

}
