using System.Collections;
using System.Linq;
using UnityEngine;

namespace GunAssembly {
    public class WeaponAssemblyState : WeaponBaseState {

        private bool _isAssembly = false;
        private PartAnimState _currentState;

        public WeaponAssemblyState(WeaponController weapon, WeaponState transition, bool isAssembly) : base(weapon, transition) {
            _isAssembly = isAssembly;
        }

        public override void OnPartSelected(GameObject obj) {
            if (!obj.TryGetComponent<WeaponPartAnim>(out WeaponPartAnim part)) return;
                        
            PartAnimState state = weapon.FindNode(weapon.rootState, part.AnimName);
            
            if (state == null) return;
            
            if (!_isAssembly && state.parent.active && !state.active) {
                weapon.Animator.CrossFade(state.name, 0);
                if (state.subStates.Count == 0) weapon.StartCoroutine(SwitchToMainCam(state));
                UpdatePart(state);
            }

            if (_isAssembly && state.active && (state.subStates == null || state.subStates.All(state => !state.active))) {
                weapon.Animator.CrossFade(state.name + "Reverse", 0);
                weapon.StartCoroutine(SwitchToMainCam(state));
                UpdatePart(state);
            }

            ValidateAssembly();
        }
        

        private void UpdatePart(PartAnimState state) {
            state.active = !_isAssembly;
            weapon.SwitchCam(state);
            AssignHint();
            state.obj.GetComponent<Outline>().enabled = false;
            if(state.sfx != null) AudioSource.PlayClipAtPoint(state.sfx, weapon.transform.position);
            _currentState = state;
        }

        public override void EnterState() {
            weapon.OnWeaponStateChange.OnEventRaised += SwitchState;
            weapon.rootState.active = !_isAssembly;
            AssignHint();
            ValidateAssembly();
        }

        public override void PlaySFX() {
            // if(_currentState.sfx != null) AudioSource.PlayClipAtPoint(_currentState.sfx, weapon.transform.position);
        }

        public override void ExitState() {
            weapon.OnWeaponStateChange.OnEventRaised -= SwitchState;
        }
        
        private bool ValidateAssembly() {
            PartAnimState remainingPart = weapon.DFS(weapon.rootState, n => n.active == _isAssembly && n.name != "Root");
            if (remainingPart != null) {
                weapon.ActiveStateChannel.RaiseEvent(WeaponState.None);
                return false;
            }
            weapon.ActiveStateChannel.RaiseEvent(transition);

            return true;
        }

        private void SwitchState(WeaponState newState) {
            if (!ValidateAssembly() && (transition & newState) == 0) {
                return;
            }
            weapon.SwitchState(newState);
        }
        
        private IEnumerator SwitchToMainCam(PartAnimState state) {
            float animLength = weapon.Animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animLength);
            var targetState = _isAssembly ? state.parent : state;
            weapon.SwitchCam(targetState);
        }
        
        private void AssignHint() {
            PartAnimState currentHint;
            if (_isAssembly) {
                PartAnimState deepest = weapon.FindDeepest(weapon.rootState, n => !n.active && n.name != "Root");
                currentHint = weapon.FindParent(deepest, n => n.active);
            }
            else {
                currentHint = weapon.DFS(weapon.rootState, n => !n.active);
            }
            if (currentHint == null) return;
            
            Debug.Log(currentHint.name);

            Outline outline = currentHint.obj.GetComponent<Outline>();
            outline.enabled = true;
        }
    }
}