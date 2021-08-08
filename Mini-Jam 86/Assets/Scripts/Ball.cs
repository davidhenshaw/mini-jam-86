using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace metakazz{
    public class Ball : MonoBehaviour
    {
        Transform _sprite;
        Transform _shadow;
        BEUCollider _beuCollider;

        [SerializeField] Vector3 velocity = Vector3.zero;
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

        // Start is called before the first frame update
        void Start()
        {
            _beuCollider.collided += onCollision;
        }

        private void onCollision(Collider2D other)
        {
            //If you collide AND you're moving toward the collider that you hit,
            // then you bounce 

            Vector3 normal = GetCollisionNormal(other);
            var dot = Vector3.Dot(normal, velocity);

            if(dot < 0) //The ball's velocity is pointed toward the other collider
            {
                Bounce(normal);
                //Bounce(Vector3.up);
            }
        }


        Vector3 GetCollisionNormal(Collider2D other)
        {
            ContactPoint2D[] points = new ContactPoint2D[3];
            var depthCol = _beuCollider.DepthCollider;

            Vector3 average = Vector3.zero;
            foreach (ContactPoint2D contact in points)
            {
                average += depthCol.transform.position - (Vector3)contact.point;
            }

            return average.normalized;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var otherBeuCol = collision.collider.GetComponentInParent<BEUCollider>();
            var depthCol = _beuCollider.DepthCollider;

            if(_beuCollider.IsTouchingHeight(otherBeuCol))
            {
                Vector3 average = Vector3.zero;
                foreach (ContactPoint2D contact in collision.contacts)
                {
                    average += (Vector3)contact.normal;
                }
                average.Normalize();

                Bounce(average);
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
            var dz = (velocity.z) * Time.deltaTime;
            transform.Translate(0, 0, dz);

            velocity -= Vector3.forward * GRAVITY * Time.deltaTime;

            if(transform.position.z < 0 )
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                Bounce(Vector3.back);
            }
        }

        void HorizontalMovement()
        {
            var dPos = velocity * Time.deltaTime;
            transform.Translate(dPos.x, dPos.y, 0);

            //float fricX = (velocity * frictionCoef).x;
            //float fricY = (velocity * frictionCoef).y;
            //velocity -= new Vector3(fricX, fricY, 0);

        }

        void Bounce(Vector3 normal)
        {
            velocity = Vector3.Reflect(velocity, normal.normalized) * bounceCoef;
        }

        void ApplyShadow()
        {
            float yOffset = transform.position.z;
            _sprite.localPosition = new Vector2(0, yOffset);
        }
    }
}