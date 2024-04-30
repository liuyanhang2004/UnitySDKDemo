using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackwings.Events
{
    public class DeselectHandler : MonoBehaviour, IDeselectHandler
    {
        public System.Action<BaseEventData> onTrigger;

        public void OnDeselect(BaseEventData data)
        {
            onTrigger?.Invoke(data);
        }
    }
}