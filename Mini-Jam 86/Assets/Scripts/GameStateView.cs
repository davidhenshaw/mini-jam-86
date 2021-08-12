using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace metakazz{
    public class GameStateView : MonoBehaviour
    {
        public TMP_Text _bounceView;

        public TMP_Text _player1Score;
        public TMP_Text _player2Score;

        public Color _bounceViewColor;
        public Color _maxBouncesColor;

        private void Awake()
        {
            GameState.Created += Bind;   
        }

        public void Bind(GameState gameState)
        {
            gameState.BouncesChanged += OnBounce;
            gameState.MaxBouncesReached += OnMaxBounces;
            gameState.ScoreChanged += OnScore;
        }

        private void OnScore(Scoreboard scoreboard)
        {
            var scores = scoreboard.GetScores();

            _player1Score.text = scores[0].ToString();
            _player2Score.text = scores[1].ToString();
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