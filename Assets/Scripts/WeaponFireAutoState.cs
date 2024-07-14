using UnityEngine;

namespace GunAssembly {
    public class WeaponFireAutoState : WeaponBaseState {
        private bool _isFiring;
        
        public WeaponFireAutoState(WeaponController weapon, WeaponState transition) : base(weapon, transition) {
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
                Release();
            }
        }

        private void Release() {
            weapon.Animator.CrossFade("TriggerRelease", 0);
            _isFiring = false;
        }

        public override void ExitState() {
            base.ExitState();
            Release();
        }
    }
}