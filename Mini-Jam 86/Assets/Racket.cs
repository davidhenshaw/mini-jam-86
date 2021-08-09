using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace metakazz{
    public class Racket : MonoBehaviour
    {
        float hitPower = 3;
        BEUCollider _beuCollider;
        [SerializeField] Vector3 hitDir;

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
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            var otherBeuCol = collision.collider.GetComponentInParent<BEUCollider>();
            var depthCol = _beuCollider.DepthCollider;

            if (_beuCollider.IsTouchingHeight(otherBeuCol))
            {
                ResolveCollision(collision);
            }
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

            ball.Launch( (-1 * avgNormal) + hitDir * 3);
        }

        public void SetCollidersEnabled(bool value)
        {
            _beuCollider.enabled = value;
        }
    }
}