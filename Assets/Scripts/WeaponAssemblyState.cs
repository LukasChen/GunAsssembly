using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace GunAssembly {
    public class WeaponAssemblyState : WeaponBaseState {

        private bool _isAssembly = false;

        public WeaponAssemblyState(WeaponController weapon, bool isAssembly) : base(weapon) {
            _isAssembly = isAssembly;
        }

        public override void OnPartSelected(GameObject obj) {
            if (!obj.TryGetComponent<WeaponPartAnim>(out WeaponPartAnim part)) return;
                        
            PartAnimState state = weapon.FindNode(weapon.rootState, part.AnimName);
            
            if (state == null) return;
            
            if (!_isAssembly && state.parent.active && !state.active) {
                weapon.Animator.CrossFade(state.name, 0);
                state.active = true;
                weapon.SwitchCam(state);
                state.obj.GetComponent<Outline>().enabled = false;
                AssignHint();
                if (state.subStates.Count == 0) weapon.StartCoroutine(SwitchToMainCam(state));
            }

            if (_isAssembly && state.active && (state.subStates == null || state.subStates.All(state => !state.active))) {
                weapon.Animator.CrossFade(state.name + "Reverse", 0);
                state.active = false;
                weapon.SwitchCam(state);
                state.obj.GetComponent<Outline>().enabled = false;
                AssignHint();
                weapon.StartCoroutine(SwitchToMainCam(state));
            }
        }

        public override void EnterState() {
            weapon.OnWeaponStateChange.OnEventRaised += ValidateWeaponState;
            weapon.rootState.active = !_isAssembly;
            AssignHint();
        }

        public override void ExitState() {
            weapon.OnWeaponStateChange.OnEventRaised -= ValidateWeaponState;
        }
        
        private void ValidateWeaponState(WeaponState newState) {
            PartAnimState remainingPart = weapon.DFS(weapon.rootState, n => n.active == _isAssembly && n.name != "Root");
            if (remainingPart != null) return;

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