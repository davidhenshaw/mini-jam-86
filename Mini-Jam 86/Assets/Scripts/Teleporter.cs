using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace metakazz{
    public class Teleporter : MonoBehaviour
    {
        [SerializeField] Transform toTeleport;
        [SerializeField] Camera mainCamera;

        private void Awake()
        {
            if(!mainCamera)
            {
                mainCamera = Camera.main;
            }
        }

        // Update is called once per frame
        void Update()
        {
            var viewPos = mainCamera.WorldToViewportPoint(toTeleport.position);

            if(viewPos.y > 1)
            {
                TeleportObject(new Vector2(viewPos.x, 0));
                return;
            }

            if(viewPos.y < 0)
            {
                TeleportObject(new Vector2(viewPos.x, 1));
                return;
            }

            if (viewPos.x > 1)
            {
                TeleportObject(new Vector2(0, viewPos.y));
                return;
            }

            if (viewPos.x < 0)
            {
                TeleportObject(new Vector2(1, viewPos.y));
                return;
            }
        }

        void TeleportObject(Vector2 viewportPos)
        {
            var worldPos = mainCamera.ViewportToWorldPoint(viewportPos);
            toTeleport.position = new Vector3(worldPos.x, worldPos.y, toTeleport.position.z);
        }
    }
}