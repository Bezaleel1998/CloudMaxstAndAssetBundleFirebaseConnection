using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class LoadAddressableScene : MonoBehaviour
{
    [Header("Connection To Database Firebase")]
    [SerializeField] private AssetReference _assetReferenceScene;
    //in case of multiple scenes
    [SerializeField] private List<AssetReference> _references = new List<AssetReference>();
    private AsyncOperationHandle<SceneInstance> handle;
    [Header("GameObject")]
    public GameObject camera;
    




}
