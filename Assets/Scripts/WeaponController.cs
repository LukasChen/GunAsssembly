using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

namespace GunAssembly {
    [Serializable]
    public class PartAnimState {
        public string name;
        public bool active;
        [SerializeReference] public GameObject obj;
        [SerializeReference] public CinemachineVirtualCamera cam;
        [SerializeReference] public List<PartAnimState> subStates = new List<PartAnimState>();
        [NonSerialized] public PartAnimState parent;
    }
    
    [Flags]
    public enum WeaponState {
        None = 0,
        Disassemble = 1,
        Assemble = 2,
        Fire = 4,
        FireAuto = 8,
    }

    [Serializable]
    public struct WeaponStateTransitions {
        public WeaponState name;
        public WeaponState nextStates;
    }
   
    public class WeaponController : MonoBehaviour {
        [SerializeField] private GameObjectDataEventChannelSO _partClicked;
        [SerializeField] private CameraDataEventChannelSO _switchCam;
        [SerializeField] public WeaponStateDataEventChannelSO OnWeaponStateChange;
        [SerializeField] public WeaponStateDataEventChannelSO ActiveStateChannel;
        public WeaponStateTransitions[] weaponStateTransitions;
        

        [SerializeReference] public PartAnimState rootState;
        

        private Dictionary<WeaponState, WeaponBaseState> _stateMap;
        
        public Animator Animator { get; private set; }

        private WeaponBaseState _currentState;

        private void OnEnable() {
            _partClicked.OnEventRaised += OnPartSelect;
        }

        private void OnDisable() {
            _partClicked.OnEventRaised -= OnPartSelect;
        }

        private void Start() {
            Animator = GetComponent<Animator>();
            _stateMap = new Dictionary<WeaponState, WeaponBaseState>() {
                { WeaponState.None, new WeaponDefaultState(this, weaponStateTransitions.Single(n => n.name == WeaponState.None).nextStates)},
                { WeaponState.Disassemble, new WeaponAssemblyState(this, weaponStateTransitions.Single(n => n.name == WeaponState.Disassemble).nextStates, false) },
                { WeaponState.Assemble, new WeaponAssemblyState(this, weaponStateTransitions.Single(n=> n.name == WeaponState.Assemble).nextStates, true) },
                { WeaponState.Fire, new WeaponFireSemiState(this, weaponStateTransitions.Single(n => n.name == WeaponState.Fire).nextStates) },
                { WeaponState.FireAuto, new WeaponFireAutoState(this, weaponStateTransitions.Single(n => n.name == WeaponState.FireAuto).nextStates) }
            };

            AssignParent(rootState);
            SwitchState(WeaponState.None);
        }

        public void SwitchState(WeaponState newState) {
            WeaponBaseState newStateClass = _stateMap[newState];
            // ActiveStateChannel.RaiseEvent(newState);
            _currentState?.ExitState();
            _currentState = newStateClass;
            _currentState.EnterState();
        }

        public void AssignObj(WeaponPartAnim part) {
            PartAnimState state = FindNode(rootState, part.AnimName);
            state.obj = part.gameObject;
        }


        private void AssignParent(PartAnimState node) {
            foreach (var child in node.subStates) {
                child.parent = node;
                if (child.subStates != null) AssignParent(child);
            }
        }
        
        private bool AnimatorIsDone() =>
            Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;

        private void OnPartSelect(GameObject obj) {
            if (!AnimatorIsDone() || obj.layer != LayerMask.NameToLayer("Weapon")) return;
            _currentState.OnPartSelected(obj);
            
        }

        public void SwitchCam(PartAnimState state) {
            PartAnimState camState = FindParent(state, n => n.cam != null);
            _switchCam.RaiseEvent(camState.cam);
        }


        public PartAnimState FindNode(PartAnimState rootNode, string searchString) {
            return DFS(rootNode, n => n.name == searchString);
        }

        public PartAnimState DFS(PartAnimState node, Func<PartAnimState, bool> pred) {
            var stack = new Stack<PartAnimState>(new[] { node });
            while (stack.Any()) {
                var n = stack.Pop();
                if (pred(n)) return n;
                foreach (var child in n.subStates) stack.Push(child);
            }

            return null;
        }

        public PartAnimState FindDeepest(PartAnimState node, Func<PartAnimState, bool> pred) {
            var queue = new Queue<PartAnimState>(new[] { node });
            PartAnimState result = null;
            while (queue.Any()) {
               PartAnimState temp  = queue.Dequeue();
               if (pred(temp)) continue;
               result = temp;
               foreach(var child in result.subStates) queue.Enqueue(child);
            }

            return result;
        }

        public PartAnimState FindParent(PartAnimState node, Func<PartAnimState, bool> pred) {
            PartAnimState temp = node;
            while (temp != null) {
                if (pred(temp)) return temp;
                temp = temp.parent;
            }

            return null;
        }
        
    }
}