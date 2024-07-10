using System;
using System.Collections;
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
        [SerializeField] private GameObjectDataEventChannelSO _partClicked;
        

        [SerializeField] private PartAnimState _rootState;
        [SerializeField] private Material _outlineMat;

        private bool _assemblyState = false;
        
        private Animator _animator;

        private Outline _prevOutline;

        private void OnEnable() {
            _partClicked.OnEventRaised += OnPartSelect;
        }

        private void Start() {
            _animator = GetComponent<Animator>();
            _rootState.active = true;

            AssignParent(_rootState);
            AssignHint(_assemblyState);
        }
        
        public void ToggleReverse() {
            _assemblyState = !_assemblyState;
            AssignHint(_assemblyState);
        }

        public void AssignObj(WeaponPartAnim part) {
            PartAnimState state = FindNode(_rootState, part.AnimName);
            state.obj = part.gameObject;
        }

        private void AssignParent(PartAnimState node) {
            foreach (var child in node.subStates) {
                child.parent = node;
                if (child.subStates != null) AssignParent(child);
            }
        }

        private void AssignHint(bool reverse) {
            PartAnimState currentHint;
            if (reverse) {
                PartAnimState deepest = FindDeepest(_rootState, n => !n.active);
                currentHint = FindParent(deepest, n => n.active);
            }
            else {
                currentHint = DFS(_rootState, n => !n.active);
            }
            if (currentHint == null) return;
            Debug.Log(currentHint.name);

            Outline outline = currentHint.obj.GetComponent<Outline>();
            outline.enabled = true;
        }

        private void OnPartSelect(GameObject obj) {
            if (obj.layer != LayerMask.NameToLayer("Weapon") || !obj.TryGetComponent<WeaponPartAnim>(out WeaponPartAnim part)) return;
            
            PartAnimState state = FindNode(_rootState, part.AnimName);
            
            if (state == null) return;
            
            if (!_assemblyState && state.parent.active && !state.active) {
                _animator.CrossFade(state.name, 0);
                state.active = true;
                state.obj.GetComponent<Outline>().enabled = false;
                AssignHint(_assemblyState);
            }

            if (_assemblyState && state.active && (state.subStates == null || state.subStates.All(state => !state.active))) {
                _animator.CrossFade(state.name + "Reverse", 0);
                state.active = false;
                state.obj.GetComponent<Outline>().enabled = false;
                AssignHint(_assemblyState);
            }
        }


        private PartAnimState FindNode(PartAnimState rootNode, string searchString) {
            return DFS(rootNode, n => n.name == searchString);
        }

        private PartAnimState DFS(PartAnimState node, Func<PartAnimState, bool> pred) {
            var stack = new Stack<PartAnimState>(new[] { node });
            while (stack.Any()) {
                var n = stack.Pop();
                if (pred(n)) return n;
                foreach (var child in n.subStates) stack.Push(child);
            }

            return null;
        }
        
        private List<PartAnimState> DFSMany(PartAnimState node, Func<PartAnimState, bool> pred) {
           var stack = new Stack<PartAnimState>(new[] { node });
           var matchList = new List<PartAnimState>();
           while (stack.Any()) {
               var n = stack.Pop();
               if (pred(n)) {
                   matchList.Add(n);
                   continue;
               }
               foreach (var child in n.subStates) stack.Push(child);
           }

           return matchList;
       }

        private PartAnimState FindDeepest(PartAnimState node, Func<PartAnimState, bool> pred) {
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

        private PartAnimState FindParent(PartAnimState node, Func<PartAnimState, bool> pred) {
            PartAnimState temp = node;
            while (temp != null && temp.name != "Root") {
                if (pred(temp)) return temp;
                temp = temp.parent;
            }

            return null;
        }
        
    }
}