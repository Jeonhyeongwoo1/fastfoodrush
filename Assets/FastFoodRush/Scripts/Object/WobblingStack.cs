using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FastFoodRush.Manager;
using UnityEngine;

namespace FastFoodRush.Interactable
{
    public class WobblingStack : MonoBehaviour
    {
        public int StackCount => _stackList.Count;
        public int Height => _height;
        public StackType CurrentStackType => _currentStackType;

        [SerializeField] private GameObject _tray;
        [SerializeField] private Vector3 _offset = new Vector3(0, 0.25f, 0);
        [SerializeField] private StackType _currentStackType;
        [SerializeField] private List<GameObject> _stackList = new();

        private int _height;
        
        private void Update()
        {
            if (_stackList.Count == 0)
            {
                return;
            }

            _stackList[0].transform.position = _tray.transform.position;
            _stackList[0].transform.rotation = _tray.transform.rotation;
            
            for (int i = 0; i < _stackList.Count; i++)
            {
                float rate = Mathf.Lerp(1f, 0.1f, i / (float) _stackList.Count);
                GameObject go = _stackList[i];
                Vector3 prevPosition = go.transform.position;
                Vector3 position = Vector3.Lerp(prevPosition, _tray.transform.position + _offset * i, rate);
                Quaternion rotation = _tray.transform.rotation;
                go.transform.position = position;
                go.transform.rotation = rotation;
            }
        }

        public void Stack(GameObject obj, StackType stackType)
        {
            if (_currentStackType != StackType.None && _currentStackType != stackType)
            {
                return;
            }
            
            if (!_tray.activeSelf)
            {
                _tray.SetActive(true);
            }

            Vector3 endValue = _tray.transform.position + _offset * _stackList.Count;
            _height++;
            _currentStackType = stackType;

            obj.transform.DOJump(endValue, 2, 1, 0.25f).OnComplete(()=>_stackList.Add(obj));
        }

        public GameObject Peek() => _stackList.LastOrDefault();

        public GameObject Pop()
        {
            GameObject go = _stackList.LastOrDefault();
            _stackList.Remove(go);
            _height--;
            if (_stackList.Count == 0)
            {
                _tray.SetActive(false);
                _currentStackType = StackType.None;
            }
            
            return go;
        }
    }
}