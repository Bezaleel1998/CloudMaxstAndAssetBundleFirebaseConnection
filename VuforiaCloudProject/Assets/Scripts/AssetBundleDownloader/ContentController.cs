using UnityEngine;
using UnityEngine.UI;

public class ContentController : MonoBehaviour {

    public API api;
    public GameObject parent;
    public GameManager gm;

    [Header("UI")]
    public GameObject loadingStatus;
    public Image downloadFillAmount;
    public Text percentage;
    public Text status;

    private void Awake()
    {
        loadingStatus.SetActive(false);
        api.isDone = true;
    }

    private void FixedUpdate()
    {

        LoadingBarFunction(api.m_CurrentValue, downloadFillAmount, api.assetN, status, percentage);

        if (!api.isDone)
        {
            loadingStatus.SetActive(true);
        }
        else
        {
            loadingStatus.SetActive(false);
        }

    }

    public void LoadContent(string name) {

        //DestroyAllChildren();
        api.isDone = false;
        api.GetBundleObject(name, OnContentLoaded, parent.transform);

    }

    void OnContentLoaded(GameObject content) {
        
        //do something cool here
        Debug.Log("Loaded: " + content.name);
        gm.nameText.text = "<color=lime>3D Loaded : " + content.name + "</color>";

    }

    void DestroyAllChildren() {
        foreach (Transform child in parent.transform) {

            Destroy(child.gameObject);
        
        }
    }

    void LoadingBarFunction(float currentValue, Image fillAmountImage, string assetName, Text status, Text percentage)
    {

        float a = 0;

        if (!api.isDone)
        {

            if (fillAmountImage.fillAmount != 1f)
            {

                fillAmountImage.fillAmount = currentValue;
                Debug.Log("Download Progress = " + currentValue + ", Data Name = " + assetName);

                a = (int)(currentValue * 100);
                if (a > 0 && a <= 33)
                {
                    status.text = "Loading...";
                }
                else if (a > 33 && a <= 67)
                {
                    status.text = "Downloading " + assetName + " 3D";
                }
                else if (a > 67 && a <= 100)
                {
                    status.text = "Please wait...";
                }

                percentage.text = a + "%";

            }
            else
            {
                fillAmountImage.fillAmount = 0.0f;
                percentage.text = "0%";
            }

        }

    }

}
