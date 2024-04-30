using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackwings.Events
{
    public class CancelHandler : MonoBehaviour, ICancelHandler
    {
        public System.Action<BaseEventData> onTrigger;

        public void OnCancel(BaseEventData data)
        {
            onTrigger?.Invoke(data);
        }
    }
}