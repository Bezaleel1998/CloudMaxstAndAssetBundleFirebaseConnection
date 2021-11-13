using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DataBaseCharacters;
//for video
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    #region Private_Variable 
    //This is private Variable Sector
    //string, numeric and etc
    [Header("String and Numeric")]
    [SerializeField]
    private string theChosenOne;

    [Header("GameObject Caller")]
    //GameObject CALLER
    [SerializeField]
    internal List<GameObject> model3D = new List<GameObject>();

    [SerializeField]
    private DataCharacters[] charDatas;



    #endregion

    #region Public_Variable
    //This is public Variable Sector

    public GameObject parent3D;
    [Header("UI GameObject")]
    public Text nameText;

    #endregion

    void Awake()
    {
        //select all char data inside saveddatasector folder in Resources
        charDatas = Resources.LoadAll("SavedDataSector", typeof(DataCharacters)).Cast<DataCharacters>().ToArray();

    }

    // Update is called once per frame
    void Update()
    {


        //isDetected();
        if (model3D.Count > 0)
        {
            CharacterSelection();
            nameText.text = "";
        }
        else
        {
            nameText.text = "There's No 3D that has been downloaded... please download 3D asset first !";
        }

    }


    private void CharacterSelection()
    {
        theChosenOne = PlayerPrefs.GetString("MaxStTrackerCloudName").ToLower();
        Debug.Log("<color=red>MaxStTrackerCloudName is = " + theChosenOne + "</color>");


        for (int i = 0; i < model3D.Count; i++)
        {

            for (int j = 0; j < charDatas.Length; j++)
            {

                if (model3D[i].name.Contains(theChosenOne) && theChosenOne.Contains(charDatas[j].charName.ToString()))
                {

                    model3D[i].SetActive(true);
                    
                }
                else if (theChosenOne == "")
                {

                    model3D[i].SetActive(false);
                    
                }

            }
        }
       
    }


    


}
