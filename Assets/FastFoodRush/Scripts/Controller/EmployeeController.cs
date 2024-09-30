using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FastFoodRush.Interactable;
using FastFoodRush.Manager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FastFoodRush.Controller
{
    public class EmployeeController : BaseController
    {
        public enum BehaviourType
        {
            CleanTrash,
            CarryFood,
        }

        private int _employeeMoveSpeed;
        private int _employeeCapacity;
        private Coroutine _processLoginCor;

        protected override void Start()
        {
            base.Start();
            
            _processLoginCor = StartCoroutine(ProcessLogic());
        }

        private void Update()
        {
            _animator.SetBool(_isMovingHash, !HasArrived());
        }

        private void OnEnable()
        {
            RestaurantManager.Instance.onUpgradedAbility += OnUpgradeAbility;
        }

        private void OnDisable()
        {
            RestaurantManager.Instance.onUpgradedAbility -= OnUpgradeAbility;
        }

        public void Sleep()
        {
            StopLogic();
            gameObject.SetActive(false);
        }

        public void StopLogic()
        {
            if (_processLoginCor != null)
            {
                StopCoroutine(_processLoginCor);
                _processLoginCor = null;
            }
        }

        private IEnumerator ProcessLogic()
        {
            yield return new WaitForSeconds(0.2f);
            
            while (true)
            {
                var length = Enum.GetNames(typeof(BehaviourType)).Length;
                int select = Random.Range(0, length);
                
                switch ((BehaviourType) select)
                {
                    case BehaviourType.CleanTrash:
                        yield return CleanTrashCor();
                        break;
                    case BehaviourType.CarryFood:
                        yield return CarryFoodCor();
                        break;
                }

                // if (!HasArrived())
                // {
                //     _agent.isStopped = true;
                // }
                
                yield return new WaitForSeconds(0.3f);
            }
        }

        private IEnumerator CarryFoodCor()
        {
            List<FoodPile> foodPileList = RestaurantManager.Instance.Piles.OfType<FoodPile>().ToList().FindAll(v=> v.StackCount > 0);
            if (foodPileList.Count > 0)
            {
                int random = Random.Range(0, foodPileList.Count);
                FoodPile foodPile = foodPileList[random];

                _agent.SetDestination(foodPile.transform.position);

                yield return new WaitUntil(() => HasArrived());

                while (foodPile.StackCount > 0)
                {
                    if (_wobblingStack.StackCount >= _employeeCapacity)
                    {
                        break;
                    }

                    GameObject obj = foodPile.RemoveStack();
                    _wobblingStack.Stack(obj, foodPile.StackType);
                    yield return new WaitForSeconds(0.2f);
                }
            }

            if (_wobblingStack.StackCount == 0)
            {
                yield break;
            }

            ObjectStack objectStack = RestaurantManager.Instance.ObjectStacks.Find(v => v.StackType == StackType.Food);
            if (objectStack == null)
            {
                Debug.LogWarning($"can't find object stack");
                yield break;
            }

            Vector3 position = objectStack.ObjectStackPointPosition;
            _agent.SetDestination(position);

            yield return new WaitUntil(() => HasArrived());

            while (_wobblingStack.StackCount > 0)
            {
                GameObject obj = _wobblingStack.Pop();
                objectStack.Stack(obj);
                yield return new WaitForSeconds(0.25f);
            }
        }

        private IEnumerator CleanTrashCor()
        {
            List<TrashPile> trashPileList = RestaurantManager.Instance.Piles.OfType<TrashPile>()
                                                                                .ToList()
                                                                                .FindAll(v => v.IsExistObject);

            if (trashPileList.Count == 0)
            {
                yield break;
            }
            
            int random = Random.Range(0, trashPileList.Count);
            TrashPile trashPile = trashPileList[random];

            _agent.SetDestination(trashPile.ObjectStackPointPosition);

            yield return new WaitUntil(() => HasArrived());

            const float interval = 0.1f;
            while (trashPile.IsExistObject)
            {
                if (_wobblingStack.StackCount >= _employeeCapacity)
                {
                    break;
                }
                
                GameObject obj = trashPile.RemoveStack();
                if (obj == null)
                {
                    break;
                }
                
                _wobblingStack.Stack(obj, StackType.Trash);
                yield return new WaitForSeconds(interval);
            }

            if (_wobblingStack.StackCount <= 0)
            {
                yield break;
            }
            
            //MoveTo TrashBin
            TrashBin trashBin = RestaurantManager.Instance.TrashBin;
            _agent.SetDestination(trashBin.transform.position);
            yield return new WaitUntil(() => HasArrived());

            while (_wobblingStack.StackCount > 0)
            {
                GameObject obj = _wobblingStack.Pop();
                trashBin.RemoveTrash(obj);
                yield return new WaitForSeconds(interval);
            }
        }
        
        public void Initialize(Vector3 spawnPosition, float speed, int capacity)
        {
            _agent.speed = speed;
            _employeeCapacity = capacity;
            transform.position = spawnPosition;
            gameObject.SetActive(true);
        }

        private void OnUpgradeAbility(AbilityType abilityType, float ability)
        {
            switch (abilityType)
            {
                case AbilityType.EmployeeSpeed:
                    _agent.speed = ability;
                    break;
                case AbilityType.EmployeeCapacity:
                    _employeeCapacity = (int)ability;
                    break;
             }
        }

        public void OnStep(AnimationEvent animationEvent)
        {
            // if (animationEvent.animatorClipInfo.weight < 0.5f) return;
            //
            // audioSource.clip = footsteps[Random.Range(0, footsteps.Length)];
            // audioSource.Play();
        }
    }
}



































