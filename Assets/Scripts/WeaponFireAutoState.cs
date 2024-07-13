using UnityEngine;

namespace GunAssembly {
    public class WeaponFireAutoState : WeaponBaseState {
        private bool _isFiring;
        
        public WeaponFireAutoState(WeaponController weapon) : base(weapon) {
        }

        public override void OnPartSelected(GameObject obj) {
            Fire();
        }

        private void Fire() {
            if (!_isFiring) {
                weapon.Animator.CrossFade("FireSelect Auto", 0);
                _isFiring = true;
            }
            else {
                weapon.Animator.CrossFade("TriggerRelease", 0);
                _isFiring = false;
            }
        }

        public override void EnterState() {
            weapon.OnWeaponStateChange.OnEventRaised += weapon.SwitchState;
        }

        public override void ExitState() {
            weapon.OnWeaponStateChange.OnEventRaised -= weapon.SwitchState;
        }
    }
}