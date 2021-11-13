using UnityEngine;

public class ContentController : MonoBehaviour {

    public API api;
    public GameObject parent;
    public GameManager gm;

    public void LoadContent(string name) {

        //DestroyAllChildren();
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
}
