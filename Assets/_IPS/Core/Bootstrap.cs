using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IPS {
    /// <summary>
    /// Place this into the first scene
    /// </summary>
    public partial class Bootstrap : MonoBehaviour {
        [SerializeField][Tooltip("Only work with not production")] bool coreLogEnable = false;

        void Awake() {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Excutor.Instance.Preload();

#if !PRODUCTION
            IPSConfig.CheatEnable = true;
#endif

#if UNITY_EDITOR || CUSTOM_DEBUG
            IPSConfig.LogEnable = true;
            EventDispatcher.Instance.SetLogEnable(coreLogEnable);
#endif

            AwakeApi();
        }

        partial void AwakeApi();

    }
}