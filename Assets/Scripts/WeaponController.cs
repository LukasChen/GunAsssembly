using UnityEditor.Animations;
using UnityEngine;

namespace GunAssembly {
    public class WeaponController : MonoBehaviour {
        [SerializeField] private AnimatorController _newAnim;

        private Animator _animator;

        private void Start() {
            _animator = GetComponent<Animator>();
        }
        
        public void SwitchAnim() {
            _animator.runtimeAnimatorController = _newAnim;
        }
    }
}