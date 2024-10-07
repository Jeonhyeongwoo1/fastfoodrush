using System;
using System.Collections;
using FastFoodRush.Controller;
using FastFoodRush.Manager;
using UnityEngine;
using CameraType = FastFoodRush.Manager.CameraType;

namespace FastFoodRush.Tutorial
{
    public abstract class BaseTutorial : MonoBehaviour
    {
        public virtual bool IsAllCompletedTutorial { get; }
        public MainTutorialType MainTutorialType => _mainTutorialType;
        public abstract void ExecuteTutorial();
        public abstract void CompletedTutorial();
        public abstract void CompletedTutorialDepth();

        protected MainTutorialType _mainTutorialType;
    }
    
    public class MainTutorial : BaseTutorial
    {
        public override bool IsAllCompletedTutorial => _arrowTargetArray.Length == _currentTutorialDepth;

        [SerializeField] private GameObject _arrowImageObj;
        [SerializeField] private Object.Indicator _arrowIndicator;
        
        private float _arrowImageShowTime = 3f;
        private Transform[] _arrowTargetArray;
        private PlayerController _player;
        private Coroutine _executeTutorialCor;
        private Camera _camera;
        private bool _isCompletedTutorialDepth;
        private int _currentTutorialDepth;
        
        public void Initialized(PlayerController player, MainTutorialType mainTutorialType, params Transform[] arrowTargetArray)
        {
            _mainTutorialType = mainTutorialType;
            _arrowTargetArray = arrowTargetArray;
            _player = player;

            if (_camera == null)
            {
                _camera = Camera.main;
            }
            
            _arrowIndicator.gameObject.SetActive(false);
        }

        public override void CompletedTutorial()
        {
            if (_executeTutorialCor != null)
            {
                StopCoroutine(_executeTutorialCor);
            }
            
            Destroy(gameObject);
        }

        public override void CompletedTutorialDepth()
        {
            _isCompletedTutorialDepth = true;
        }

        private void ActivateOrDeactivateArrowImageObj(Transform target)
        {
            var position = _camera.WorldToViewportPoint(target.position);
            bool isInside = (position.x >= 0 && position.x <= 1) && (position.y >= 0 && position.y <= 1) &&
                            (position.z > 0);

            _arrowImageObj.SetActive(!isInside);
        }
        
        private void HandleIndicateArrow(Transform target)
        {
            if (_arrowTargetArray == null)
            {
                return;
            }
            
            Vector3 screenPosPlayer = _camera.WorldToScreenPoint(_player.transform.position);
            Vector3 screenPosTarget = _camera.WorldToScreenPoint(target.position);
            Vector2 directionOnScreen = screenPosTarget - screenPosPlayer;
            float angle = Mathf.Atan2(directionOnScreen.y, directionOnScreen.x) * Mathf.Rad2Deg;
            _arrowImageObj.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        public override void ExecuteTutorial()
        {
            /*
             * - 카메라로 플레이어가 이동해야할 장소를 보여줌
             * - 다시 플레이어로 카메라 이동
             * - 플레이어 하단에 화살표 표시
             */

            _executeTutorialCor = StartCoroutine(ExecuteTutorialCor());
        }
        
        private IEnumerator ChangeCameraCor(Transform followTarget, CameraType cameraType)
        {
            bool isCompleted = false;
            CameraManager cameraManager = CameraManager.Instance;
            cameraManager.SetFollowAndLookAtTarget(followTarget, null, cameraType);
            cameraManager.ChangeCamera(cameraType, 1, ()=> isCompleted = true);
            yield return new WaitUntil(() => isCompleted);
        }
        
        private IEnumerator ExecuteTutorialCor()
        {
            for (int i = 0; i < _arrowTargetArray.Length; i++)
            {
                _arrowIndicator.gameObject.SetActive(true);
                _isCompletedTutorialDepth = false;
                SimpleInput.IsStop = true;
                yield return new WaitForSeconds(0.5f);
                Transform target = _arrowTargetArray[i];
                _arrowIndicator.DoIndicate(target);
                yield return ChangeCameraCor(target, CameraType.Tutorial);
                yield return new WaitForSeconds(1);
                yield return ChangeCameraCor(_player.transform, CameraType.Player);

                SimpleInput.IsStop = false;
                StartCoroutine(ShowIndicatorArrowCor(target));
                yield return new WaitUntil(() => _isCompletedTutorialDepth);

                _arrowIndicator.gameObject.SetActive(false);
                _currentTutorialDepth++;
            }

            TutorialManager.Instance.CheckMainTutorialCompletion(_mainTutorialType);
        }

        private IEnumerator ShowIndicatorArrowCor(Transform target)
        {
            float elapsed = 0;
            while (true)
            {
                elapsed += Time.deltaTime;

                if (elapsed > _arrowImageShowTime)
                {
                    break;
                }
                HandleIndicateArrow(target);
                ActivateOrDeactivateArrowImageObj(target);

                yield return null;
            }
            _arrowImageObj.SetActive(false);
        }
    }
}