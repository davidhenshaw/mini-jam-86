using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace metakazz{
    public class CourtBound : MonoBehaviour
    {
        [SerializeField] PlayerController _player;
        public PlayerController Player => _player;
    }
}