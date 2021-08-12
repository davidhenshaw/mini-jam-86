using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace metakazz{

    public class Scoreboard
    {
        Dictionary<PlayerController, int> _scoreboard = new Dictionary<PlayerController, int>();

        public Scoreboard(ICollection<PlayerController> players)
        {

            foreach(PlayerController pc in players)
            {
                _scoreboard.Add(pc, 0);
            }
        }

        public void UpdatePoints(PlayerController p, int value)
        {
            _scoreboard[p] += value;
        }

        public int[] GetScores()
        {
            int[] arr = new int[_scoreboard.Count];
            _scoreboard.Values.CopyTo(arr, 0);

            return arr;
        }
    }


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
        public int _maxBounces = 1;
        public int MaxBounces { get; }
        bool _ballIsLive = true;
        public PlayerController _player1;
        int _player1Score;
        public PlayerController _player2;
        int _player2Score;
        Scoreboard _scoreboard;

        PlayerController Server
        {
            get { return _server; }
            set
            {
                _receiver = _server;
                _server = value;
            }
        }

        PlayerController _server;
        PlayerController _receiver;

        public event Action<int> BouncesChanged;
        public event Action<bool> MaxBouncesReached;
        public event Action<Scoreboard> ScoreChanged;

        private void Awake()
        {
            SubscribeEvents();
            Created?.Invoke(this);
        }

        private void Start()
        {
            _scoreboard = new Scoreboard(new PlayerController[]{ _player1, _player2 });
            _server = _player1;
            _receiver = _player2;

            Bounces = 0;
            MaxBouncesReached?.Invoke(false);
        }

        void SubscribeEvents()
        {
            Ball.Bounced += OnBounce;
            Racket.BallHit += OnRacketHit;
        }

        private void OnRacketHit(Racket racket)
        {
            ResetBounces();
            Server = racket.Player;
            Debug.Log("Racket Hit: " + _server.name);
        }

        public bool IsInBounds(Vector2 point)
        {
            var collider = Physics2D.OverlapPoint(point, LayerMask.GetMask("Court"));

            if (!collider)
                return false;

            CourtBound bounds = collider.GetComponent<CourtBound>();

            return (bounds.Player != Server);
        }

        void ResetBounces()
        {
            Bounces = 0;
            MaxBouncesReached?.Invoke(false);
        }

        void Judge(Vector3 position)
        {
            // If the server hits out of bounds, the receiver wins
            if (!IsInBounds(position) && Bounces == 0)
            {
                _scoreboard.UpdatePoints(_receiver, 1);
                ScoreChanged?.Invoke(_scoreboard);
                _ballIsLive = false;

                return;
            }
            else if( !IsInBounds(position) && Bounces > 0)
            {// If any subsequent bounce is out of bounds, the server wins
                _scoreboard.UpdatePoints(Server, 1);
                ScoreChanged?.Invoke(_scoreboard);
                _ballIsLive = false;

                return;
            }

            // If the ball bounces too many times, the server wins
            if (Bounces >= _maxBounces)
            {
                MaxBouncesReached?.Invoke(true);

                _scoreboard.UpdatePoints(Server, 1);
                ScoreChanged?.Invoke(_scoreboard);
                _ballIsLive = false;
            }

        }

        void OnBounce(Ball ball)
        {
            Debug.Log(IsInBounds(ball.transform.position) ? "In Bounds" : "Out of Bounds");

            if(_ballIsLive)
                Judge(ball.transform.position);

            Bounces++;
        }
    }
}