using Cinemachine;
using UnityEditor;
using UnityEngine;

namespace GunAssembly {
    [CustomEditor(typeof(CameraSwitcher))]
    public class CameraSwticherEditor : Editor {
        public override void OnInspectorGUI() {
            
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Assign Cameras")) {
                CameraSwitcher controller = (CameraSwitcher)target;
                CinemachineVirtualCamera[] cams = FindObjectsOfType<CinemachineVirtualCamera>();

                controller.cameras = cams;
            }
        }
    }
}