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
            CarryPackage,
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
            RestaurantManager.Instance.onUpgradedAbilityAction += OnUpgradeAbility;
        }

        private void OnDisable()
        {
            RestaurantManager.Instance.onUpgradedAbilityAction -= OnUpgradeAbility;
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
                        yield return ReFill<FoodPile>(StackType.Food);
                        break;
                    case BehaviourType.CarryPackage:
                        yield return ReFill<PackagePile>(StackType.Package);
                        break;
                }

                // if (!HasArrived())
                // {
                //     _agent.isStopped = true;
                // }
                
                yield return new WaitForSeconds(0.3f);
            }
        }

        private IEnumerator ReFill<T>(StackType stackType) where T : Pile
        {
            List<T> pileList = RestaurantManager.Instance.Piles.OfType<T>().ToList()
                .FindAll(v => v.StackCount > 0);
            if (pileList.Count > 0)
            {
                int random = Random.Range(0, pileList.Count);
                T pile = pileList[random];
                _agent.SetDestination(pile.transform.position);

                yield return new WaitUntil(() => HasArrived());

                while (pile.StackCount > 0)
                {
                    if (_wobblingStack.Height >= _employeeCapacity)
                    {
                        break;
                    }

                    GameObject obj = pile.RemoveStack();
                    _wobblingStack.Stack(obj, pile.StackType);
                    yield return new WaitForSeconds(0.2f);
                }
            }

            yield return new WaitForSeconds(0.25f);
            
            if (_wobblingStack.StackCount == 0)
            {
                yield break;
            }
            
            List<ObjectStack> objectStackList = RestaurantManager.Instance.ObjectStacks.FindAll(v => v.StackType == stackType);
            if(objectStackList.Count == 0)
            {
                Debug.LogWarning($"can't find object stack");
                yield break;
            }

            ObjectStack selectedObjectStack = objectStackList[Random.Range(0, objectStackList.Count)]; 
            Vector3 position = selectedObjectStack.ObjectStackPointPosition;
            _agent.SetDestination(position);

            yield return new WaitUntil(() => HasArrived());

            while (_wobblingStack.StackCount > 0)
            {
                GameObject obj = _wobblingStack.Pop();
                selectedObjectStack.Stack(obj);
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
                if (_wobblingStack.Height >= _employeeCapacity)
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

            yield return new WaitForSeconds(0.25f);
            
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



































