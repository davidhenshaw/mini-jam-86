using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace metakazz{
    public class Ball : MonoBehaviour
    {
        Transform _sprite;
        Transform _shadow;

        [SerializeField] Vector3 velocity = Vector3.zero;
        public float GRAVITY = 5;
        public float bounceCoef = 0.89f;
        public float frictionCoef = 0.12f;

        private void Awake()
        {
            var transforms = GetComponentsInChildren<Transform>();
            _sprite = transforms[1];
            _shadow = transforms[2];
        }

        // Start is called before the first frame update
        void Start()
        {
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
            velocity = Vector3.Reflect(velocity, normal) * bounceCoef;
        }

        void ApplyShadow()
        {
            float yOffset = transform.position.z;
            _sprite.localPosition = new Vector2(0, yOffset);
        }
    }
}