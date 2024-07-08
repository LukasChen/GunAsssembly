using UnityEngine;

namespace GunAssembly {
    public class ShellEject : MonoBehaviour {
        [SerializeField] private Transform _shellSpawn;

        public void Eject() {
            Debug.Log("Triggered");
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Rigidbody rb = cube.AddComponent<Rigidbody>();
            cube.transform.position = _shellSpawn.position;
            cube.transform.localScale = Vector3.one * 0.01f;
            rb.velocity = new Vector3(0, 5f, 1f);
            //rb.AddForce(transform.right * 20f, ForceMode.Impulse);
        }
    }
}