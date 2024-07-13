using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName="Camera Event",menuName="Events/Camera Data Event Channel")]
public class CameraDataEventChannelSO : ScriptableObject {
    public UnityAction<ICinemachineCamera> OnEventRaised;

    public void RaiseEvent(ICinemachineCamera data) {
        OnEventRaised?.Invoke(data);
    }
}
