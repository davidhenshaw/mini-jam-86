using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace metakazz{
    public class BEUCollider : MonoBehaviour
    {
        public Collider2D _depthCollider;
        public Collider2D DepthCollider => _depthCollider;

        public Collider2D _heightCollider;
        public Collider2D HeightCollider => _heightCollider;

        public event Action<Collider2D> collided;

        private void Awake()
        {
            if(!_heightCollider)
            {
                var colliders = GetComponentsInChildren<Collider2D>();
                _heightCollider = colliders[0];
            }

            if(!_depthCollider)
            {
                var colliders = GetComponentsInChildren<Collider2D>();
                _depthCollider = colliders[1];
            }


            //Make the depth hitbox a trigger
            _depthCollider.isTrigger = true;

        }

        private void Update()
        {
            CheckCollision();
        }

        private void CheckCollision()
        {
            if (_depthCollider.IsTouchingLayers(LayerMask.GetMask("Depth Hitbox")))
            {
                Collider2D[] result = new Collider2D[1];
                ContactFilter2D filter = new ContactFilter2D();
                filter.SetLayerMask(LayerMask.GetMask("Height Hitbox"));
                _heightCollider.OverlapCollider(filter, result);

                if(result[0] != null)
                {
                    collided?.Invoke(result[0]);
                }
            }
        }

    }
}