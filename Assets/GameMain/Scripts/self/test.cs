using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace StarForce
{
    public class test : MonoBehaviour
    {
        
        void Start()
        {
            //DebuggerComponent debug = UnityGameFramework.Runtime.GameEntry.GetComponent<DebuggerComponent>();
            GameEntry.Debugger.enabled=false;
            GameEntry.HPBar.enabled=false;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

