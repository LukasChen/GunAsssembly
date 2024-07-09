using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName="GameObject Event",menuName="Events/GameObject Data Event Channel")]
public class GameObjectDataEventChannelSO : ScriptableObject {
    public UnityAction<GameObject> OnEventRaised;

    public void RaiseEvent(GameObject data) {
        OnEventRaised?.Invoke(data);
    }
}
