using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName="StringEvent",menuName="Events/String Data Event Channel")]
public class StringDataEventChannelSO : ScriptableObject {
    public UnityAction<string> OnEventRaised;

    public void RaiseEvent(string data) {
        OnEventRaised?.Invoke(data);
    }
}
