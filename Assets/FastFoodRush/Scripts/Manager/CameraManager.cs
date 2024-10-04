using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FastFoodRush.Manager
{
    public enum CameraType
    {
        Player,
        Tutorial
    }
    
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CameraManager>();
                }

                return _instance;
            }
        }

        private static CameraManager _instance;

        public CameraType CurrentLiveCameraType { get; private set; }
        public ICinemachineCamera LiveCamera => _brain.ActiveVirtualCamera;
        
        [Serializable]
        public struct CameraData
        {
            public CameraType cameraType;
            public CinemachineVirtualCamera camera;
        }

        [SerializeField] private List<CameraData> _cameraList;

        private CinemachineBrain _brain;

        private void Start()
        {
            _brain = Camera.main.GetComponent<CinemachineBrain>();
        }

        public void SetFollowAndLookAtTarget(Transform followTarget, Transform lookAtTarget, CameraType cameraType)
        {
            _cameraList.ForEach(v =>
            {
                if (v.cameraType == cameraType)
                {
                    v.camera.Follow = followTarget;
                    v.camera.LookAt = lookAtTarget;
                }
            });
        }
        
        [Button]
        public void ChangeCamera(CameraType cameraType, float blendingTime, Action done = null)
        {
            foreach (CameraData cameraData in _cameraList)
            {
                if (cameraData.cameraType == cameraType)
                {
                    StartCoroutine(ChangeCameraCor(cameraType, cameraData.camera, blendingTime, done));
                    break;
                }            
            }
        }

        private IEnumerator ChangeCameraCor(CameraType cameraType, CinemachineVirtualCamera camera, float blendingTime,  Action done)
        {
            //현재 변경중인게 있다면 대기
            while (_brain.IsBlending)
            {
                yield return null;
            }

            CurrentLiveCameraType = cameraType;
            _brain.m_DefaultBlend.m_Time = blendingTime;
            _brain.ActiveVirtualCamera.VirtualCameraGameObject.SetActive(false);
            camera.gameObject.SetActive(true);

            yield return null;
            while (_brain.IsBlending)
            {
                yield return null;
            }
            
            Debug.LogWarning(_brain.IsBlending);
            done?.Invoke();
        }
        
    }
}
