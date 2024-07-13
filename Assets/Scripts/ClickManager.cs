using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GunAssembly {
    public class ClickManager : MonoBehaviour {
        [SerializeField] private InputAction _click;
        [SerializeField] private GameObjectDataEventChannelSO _objClicked;

        private Camera _cam;

        private void Awake() {
            _cam = Camera.main;
            Application.targetFrameRate = 60;
        }

        private void OnEnable() {
            _click.Enable();
            _click.performed += OnClick;
        }

        private void OnDisable() {
            _click.Disable();
            _click.performed -= OnClick;
        }

        private void OnClick(InputAction.CallbackContext ctx) {
            Ray ray = _cam.ScreenPointToRay(ctx.ReadValue<Vector2>());
            if (Physics.Raycast(ray, out RaycastHit hit, 100)) {
                _objClicked?.RaiseEvent(hit.collider.gameObject);
            }
        }
    }
}