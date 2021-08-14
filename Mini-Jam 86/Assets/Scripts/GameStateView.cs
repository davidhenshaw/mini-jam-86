using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace metakazz{
    public class GameStateView : MonoBehaviour
    {
        public TMP_Text _bounceView;
        public Color _bounceViewColor;
        public Color _maxBouncesColor;
        [Space]
        public TMP_Text _player1Label;
        public TMP_Text _player1Score;
        [Space]
        public TMP_Text _player2Label;
        public TMP_Text _player2Score;
        [Space]
        public TMP_Text _alertText;

        private void Awake()
        {
            GameState.Created += Bind;   
        }

        public void Bind(GameState gameState)
        {
            gameState.BouncesChanged += OnBounce;
            gameState.MaxBouncesReached += OnMaxBounces;

            gameState.ScoreChanged += OnScore;
            gameState.ScoreChanged += OnRoundEnded;
            gameState.NextRoundReady += OnLoadNextRound;

            _player1Label.text = gameState.Team1.TeamName;
            _player2Label.text = gameState.Team2.TeamName;
        }

        private void OnLoadNextRound()
        {
            _alertText.enabled = false;
        }

        private void OnScore(GameInfo info)
        {
            var scores = info.scoreboard.GetScores();

            _player1Score.text = scores[0].ToString();
            _player2Score.text = scores[1].ToString();
        }

        void OnRoundEnded(GameInfo info)
        {
            _alertText.enabled = true;
            _alertText.text = $"{info.winner.TeamName} Wins!";
        }

        private void OnMaxBounces(bool value)
        {
            _bounceView.color = value ? _maxBouncesColor : _bounceViewColor;
        }

        void OnBounce(int value)
        {
            _bounceView.SetText($"Bounces: {value}");
        }
    }
}