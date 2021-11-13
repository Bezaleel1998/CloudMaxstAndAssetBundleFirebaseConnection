using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataBaseCharacters
{
    [CreateAssetMenu(fileName = "Data", menuName = "DataBaseCharacters/DataProfileCharacters", order = 1)]
    public class DataCharacters : ScriptableObject
    {

        [Header("Character Profiles")]
        [Tooltip("u need to input the char name at here")]
        public string charName;
        
    }

}
