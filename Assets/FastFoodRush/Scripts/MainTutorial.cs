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
        public MainTutorialType MainTutorialType => _mainTutorialType;
        public abstract void ExecuteTutorial();
        public abstract void CompletedTutorial();

        protected MainTutorialType _mainTutorialType;
    }
    
    public class MainTutorial : BaseTutorial
    {
        [SerializeField] private GameObject _arrowImageObj;
        [SerializeField] private FastFoodRush.Object.Indicator _arrowIndicator;
        
        [SerializeField] private Transform _arrowtarget;
        [SerializeField] private PlayerController _player;

        private Coroutine _executeCommonTutorialCor;
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        public void Initialized(PlayerController player, Transform arrowTarget, MainTutorialType mainTutorialType)
        {
            _mainTutorialType = mainTutorialType;
            _arrowtarget = arrowTarget;
            _player = player;
            _arrowIndicator.DoIndicate(_arrowtarget);
        }

        public override void CompletedTutorial()
        {
            if (_executeCommonTutorialCor != null)
            {
                StopCoroutine(_executeCommonTutorialCor);
            }
            
            Destroy(gameObject);
        }

        private void ActivateOrDeactivateArrowImageObj()
        {
            var position = _camera.WorldToViewportPoint(_arrowtarget.position);
            bool isInside = (position.x >= 0 && position.x <= 1) && (position.y >= 0 && position.y <= 1) &&
                            (position.z > 0);

            _arrowImageObj.SetActive(!isInside);
        }
        
        private void HandleIndicateArrow()
        {
            if (_arrowtarget == null)
            {
                return;
            }
            
            Vector3 screenPosPlayer = _camera.WorldToScreenPoint(_player.transform.position);
            Vector3 screenPosTarget = _camera.WorldToScreenPoint(_arrowtarget.position);
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

            _executeCommonTutorialCor = StartCoroutine(ExecuteBasicTutorialCor());
        }

        private IEnumerator ExecuteBasicTutorialCor()
        {
            bool isCompleted = false;

            SimpleInput.IsStop = true;
            yield return new WaitForSeconds(1f);
            
            CameraManager cameraManager = CameraManager.Instance;
            cameraManager.SetFollowAndLookAtTarget(_arrowtarget.transform, null, CameraType.Tutorial);
            cameraManager.ChangeCamera(CameraType.Tutorial, 1, ()=> isCompleted = true);
            
            yield return new WaitUntil(() => isCompleted);
            yield return new WaitForSeconds(2f);

            isCompleted = false;
            cameraManager.ChangeCamera(CameraType.Player, 1, ()=> isCompleted = true);

            yield return new WaitUntil(() => isCompleted = true);
            
            SimpleInput.IsStop = false;
            while (true)
            {
                HandleIndicateArrow();
                ActivateOrDeactivateArrowImageObj();

                yield return null;
            }
        }
    }
}