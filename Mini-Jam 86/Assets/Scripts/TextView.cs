using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace metakazz{
    public class TextView : MonoBehaviour
    {
        TMP_Text _text;
        GameStateView _view;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        public void UpdateView<T>(T value)
        {
            _text.text = value.ToString();
        }
    }
}