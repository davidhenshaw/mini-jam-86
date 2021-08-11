using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace metakazz{
    public class Racket : MonoBehaviour
    {
        BEUCollider _beuCollider;
        [SerializeField] float _hitPower = 3;
        [SerializeField] Vector3 _hitDir;
        bool _alreadyHit = false;

        public static event Action<Racket> BallHit;

        private void Awake()
        {
            _beuCollider = GetComponent<BEUCollider>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var otherBeuCol = collision.collider.GetComponentInParent<BEUCollider>();
            var depthCol = _beuCollider.DepthCollider;

            if (_beuCollider.IsTouchingHeight(otherBeuCol))
            {
                ResolveCollision(collision);
                BallHit?.Invoke(this);
                _alreadyHit = true;
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            var otherBeuCol = collision.collider.GetComponentInParent<BEUCollider>();
            var depthCol = _beuCollider.DepthCollider;

            if (_beuCollider.IsTouchingHeight(otherBeuCol) && !_alreadyHit)
            {
                ResolveCollision(collision);
                BallHit?.Invoke(this);
                _alreadyHit = true;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            _alreadyHit = false;
        }

        void ResolveCollision(Collision2D collision)
        {
            var ball = collision.collider.GetComponentInParent<Ball>();
            if (!ball)
                return;

            Vector3 avgNormal = Vector3.zero;
            foreach (ContactPoint2D contact in collision.contacts)
            {
                avgNormal += (Vector3)contact.normal;
            }
            avgNormal.Normalize();

            ball.Launch( (-1 * avgNormal) + _hitDir.normalized * _hitPower);
        }

        public void SetCollidersEnabled(bool value)
        {
            _beuCollider.enabled = value;
        }
    }
}