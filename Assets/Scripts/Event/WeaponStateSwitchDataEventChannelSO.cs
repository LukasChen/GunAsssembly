using System.Collections.Generic;
using Cinemachine;
using GunAssembly;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName="Weapon Possibility State Event",menuName="Events/Weapon State Possibility Data Event Channel")]
public class WeaponStateSwitchStatusDataEventChannelSO : ScriptableObject {
    public UnityAction<List<WeaponState>> OnEventRaised;

    public void RaiseEvent(List<WeaponState> data) {
        OnEventRaised?.Invoke(data);
    }
}
