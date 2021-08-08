using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;

namespace HooahUtility.Editor
{
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "FormPrefabs", menuName = "FormPrefabs for FormContentManagers", order = 1)]
#endif
    public class FormPrefabs : ScriptableObject, ISerializationCallbackReceiver
    {
        [Serializable]
        public struct FormPrefab
        {
            public string id;
            public GameObject prefab;
        }

        // This will be null anyway
        // Thanks obama
        [SerializeField] private FormPrefab[] prefabs = { };
        [SerializeField, HideInInspector] private string[] compID = { };
        [SerializeField, HideInInspector] private GameObject[] compPrefab = { };
        private Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();

        public bool TryGetForm(string key, out GameObject prefab)
        {
            return _prefabs.TryGetValue(key, out prefab);
        }

        // PROC STRUCTURE -> FLATTEN STORAGE -> RUNTIME OPTIMIZED STRUCTURE

        public void OnBeforeSerialize()
        {
            // struct to flatten arrays
            _prefabs = prefabs.Distinct().ToDictionary(x => x.id, x => x.prefab);
            compID = _prefabs.Keys.ToArray();
            compPrefab = _prefabs.Values.ToArray();
        }

        public void OnAfterDeserialize()
        {
            // flattern arrays to runtime dictionary
            _prefabs = new Dictionary<string, GameObject>();
            for (var i = 0; i != Math.Min(compID.Length, compPrefab.Length); i++)
            {
                _prefabs.Add(compID[i], compPrefab[i]);
            }
        }
    }
}
