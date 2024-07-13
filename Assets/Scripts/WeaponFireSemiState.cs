using UnityEngine;

namespace GunAssembly {
    public class WeaponFireSemiState : WeaponBaseState {
        private bool _hasFired = false;
        public WeaponFireSemiState(WeaponController weapon) : base(weapon) {
        }

        public override void OnPartSelected(GameObject obj) {
            Fire();
        }


        public override void EnterState() {
            weapon.OnWeaponStateChange.OnEventRaised += weapon.SwitchState;
            weapon.SwitchCam(weapon.rootState);
        }

        public override void ExitState() {
            weapon.OnWeaponStateChange.OnEventRaised -= weapon.SwitchState;
        }
        
        private void Fire() {
            if (!_hasFired) {
                 weapon.Animator.CrossFade("FireSelect", 0);
            }
            else {
                weapon.Animator.Play("Fire", -1, 0f);
            }
            _hasFired = true;
        }
    }
}