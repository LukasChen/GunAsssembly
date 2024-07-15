using Cinemachine;
using UnityEditor;
using UnityEngine;
using GunAssembly.Weapon;

namespace GunAssembly{
    
    [CustomEditor(typeof(WeaponController))]
    public class WeaponControllerEditor : Editor, ISerializationCallbackReceiver {
        private PartAnimState rootState;

        public void OnBeforeSerialize() {
        }

        public void OnAfterDeserialize() {
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            
            WeaponController controller = (WeaponController)target;

            if (controller.rootState == null) controller.rootState = new PartAnimState();
            
            
            // GUILayout.Label("Weapon Anim");
            // GUILayout.BeginVertical();
            // GUILayout.Space(20);
            
            
            // AssignParent(controller.rootState);
            // Display(controller.rootState, 0);
            
            // GUILayout.EndVertical();
            

            if (GUILayout.Button("Assign Array")) {
                WeaponPartAnim[] parts = FindObjectsOfType<WeaponPartAnim>();
                

                foreach (var part in parts) {
                    controller.AssignObj(part);
                }
            }
        }
        
        private void AssignParent(PartAnimState node) {
            foreach (var child in node.subStates) {
                child.parent = node;
                if (child.subStates != null) AssignParent(child);
            }
        }

        private void Display(PartAnimState node, int indent) {
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginVertical();
            node.name = EditorGUILayout.TextField("Name: ", node.name);
            node.active = EditorGUILayout.Toggle("active", node.active);
            // node.obj = EditorGUILayout.ObjectField("GameObject: ", node.obj, typeof(GameObject),
                // true, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as GameObject;
            node.cam = EditorGUILayout.ObjectField("Camera : ", node.cam, typeof(CinemachineVirtualCamera),
                true, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as CinemachineVirtualCamera;
            
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Child")) {
                node.subStates.Add(new PartAnimState());
            }

            if (GUILayout.Button("Remove Current")) {
                node.parent.subStates.Remove(node);
                return;
            }
            GUILayout.EndHorizontal();
            
            
            GUILayout.Label("SubStates: ");
            
            
            foreach (var subState in node.subStates) {
                Display(subState, indent + 1);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }
}