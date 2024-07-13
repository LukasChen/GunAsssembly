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
    

    public enum WeaponState {
        Disassemble,
        Assemble,
        Fire,
        FireAuto, // Temp
        None
    }
    
    // TODO: Turn in to state machine
   
    public class WeaponController : MonoBehaviour {
        [SerializeField] private GameObjectDataEventChannelSO _partClicked;
        [SerializeField] private CameraDataEventChannelSO _switchCam;
        [SerializeField] private WeaponStateDataEventChannelSO _onWeaponStateChange;
        

        [SerializeReference] public PartAnimState rootState;
        [SerializeField] private Transform _shellSpawn;
        [SerializeField] private GameObject _casing;
        [SerializeField] private int _casePoolLength;
        [SerializeField] private VisualEffect[] _effects;
        private Queue<GameObject> _casePool = new Queue<GameObject>();

        private WeaponState _assemblyState = WeaponState.None;
        
        private Animator _animator;

        private bool _hasFired;
        private bool _isFiring;

        private void OnEnable() {
            _partClicked.OnEventRaised += OnPartSelect;
            _onWeaponStateChange.OnEventRaised += StateChange;
        }

        private void OnDisable() {
            _partClicked.OnEventRaised -= OnPartSelect;
            _onWeaponStateChange.OnEventRaised -= StateChange;
        }

        private void Start() {
            _animator = GetComponent<Animator>();
            Debug.Log(rootState);
            rootState.active = true;

            AssignParent(rootState);
            PrepPool();
        }

        private void StateChange(WeaponState newState) {
            if (!ValidateWeaponState(_assemblyState)) return;
            _assemblyState = newState;
            rootState.active = _assemblyState == WeaponState.Disassemble;
            if (_assemblyState != WeaponState.Fire) AssignHint(_assemblyState);
        }

        private bool ValidateWeaponState(WeaponState state) {
            bool predicate = (int)state == 1;
            PartAnimState remainingPart = DFS(rootState, n => n.active == predicate && n.name != "Root");
            bool result = remainingPart == null;

            return result || state == WeaponState.None || state == WeaponState.Fire || state == WeaponState.FireAuto;
        }

        public void AssignObj(WeaponPartAnim part) {
            PartAnimState state = FindNode(rootState, part.AnimName);
            state.obj = part.gameObject;
        }

        public void ShellEject() {
            GameObject shell = _casePool.Dequeue();
            shell.transform.position = _shellSpawn.position;
            shell.SetActive(true);
            
            Rigidbody rb = shell.GetComponent<Rigidbody>();
            rb.velocity = new Vector3(0, Random.Range(3f, 4f), Random.Range(0.5f, 1f));
            rb.angularVelocity = new Vector3(10, 10, 10);

            foreach (var effect in _effects) {
                effect.Play();
            }

            StartCoroutine(DespawnCase(shell));
            //rb.AddForce(transform.right * 20f, ForceMode.Impulse);
        }

        private IEnumerator DespawnCase(GameObject shell) {
            yield return new WaitForSeconds(0.5f);
            shell.SetActive(false);
            _casePool.Enqueue(shell);
        }

        private void PrepPool() {
            for (int i = 0; i < _casePoolLength; i++) {
                GameObject casing = Instantiate(_casing, Vector3.zero, Quaternion.identity);
                _casePool.Enqueue(casing);
                casing.SetActive(false);
            }
        }

        private void AssignParent(PartAnimState node) {
            foreach (var child in node.subStates) {
                child.parent = node;
                if (child.subStates != null) AssignParent(child);
            }
        }

        private void AssignHint(WeaponState state) {
            PartAnimState currentHint;
            if (state == WeaponState.Assemble) {
                PartAnimState deepest = FindDeepest(rootState, n => !n.active && n.name != "Root");
                currentHint = FindParent(deepest, n => n.active);
            }
            else {
                currentHint = DFS(rootState, n => !n.active);
            }
            if (currentHint == null) return;

            Outline outline = currentHint.obj.GetComponent<Outline>();
            outline.enabled = true;
        }

        private bool AnimatorIsDone() =>
            _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;

        private void OnPartSelect(GameObject obj) {
            if (!AnimatorIsDone() || obj.layer != LayerMask.NameToLayer("Weapon")) return;

            if (_assemblyState == WeaponState.Fire) {
                FirePartSelect(obj);
                return;
            }


            if (_assemblyState == WeaponState.FireAuto) {
                if (!_isFiring) {
                    _animator.CrossFade("FireSelect Auto", 0);
                    _isFiring = true;
                }
                else {
                    _animator.CrossFade("TriggerRelease", 0);
                    _isFiring = false;
                }
                return;
            }

            if (!obj.TryGetComponent<WeaponPartAnim>(out WeaponPartAnim part)) return;
            
            PartAnimState state = FindNode(rootState, part.AnimName);
            
            if (state == null) return;
            
            if (_assemblyState == WeaponState.Disassemble && state.parent.active && !state.active) {
                _animator.CrossFade(state.name, 0);
                state.active = true;
                SwitchCam(state, _assemblyState);
                state.obj.GetComponent<Outline>().enabled = false;
                AssignHint(_assemblyState);
                if (state.subStates.Count == 0) StartCoroutine(SwitchToMainCam(state));
            }

            if (_assemblyState == WeaponState.Assemble && state.active && (state.subStates == null || state.subStates.All(state => !state.active))) {
                _animator.CrossFade(state.name + "Reverse", 0);
                state.active = false;
                SwitchCam(state, _assemblyState);
                state.obj.GetComponent<Outline>().enabled = false;
                AssignHint(_assemblyState);
                StartCoroutine(SwitchToMainCam(state));
            }
        }

        private IEnumerator SwitchToMainCam(PartAnimState state) {
            float animLength = _animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animLength);
            if ((int)_assemblyState == 0) {
                SwitchCam(rootState, _assemblyState);
            }
            else {
                SwitchCam(state.parent, _assemblyState);
            }
        }

        private void FirePartSelect(GameObject obj) {
             if (!_hasFired) {
                 
                 _animator.CrossFade("FireSelect", 0);
             }
             else {
                 _animator.Play("Fire", -1, 0f);
             }
             _hasFired = true;
             SwitchCam(rootState, _assemblyState);           
        }

        private void SwitchCam(PartAnimState state, WeaponState weaponState) {
            PartAnimState camState = FindParent(state, n => n.cam != null);
            _switchCam.RaiseEvent(camState.cam);
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
            while (temp != null) {
                if (pred(temp)) return temp;
                temp = temp.parent;
            }

            return null;
        }
        
    }
}