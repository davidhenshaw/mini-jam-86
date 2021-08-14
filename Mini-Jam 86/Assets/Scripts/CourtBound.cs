using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace metakazz{
    public class CourtBound : MonoBehaviour
    {
        [SerializeField] Team _team;
        public Team Team => _team;
    }
}