using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace metakazz{
    [CreateAssetMenu(menuName ="Team")]
    public class Team : ScriptableObject
    {
        List<PlayerController> _players;
        [SerializeField] string _teamName;
        public string TeamName => _teamName;

        public bool AddPlayer(PlayerController newPlayer)
        {
            if (_players.Contains(newPlayer))
                return false;

            _players.Add(newPlayer);
            return true;
        }

        public bool RemovePlayer(PlayerController toRemove)
        {
            if (!_players.Contains(toRemove))
                return false;

            _players.Remove(toRemove);
            return true;
        }
    }
}