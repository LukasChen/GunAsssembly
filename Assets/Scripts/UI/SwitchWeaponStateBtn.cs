using UnityEngine;

namespace GunAssembly.UI {
    public class SwitchWeaponStateBtn : MonoBehaviour {
        [SerializeField] private WeaponState _weaponState;
        [SerializeField] private WeaponStateDataEventChannelSO _changeState;

        public void OnClick() {
            _changeState.RaiseEvent(_weaponState);
        }
    }
}