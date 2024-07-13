using UnityEngine;

namespace GunAssembly {
    public abstract class WeaponBaseState {
        protected readonly WeaponController weapon;

        protected WeaponBaseState(WeaponController weapon) {
            this.weapon = weapon;
        }

        public abstract void OnPartSelected(GameObject obj);
        public abstract void EnterState();
        public abstract void ExitState();
    }
}