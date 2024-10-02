using System;
using System.Collections;
using FastFoodRush.Controller;
using FastFoodRush.Interactable;
using FastFoodRush.Manager;
using UnityEngine;

namespace FastFoodRush.Object
{
    public class CustomerController : BaseController, IOrderable
    {
        private enum State
        {
            Idle,
            MoveToCounterTable,
            MoveToSit,
            Eat,
            Leave
        }

        public bool IsReadyOrder { get; private set; }
        public int RemainOrderCount => _orderCount - _wobblingStack.StackCount;
        public int Height => _height;
        public int OrderCount => _orderCount;
        public Transform Transform => transform;

        [SerializeField] private State _state;
        [SerializeField] private CustomerAIConfigData _data;

        private int _height;
        private int _orderCount;
        private Vector3 _despawnPosition;
        private Coroutine _moveToCor;
        
        private int _sitHash;
        private int _eatHash;
        private int _leaveHash;

        private readonly string _entranceLayer = "Entrance";
        
        protected override void Start()
        {
            _isMovingHash = Animator.StringToHash("IsMoving");
            _sitHash = Animator.StringToHash("Sit");
            _eatHash = Animator.StringToHash("Eat");
            _leaveHash = Animator.StringToHash("Leave");

            StartCoroutine(CheckDoorCor());
        }

        private void OnDisable()
        {
            _animator.speed = 1;
            _orderCount = 0;
            _height = 0;
        }

        private void Update()
        {
            bool isMoving = !HasArrived();
            _animator.SetBool(_isMovingHash, isMoving);
        }

        private IEnumerator CheckDoorCor()
        {
            while (true)
            {
                RaycastHit hit;

                while (!Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, 2.5f, LayerMask.GetMask(_entranceLayer), QueryTriggerInteraction.Collide))
                {
                    yield return null;
                }

                var doors = hit.transform.GetComponentsInChildren<Door>();
                foreach (var door in doors)
                {
                    door.OpenDoor(transform.position);
                }

                yield return new WaitForSeconds(1f);

                foreach (var door in doors)
                {
                    door.CloseDoor();
                }
                
                yield return new WaitForSeconds(0.2f);
            }
        }
        
        private void UpdateState(State state)
        {
            _state = state;
        }

        public bool TryReceivedOrderInfo(GameObject orderObj)
        {
            if (RemainOrderCount == 0)
            {
                return false;
            }

            _height++;
            _wobblingStack.Stack(orderObj, StackType.Food);

            return true;
        }
        
        public void UpdateQueuePosition(Vector3 position)
        {
            UpdateState(State.MoveToCounterTable);
            MoveTo(position);
        }

        private IEnumerator MoveToCor(Vector3 destinationPosition, Action done)
        {
            _agent.SetDestination(destinationPosition);
            yield return new WaitUntil(() => HasArrived());
            
            done?.Invoke();
        }
        
        public void MoveToTable(Vector3 seatPosition, Seat seat)
        {
            UpdateState(State.MoveToSit);
            CompletedOrder();

            StartCoroutine(SeatCor(seatPosition, seat));
        }

        private IEnumerator SeatCor(Vector3 seatPosition, Seat seat)
        {
            _agent.SetDestination(seatPosition);

            yield return new WaitUntil(()=> HasArrived());

            float elasepd = 0;
            float duration = 1f;
            Vector3 tablePosition = seat.GetTablePosition();
            Vector3 dir = (tablePosition - transform.position).normalized;
            Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
            while (elasepd < duration)
            {
                elasepd += Time.deltaTime;
                Quaternion prev = transform.rotation;
                transform.rotation = Quaternion.Lerp(prev, rot, elasepd / duration);
                yield return null;
            }
            
            _animator.SetTrigger(_sitHash);
            
            while (_wobblingStack.StackCount > 0)
            {
                float d = 0.15f;
                GameObject go = _wobblingStack.Pop();
                seat.StackFood(go, d);
                yield return new WaitForSeconds(d);
            }
            
            seat.CompleteStackFood(OnEatting, OnLeave);
        }

        private void OnLeave()
        {
            UpdateState(State.Leave);
            StartCoroutine(LeaveCor());
        }

        private void OnEatting(int level)
        {
            //현재 앉은 의자의 레벨에 따라서 애니메이션 스피드를 조금 더 빠르게 설정
            _animator.speed = 1 + (0.4f * (level - 1));
            UpdateState(State.Eat);
            
            _animator.SetTrigger(_eatHash);
        }

        private IEnumerator LeaveCor()
        {
            _animator.speed = 1;
            _animator.SetTrigger(_leaveHash);

            yield return new WaitForSeconds(0.5f);
            _agent.SetDestination(_despawnPosition);
        }

        private void CompletedOrder()
        {
            IsReadyOrder = false;  
        }

        private void MoveTo(Vector3 destinationPosition)
        {
            if (_moveToCor != null)
            {
                StopCoroutine(_moveToCor);
            }

            IsReadyOrder = false;
            _moveToCor = StartCoroutine(MoveToCor(destinationPosition, () =>
            {
                UpdateState(State.Idle);
                IsReadyOrder = true;
            }));
        }

        public void Spawn(Vector3 spawnPosition, Vector3 queuePosition, int maxFoodCapacity, Vector3 despawnPosition)
        {
            _agent.speed = _data.MoveSpeed;
            _orderCount = maxFoodCapacity;
            transform.position = spawnPosition;
            UpdateState(State.MoveToCounterTable);
            gameObject.SetActive(true);
            _despawnPosition = despawnPosition;
            MoveTo(queuePosition);
        }
    }
}