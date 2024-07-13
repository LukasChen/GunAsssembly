using System;
using Cinemachine;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

namespace GunAssembly {
    public class CameraSwitcher : MonoBehaviour {
        public CinemachineVirtualCamera[] cameras;
        [SerializeField] private CameraDataEventChannelSO _onCamChange;

        private ICinemachineCamera _activeCam;

        private void Start() {
            Debug.Log(cameras.Length);
        }

        private void OnEnable() {
            _onCamChange.OnEventRaised += SwitchCamera;
        }

        private void OnDisable() {
            _onCamChange.OnEventRaised -= SwitchCamera;
        }
        //
        // public void Register(ICinemachineCamera camera) {
        //     if (!_cameras.Contains(camera)) {
        //         _cameras.Add(camera);
        //     }
        // }

        private void SwitchCamera(ICinemachineCamera camera) {
            camera.Priority = 10;
            _activeCam = camera;
            Debug.Log(cameras.Length);

            foreach (var c in cameras.Where(c => c != camera && c.Priority != 0)) {
                c.Priority = 0;
            }
        }
    }
}