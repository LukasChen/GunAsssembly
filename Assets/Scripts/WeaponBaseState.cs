using UnityEngine;

namespace GunAssembly {
    public abstract class WeaponBaseState {
        protected readonly WeaponController weapon;
        protected readonly WeaponState transition;

        protected WeaponBaseState(WeaponController weapon, WeaponState transition) {
            this.weapon = weapon;
            this.transition = transition;
        }

        public abstract void OnPartSelected(GameObject obj);

        public virtual void EnterState() {
            weapon.OnWeaponStateChange.OnEventRaised += SwitchState;
            weapon.ActiveStateChannel.RaiseEvent(transition);
            Debug.Log("entered called");
        }

        public virtual void ExitState() {
            weapon.OnWeaponStateChange.OnEventRaised -= SwitchState;
        }

        private void SwitchState(WeaponState newState) {
            if ((transition & newState) != 0) {
                weapon.SwitchState(newState);
            }
        }
    }
}