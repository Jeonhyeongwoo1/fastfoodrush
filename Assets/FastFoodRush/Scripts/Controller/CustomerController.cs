using System.Collections;
using FastFoodRush.Controller;
using FastFoodRush.Interactable;
using FastFoodRush.Manager;
using UnityEngine;

namespace FastFoodRush.Object
{
    public class CustomerController : BaseController
    {
        public enum State
        {
            Idle,
            MoveToCounterTable,
            MoveToSit,
            Eat,
            Leave
        }
        
        public bool IsReadyOrder { get; private set; }
        public int RemainOrderCount => _orderCount - _wobblingStack.StackCount;

        [SerializeField] private State _state;
        [SerializeField] private CustomerAIConfigData _data;
        
        private int _orderCount;
        private Vector3 _queuePointPosition;
        private Vector3 _despawnPosition;
        
        private int _sitHash;
        private int _eatHash;
        private int _leaveHash;

        private void Start()
        {
            _isMovingHash = Animator.StringToHash("IsMoving");
            _sitHash = Animator.StringToHash("Sit");
            _eatHash = Animator.StringToHash("Eat");
            _leaveHash = Animator.StringToHash("Leave");
        }

        private void Update()
        {
            bool isMoving = !HasArrived();
            switch (_state)
            {
                case State.Idle:
                    break;
                case State.MoveToCounterTable:
                    IsReadyOrder = !isMoving;
                    if (!isMoving)
                    {
                        UpdateState(State.Idle);
                    }
                    
                    break;
                case State.MoveToSit:
                    if (!isMoving)
                    {
                    
                    }
                    break;
                case State.Eat:
                    break;
            }
            
            _animator.SetBool(_isMovingHash, isMoving);
        }
        
        private void UpdateState(State state)
        {
            _state = state;
        }

        public void ReceiveOrderInfo(GameObject orderObj)
        {
            if (RemainOrderCount == 0)
            {
                return;
            }
            
            _wobblingStack.Stack(orderObj, StackType.Food);
        }

        public void UpdateQueuePosition(Vector3 position)
        {
            UpdateState(State.MoveToCounterTable);
            _queuePointPosition = position;
            _agent.SetDestination(_queuePointPosition);
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

        private void OnEatting()
        {
            UpdateState(State.Eat);
            _animator.SetTrigger(_eatHash);
        }

        private IEnumerator LeaveCor()
        {
            _animator.SetTrigger(_leaveHash);

            yield return new WaitForSeconds(0.5f);
            _agent.SetDestination(_despawnPosition);
        }

        private void CompletedOrder()
        {
            IsReadyOrder = false;  
        }

        public void Spawn(Vector3 spawnPosition, Vector3 queuePosition, int maxFoodCapacity, Vector3 despawnPosition)
        {
            _agent.speed = _data.MoveSpeed;
            _queuePointPosition = queuePosition;
            _orderCount = maxFoodCapacity;
            transform.position = spawnPosition;
            UpdateState(State.MoveToCounterTable);
            gameObject.SetActive(true);
            _agent.SetDestination(_queuePointPosition);
            _despawnPosition = despawnPosition;
        }
    }
}