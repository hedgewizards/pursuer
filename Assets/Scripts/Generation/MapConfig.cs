using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    [CreateAssetMenu(fileName = "new config", menuName = "ScriptableObjects/Generation/MapConfig", order = 1)]
    public class MapConfig : ScriptableObject
    {
        public string OptionText;
        public GameObject LevelPrefab;
    }
}
