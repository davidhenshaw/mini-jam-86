using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace metakazz{
    public class Teleporter : MonoBehaviour
    {
        [SerializeField] Transform toTeleport;
        [SerializeField] Camera mainCamera;
        public bool isHorizontal;

        private void Awake()
        {
            if(!mainCamera)
            {
                mainCamera = Camera.main;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            TeleportObject();
        }

        void TeleportObject()
        {
            var pos = toTeleport.position;
            toTeleport.position = isHorizontal ? new Vector3(pos.x, -pos.y, pos.z) : new Vector3(-pos.x, pos.y, pos.z);
            Debug.Log("Teleported object");
        }
    }
}