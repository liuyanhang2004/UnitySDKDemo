using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackwings.Events
{
    public class SelectHandler : MonoBehaviour, ISelectHandler
    {
        public System.Action<BaseEventData> onTrigger;

        public void OnSelect(BaseEventData data)
        {
            onTrigger?.Invoke(data);
        }
    }
}