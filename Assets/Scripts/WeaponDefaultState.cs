using UnityEngine;

namespace GunAssembly {
    public class WeaponDefaultState : WeaponBaseState {
        public WeaponDefaultState(WeaponController weapon) : base(weapon) {
        }

        public override void OnPartSelected(GameObject obj) {
            throw new System.NotImplementedException();
        }

        public override void EnterState() {
            weapon.OnWeaponStateChange.OnEventRaised += weapon.SwitchState;
        }

        public override void ExitState() {
            weapon.OnWeaponStateChange.OnEventRaised -= weapon.SwitchState;
        }
    }
}