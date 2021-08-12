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
        [SerializeField] Team _team;
        public Team Team => _team;

        [SerializeField] float moveSpeed = 2;
        [SerializeField] float moveAccel = 10;
        [SerializeField] float moveDecel = 7;
        float minVelocity = 0.01f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _racket = GetComponentInChildren<Racket>();
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
        }

        public void OnMovement(InputAction.CallbackContext value)
        {
            _direction = value.ReadValue<Vector2>();
        }

        public void OnSwing(InputAction.CallbackContext value)
        {
            if(value.started)
            {
                StartCoroutine(ActivateRacket(1));
            }
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