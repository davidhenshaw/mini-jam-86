using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace metakazz{
    public class GameState : MonoBehaviour
    {
        public static event Action<GameState> Created;
        public static event Action<GameState> Destroyed;

        int _bounces = 0;
        public int Bounces
        {
            get { return _bounces; }
            set {
                _bounces = value;
                BouncesChanged?.Invoke(value);
            }
        }

        public int _maxBounces = 3;
        public int MaxBounces { get; }

        public event Action<int> BouncesChanged;
        public event Action<bool> MaxBouncesReached;

        private void Awake()
        {
            SubscribeEvents();
            Created?.Invoke(this);
        }

        private void Start()
        {
            Bounces = 0;
            MaxBouncesReached?.Invoke(false);
        }

        void SubscribeEvents()
        {
            Ball.Bounced += OnBounce;
            Racket.BallHit += OnRacketHit;
        }

        private void OnRacketHit(Racket obj)
        {
            Debug.Log("Racket Hit");
            ResetBounces();
        }

        void ResetBounces()
        {
            Bounces = 0;
            MaxBouncesReached?.Invoke(false);
        }

        void OnBounce(Ball ball)
        {
            Bounces++;

            if(Bounces >= _maxBounces)
            {
                MaxBouncesReached?.Invoke(true);
            }
        }
    }
}