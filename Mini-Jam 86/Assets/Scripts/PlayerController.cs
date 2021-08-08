using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace metakazz{
    public class PlayerController : MonoBehaviour
    {
        float moveSpeed = 10;

        public void Move(Vector2 dir)
        {
            var movement = dir * moveSpeed * Time.deltaTime;

            transform.Translate(movement.x, movement.y, 0);
        }
    }
}