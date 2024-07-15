using UnityEngine;

namespace GunAssembly.Weapon {
    
    public class WeaponFireSemiState : WeaponBaseState {
        private bool _hasFired = false;
        public WeaponFireSemiState(WeaponController weapon, WeaponState transition) : base(weapon, transition) {
        }

        public override void OnPartSelected(GameObject obj) {
            Fire();
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