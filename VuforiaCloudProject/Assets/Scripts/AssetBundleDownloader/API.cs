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
        isDone = false;
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

}
