using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DarckNet
{
    public class NetworkPrefabs : MonoBehaviour
    {
        [SerializeField]
        public List<GameObject> Prefabs = new List<GameObject>();

        void Start()
        {
            Network.PregabsList = this;

            for (int i = 0; i < Prefabs.Count; i++)
            {
                Prefabs[i].GetComponent<NetworkObj>().PrefabID = i;
            }
        }
    }
}