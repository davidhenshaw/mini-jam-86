using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace metakazz{
    public class PlayerController : MonoBehaviour
    {
        Rigidbody2D _rigidbody;
        Racket _racket;
        Vector2 _direction;
        float moveSpeed = 2;
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
                Debug.Log("Swing");
                StartCoroutine(ActivateRacket(1));
            }
        }

        IEnumerator ActivateRacket(float duration)
        {
            _racket.SetCollidersEnabled(true);
            yield return new WaitForSeconds(duration);
            _racket.SetCollidersEnabled(false);
        }
    }
}