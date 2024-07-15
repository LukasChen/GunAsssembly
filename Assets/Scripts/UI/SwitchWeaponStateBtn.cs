using System;
using UnityEngine;
using UnityEngine.UI;
using GunAssembly.Weapon;

namespace GunAssembly.UI {
    public class SwitchWeaponStateBtn : MonoBehaviour {
        [SerializeField] private WeaponState _weaponState;
        [SerializeField] private WeaponStateDataEventChannelSO _changeState;
        [SerializeField] private WeaponStateDataEventChannelSO _currentActiveState;

        private Image _btnBg;

        private void Awake() {
            _btnBg = GetComponent<Image>();
        }

        private void OnEnable() {
            _currentActiveState.OnEventRaised += CheckActive;
        }

        private void OnDisable() {
            _currentActiveState.OnEventRaised -= CheckActive;
        }

        private void CheckActive(WeaponState state) {
            _btnBg.color = (_weaponState & state) == 0 ? new Color(1, 1, 1, 0.3f) : Color.white;
        }

        public void OnClick() {
            _changeState.RaiseEvent(_weaponState);
        }
    }
}