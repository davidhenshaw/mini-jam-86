using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace metakazz{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        Rigidbody2D _rigidbody;
        Racket _racket;
        Vector2 _direction;
        Animator _animator;
        [SerializeField] Team _team;
        public Team Team => _team;

        [SerializeField] float moveSpeed = 2;
        [SerializeField] float moveAccel = 10;
        [SerializeField] float moveDecel = 7;
        float minVelocity = 0.01f;
        [SerializeField] float _swingDuration = 0.5f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _racket = GetComponentInChildren<Racket>();
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _racket.SetCollidersEnabled(false);
        }

        private void OnDisable()
        {
            _team.RemovePlayer(this);
        }

        private void OnEnable()
        {
            _team.AddPlayer(this);
        }

        private void Update()
        {
            ApplyMotion();
        }

        void ApplyMotion()
        {
            Vector2 moveVector = Vector2.zero;

            if(_direction.magnitude > 0)
            {
                moveVector += _direction * moveAccel * Time.deltaTime;
            }

            if(_rigidbody.velocity.magnitude > minVelocity)
                moveVector += -1 * _rigidbody.velocity.normalized * moveDecel * Time.deltaTime;

            _rigidbody.velocity += moveVector;
            _rigidbody.velocity = Vector2.ClampMagnitude(_rigidbody.velocity, moveSpeed);
            _animator.SetFloat("moveDir", _rigidbody.velocity.y);
        }

        public void OnMovement(InputAction.CallbackContext value)
        {
            Move(value.ReadValue<Vector2>());

            if(value.started)
            {
                _animator.SetBool("isMoving", true);
            }
            else if(value.canceled)
            {
                _animator.SetBool("isMoving", false);
            }
        }

        public void Move(Vector2 direction)
        {
            _direction = direction;
        }

        public void OnSwing(InputAction.CallbackContext value)
        {
            if(value.started)
            {
                Swing();
            }
        }

        public void Swing()
        {
            StartCoroutine(ActivateRacket(_swingDuration));
            _animator.SetTrigger("fwdSwing");
        }

        public void OnAimAxis(InputAction.CallbackContext value)
        {

        }

        public void OnAimPosition(InputAction.CallbackContext value)
        {
            var rawPos = value.ReadValue<Vector2>();
        }

        IEnumerator ActivateRacket(float duration)
        {
            _racket.SetCollidersEnabled(true);
            yield return new WaitForSeconds(duration);
            _racket.SetCollidersEnabled(false);
        }
    }
}