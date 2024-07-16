using UnityEngine;

namespace GunAssembly {
    public class WeaponFireSemiState : WeaponBaseState {
        private bool _hasFired = false;
        public WeaponFireSemiState(WeaponController weapon, WeaponState transition) : base(weapon, transition) {
        }

        public override void OnPartSelected(GameObject obj) {
            Fire();
        }

        public override void PlaySFX() {
            AudioSource.PlayClipAtPoint(weapon.fireSFX, weapon.transform.position);
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

        public override void ExitState() {
            base.ExitState();
            weapon.Animator.StopPlayback();
        }
    }
}