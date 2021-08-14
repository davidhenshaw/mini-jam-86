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

    public struct GameInfo
    {
        public readonly Scoreboard scoreboard;
        public readonly bool ballIsLive;
        public readonly Team server;
        public readonly Team receiver;
        public readonly Team winner;

        public GameInfo(Scoreboard s, bool ballLive, Team w, Team svr, Team rcv)
        {
            scoreboard = s;
            ballIsLive = ballLive;
            server = svr;
            receiver = rcv;
            winner = w;
        }
    }

    public class GameState : MonoBehaviour
    {
        private static GameState instance;
        public static GameState Instance => instance;

        public static event Action<GameState> Created;
        public static event Action<GameState> Destroyed;

        [SerializeField] InputAction accept;
        [Space]
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
        [Space]
        public float transitionTime = 2;
        [Space]
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
        Team _winner;

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
            accept.performed += NextRound;

            _scoreboard = new Scoreboard(new Team[]{ _team1, _team2 });
            _server = _team1;
            _receiver = _team2;
            Bounces = 0;
            MaxBouncesReached?.Invoke(false);
        }

        private void OnDisable()
        {
            accept.Disable();    
        }

        private void OnEnable()
        {
            accept.Enable();
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
            if(!_ballIsLive)
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

        void OnSceneLoad(Scene scene, LoadSceneMode mode)
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
            _winner = null;
            Bounces = 0;
            MaxBouncesReached?.Invoke(false);
        }

        void OnRacketHit(Racket racket)
        {
            ResetBounces();
            Server = racket.Player.Team;
            Debug.Log("Racket Hit: " + _server.name);
        }

        void ResetBounces()
        {
            Bounces = 0;
            MaxBouncesReached?.Invoke(false);
        }

        void SetWinner(Team team)
        {
            _winner = team;
            _scoreboard.UpdatePoints(team, 1);
            ScoreChanged?.Invoke(GetGameInfo());
            _ballIsLive = false;

            StartCoroutine(RoundEnd_co(transitionTime));
        }

        void Judge(Vector3 position)
        {
            // If the server hits out of bounds, the receiver wins
            if (!IsInBounds(position) && Bounces == 0)
            {
                SetWinner(_receiver);
                return;
            }
            else if( !IsInBounds(position) && Bounces > 0)
            {// If any subsequent bounce is out of bounds, the server wins
                SetWinner(_server);
                return;
            }

            // If the ball bounces too many times, the server wins
            if (Bounces >= _maxBounces)
            {
                MaxBouncesReached?.Invoke(true);
                SetWinner(_server);
            }

        }

        void OnBounce(Ball ball)
        {
            Debug.Log(IsInBounds(ball.transform.position) ? "In Bounds" : "Out of Bounds");

            if(_ballIsLive)
                Judge(ball.transform.position);

            Bounces++;
        }

        IEnumerator RoundEnd_co(float waitTime)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            NextRoundReady?.Invoke();
        }

    }
}