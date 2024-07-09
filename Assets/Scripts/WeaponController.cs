using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace GunAssembly {
    [Serializable]
    public class PartAnimState {
        public string name;
        public bool active;
        public GameObject obj;
        public PartAnimState[] subStates;
        public PartAnimState parent;
    }
   
    public class WeaponController : MonoBehaviour {
        [SerializeField] private string[] _animStates;
        [SerializeField] private GameObjectDataEventChannelSO _partClicked;

        [SerializeField] private PartAnimState _rootState;

        private bool _assemblyState;
        
        private Animator _animator;

        private int _currentIndex = 0;

        private void OnEnable() {
            _partClicked.OnEventRaised += OnPartSelect;
        }

        private void Start() {
            _animator = GetComponent<Animator>();
            _rootState.active = true;

            AssignParent(_rootState);
        }

        private void AssignParent(PartAnimState node) {
            foreach (var child in node.subStates) {
                child.parent = node;
                if (child.subStates != null) AssignParent(child);
            }
        }

        private void OnPartSelect(GameObject obj) {
            if (obj.layer != LayerMask.NameToLayer("Weapon") || !obj.TryGetComponent<WeaponPartAnim>(out WeaponPartAnim part)) return;
            
            PartAnimState state = FindNode(_rootState, part.AnimName);
            
            if (state == null) return;
            
            if (!_assemblyState && state.parent.active && !state.active) {
                _animator.CrossFade(state.name, 0);
                state.active = true;    
            }

            if (_assemblyState && state.active && (state.subStates == null || state.subStates.All(state => !state.active))) {
                _animator.CrossFade(state.name + "Reverse", 0);
                state.active = false;
            }
        }

        public void ToggleReverse() {
            _assemblyState = !_assemblyState;
        }

        public void AssignObj(WeaponPartAnim part) {
            PartAnimState state = FindNode(_rootState, part.AnimName);
            state.obj = part.gameObject;
        }
        
        
        public void SwitchAnim() {
            _animator.CrossFade(_animStates[++_currentIndex % _animStates.Length], 0);
        }

        private PartAnimState FindNode(PartAnimState rootNode, string searchString) {
            var stack = new Stack<PartAnimState>(new[] { rootNode });
            while (stack.Any()) {
                var n = stack.Pop();
                if (n.name == searchString) return n;
                foreach (var child in n.subStates) stack.Push(child);
            }

            return null;
        }
    }
}