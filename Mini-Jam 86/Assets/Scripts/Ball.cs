using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace metakazz{
    public class Ball : MonoBehaviour
    {
        Transform _sprite;
        Transform _shadow;
        BEUCollider _beuCollider;

        [SerializeField] Vector3 _velocity = Vector3.zero;
        public float GRAVITY = 5;
        public float bounceCoef = 0.89f;
        public float frictionCoef = 0.12f;

        private void Awake()
        {
            var transforms = GetComponentsInChildren<Transform>();
            _sprite = transforms[1];
            _shadow = transforms[2];

            _beuCollider = GetComponent<BEUCollider>();
        }

        void ResolveCollision(Collision2D collision)
        {
            if (collision.collider.GetComponent<Racket>())
                return;

            DefaultCollision(collision);
        }

        void DefaultCollision(Collision2D collision)
        {
            Vector3 avgNormal = Vector3.zero;
            foreach (ContactPoint2D contact in collision.contacts)
            {
                avgNormal += (Vector3)contact.normal;
            }
            avgNormal.Normalize();

            Bounce(avgNormal);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var otherBeuCol = collision.collider.GetComponentInParent<BEUCollider>();
            var depthCol = _beuCollider.DepthCollider;

            if(_beuCollider.IsTouchingHeight(otherBeuCol))
            {
                ResolveCollision(collision);
            }
        }

        // Update is called once per frame
        void Update()
        {
            ApplyShadow();
            VerticalMovement();
            HorizontalMovement();
        }

        void VerticalMovement()
        {
            var dz = (_velocity.z) * Time.deltaTime;
            transform.Translate(0, 0, dz);

            _velocity -= Vector3.forward * GRAVITY * Time.deltaTime;

            if(transform.position.z < 0 )
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                Bounce(Vector3.back);
            }
        }

        void HorizontalMovement()
        {
            var dPos = _velocity * Time.deltaTime;
            transform.Translate(dPos.x, dPos.y, 0);

            //float fricX = (velocity * frictionCoef).x;
            //float fricY = (velocity * frictionCoef).y;
            //velocity -= new Vector3(fricX, fricY, 0);

        }

        void Bounce(Vector3 normal)
        {
            _velocity = Vector3.Reflect(_velocity, normal.normalized) * bounceCoef;
        }

        public void Launch(Vector3 impulse)
        {
            _velocity += impulse;
        }

        void ApplyShadow()
        {
            float yOffset = transform.position.z;
            _sprite.localPosition = new Vector2(0, yOffset);
        }
    }
}