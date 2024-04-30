using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackwings.Events
{
    public class UpdateSelectedHandler : MonoBehaviour, IUpdateSelectedHandler
    {
        public System.Action<BaseEventData> onTrigger;

        public void OnUpdateSelected(BaseEventData data)
        {
            onTrigger?.Invoke(data);
        }
    }
}