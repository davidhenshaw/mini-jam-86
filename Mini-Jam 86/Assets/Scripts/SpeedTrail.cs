using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace metakazz{
    [RequireComponent(typeof(TrailRenderer))]
    public class SpeedTrail : MonoBehaviour
    {
        TrailRenderer _trail;
        Ball _ball;

        public float maxSpeed;
        public float minSpeed;

        public float maxTrailTime = 0.5f;
        public float minTrailIntensity = 0.2f;

        private void Awake()
        {
            _trail = GetComponent<TrailRenderer>();
            _ball = GetComponentInParent<Ball>();
        }

        // Update is called once per frame
        void Update()
        {
            var xyVelocity = new Vector3 (_ball.Velocity.x, _ball.Velocity.y, 0);
            var intensity = Mathf.InverseLerp(minSpeed, maxSpeed,  xyVelocity.magnitude);

            if (intensity <= 0)
            {
                _trail.enabled = false;
                return;
            }
            else
            {
                _trail.enabled = true;
            }

            var trailTime = Mathf.Lerp(minTrailIntensity, maxTrailTime, intensity);
            _trail.time = trailTime;
        }
    }
}