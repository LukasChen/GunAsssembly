using Cinemachine;
using GunAssembly;
using UnityEngine;
using UnityEngine.Events;
using GunAssembly.Weapon;

[CreateAssetMenu(fileName="Weapon State Event",menuName="Events/Weapon State Data Event Channel")]
public class WeaponStateDataEventChannelSO : ScriptableObject {
    public UnityAction<WeaponState> OnEventRaised;

    public void RaiseEvent(WeaponState data) {
        OnEventRaised?.Invoke(data);
    }
}
