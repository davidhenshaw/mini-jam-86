using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace metakazz{

    public class Scoreboard
    {
        Dictionary<Team, int> _scoreboard = new Dictionary<Team, int>();

        public Scoreboard(ICollection<Team> players)
        {

            foreach(Team pc in players)
            {
                _scoreboard.Add(pc, 0);
            }
        }

        public void UpdatePoints(Team p, int value)
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
        private static GameState instance;
        public static GameState Instance => instance;

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

        [SerializeField] Team _team1;
        public Team Team1 => _team1;
        [SerializeField] Team _team2;
        public Team Team2 => _team2;

        Scoreboard _scoreboard;

        Team Server
        {
            get { return _server; }
            set
            {
                _receiver = _server;
                _server = value;
            }
        }

        Team _server;
        Team _receiver;

        public event Action<int> BouncesChanged;
        public event Action<bool> MaxBouncesReached;
        public event Action<GameInfo> ScoreChanged;
        public event Action LoadNextRound;
        public event Action NextRoundReady;

        private void Awake()
        {
            if(!instance)
            {
                instance = this;
                SubscribeEvents();
                Created?.Invoke(this);
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            _scoreboard = new Scoreboard(new Team[]{ _team1, _team2 });
            _server = _team1;
            _receiver = _team2;
            Bounces = 0;
            MaxBouncesReached?.Invoke(false);
        }

        public bool IsInBounds(Vector2 point)
        {
            var collider = Physics2D.OverlapPoint(point, LayerMask.GetMask("Court"));

            if (!collider)
                return false;

            CourtBound bounds = collider.GetComponent<CourtBound>();

            return (bounds.Team != Server);
        }

        public GameInfo GetGameInfo()
        {
            var info = new GameInfo(_scoreboard, _ballIsLive, _winner, _server, _receiver);
            return info;
        }

        public void NextRound(InputAction.CallbackContext context )
        {
            if(context.started && !_ballIsLive)
            {
                LoadNextRound?.Invoke();
            }
        }

        void SubscribeEvents()
        {
            SceneManager.sceneLoaded += OnSceneLoad;
            Ball.Bounced += OnBounce;
            Racket.BallHit += OnRacketHit;
        }

        private void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            if(scene.name == SceneLoader.Instance.MainGame.name)
            {
                ResetRound();
            }
        }

        void ResetRound()
        {
            _ballIsLive = true;
            _server = _team1;
            _receiver = _team2;
            Bounces = 0;
            MaxBouncesReached?.Invoke(false);
        }

        private void OnRacketHit(Racket racket)
        {
            ResetBounces();
            Server = racket.Player.Team;
            Debug.Log("Racket Hit: " + _server.name);
        }

        public bool IsInBounds(Vector2 point)
        {
            var collider = Physics2D.OverlapPoint(point, LayerMask.GetMask("Court"));

            if (!collider)
                return false;

            CourtBound bounds = collider.GetComponent<CourtBound>();

            return (bounds.Team != Server);
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