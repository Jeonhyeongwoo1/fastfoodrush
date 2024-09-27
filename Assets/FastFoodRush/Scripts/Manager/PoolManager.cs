using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastFoodRush.Manager
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PoolManager>();
                }

                return instance;
            }
        }

        private static PoolManager instance;
        
        [Serializable]
        public struct PoolData
        {
            public GameObject prefab;
            public int size;
        }

        [SerializeField] private List<PoolData> _poolDataList;

        private Dictionary<string, List<GameObject>> _pooledDict = new();

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            foreach (PoolData poolData in _poolDataList)
            {
                int size = poolData.size;
                CreatePool(poolData.prefab, size);
            }
        }

        private void CreatePool(GameObject go, int size)
        {
            for (int i = 0; i < size; i++)
            {
                GameObject prefab = Instantiate(go, transform);
                string key = go.name;
                if (!_pooledDict.TryGetValue(key, out List<GameObject> list))
                {
                    list = new List<GameObject>();
                    _pooledDict.Add(key, list);
                }
                    
                list.Add(prefab);
                prefab.SetActive(false);
            }
        }

        private PoolData GetPoolData(string key)
        {
            PoolData poolData = _poolDataList.Find(v => v.prefab.name == key);
            return poolData;
        }

        public GameObject Get(string key)
        {
            List<GameObject> list = _pooledDict[key];
            if (list == null)
            {
                Debug.LogWarning($"{key} is null");
                return null;
            }
            
            GameObject go = list.Find(v => !v.activeSelf);
            if (go == null)
            {
                PoolData poolData = GetPoolData(key);
                CreatePool(poolData.prefab, poolData.size);
                return list.Find(v => !v.activeSelf);                
            }

            return go;
        }
    }
}










