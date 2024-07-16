using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.VFX;

namespace GunAssembly {
    public class WeaponEffects : MonoBehaviour {
        
        [SerializeField] private Transform _caseSpawn;
        [SerializeField] private GameObject _casing;
        [SerializeField] private int _casePoolLength;
        [SerializeField] private VisualEffect[] _effects;
        [SerializeField] private Vector2 randomYOffset;
        [SerializeField] private Vector2 randomXOffset;
        [SerializeField] private float _despawnDelay = 0.5f;
        private Queue<GameObject> _casePool = new Queue<GameObject>();

        private void Start() {
            PrepPool();
        }
        
        
        public void ShellEject() {
            GameObject shell = _casePool.Dequeue();
            shell.transform.position = _caseSpawn.position;
            shell.SetActive(true);
            
            Rigidbody rb = shell.GetComponent<Rigidbody>();
            rb.velocity = new Vector3(0, Random.Range(randomYOffset.x, randomYOffset.y), Random.Range(randomXOffset.x, randomXOffset.y));
            rb.angularVelocity = new Vector3(10, 10, 10);

            foreach (var effect in _effects) {
                effect.Play();
            }

            StartCoroutine(DespawnCase(shell));
        }

        private IEnumerator DespawnCase(GameObject shell) {
            yield return new WaitForSeconds(_despawnDelay);
            shell.SetActive(false);
            _casePool.Enqueue(shell);
        }

        private void PrepPool() {
            for (int i = 0; i < _casePoolLength; i++) {
                GameObject casing = Instantiate(_casing, Vector3.zero, Quaternion.identity);
                _casePool.Enqueue(casing);
                casing.SetActive(false);
            }
        }
    }
}