using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName="BoolEvent",menuName="Events/Bool Data Event Channel")]
public class BoolDataEventChannelSO : ScriptableObject {
    public UnityAction<bool> OnEventRaised;

    public void RaiseEvent(bool data) {
        OnEventRaised?.Invoke(data);
    }
}
