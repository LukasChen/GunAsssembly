using UnityEditor;
using UnityEngine;

namespace GunAssembly{
    
    [CustomEditor(typeof(WeaponController))]
    public class WeaponControllerEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            

            if (GUILayout.Button("Assign Array")) {
                WeaponController controller = (WeaponController)target;
                WeaponPartAnim[] parts = FindObjectsOfType<WeaponPartAnim>();

                foreach (var part in parts) {
                    controller.AssignObj(part);
                }
            }
        }
    }
}