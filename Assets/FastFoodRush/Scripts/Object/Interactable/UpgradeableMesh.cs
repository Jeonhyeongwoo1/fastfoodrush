using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FastFoodRush.Object
{
    public class UpgradeableMesh : MonoBehaviour
    {
        [SerializeField] private List<Mesh> _mesheList;

        private MeshFilter _meshFilter;
        [SerializeField]   private int _level;
        
        private void Start()
        {
            TryGetComponent(out MeshFilter meshFilter);
            _meshFilter = meshFilter;
        }

        public void UpdateMesh()
        {
            if (_mesheList.Count <= _level)
            {
                Debug.LogWarning($"failed update mesh {transform.parent.name}/{transform.name}");
                return;
            }

            Mesh mesh = _mesheList[_level];
            if (_meshFilter == null)
            {
                Debug.LogWarning($"There isn't mesh filter {transform.parent.name}/{transform.name}");
                return;
            }

            _level++;            
            _meshFilter.mesh = mesh;
        }
    }
}
