using UnityEngine;

namespace GunAssembly.Weapon {
    public class WeaponDefaultState : WeaponBaseState {
        public WeaponDefaultState(WeaponController weapon, WeaponState transition) : base(weapon, transition) {
        }

        public override void OnPartSelected(GameObject obj) { }
    }
}