using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace metakazz{
    public class ShadowCollider : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            SendMessageUpwards("OnTriggerEnter2D", collision);
        }
    }
}