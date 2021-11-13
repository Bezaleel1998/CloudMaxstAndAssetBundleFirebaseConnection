using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Net;
using System.IO;

public class API : MonoBehaviour {
    
    #region Get_Asset_Bundle_Cloud_FireBase_Google 


    [SerializeField]
    internal string BundleFolder = "https://storage.googleapis.com/unityassetbundleproject123.appspot.com/AssetBundles/";
    public GameManager gm;

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
        yield return www.SendWebRequest();

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
            }
            else
            {
                Debug.Log("Not a valid asset bundle");
            }
        }
    }

    #endregion

}
