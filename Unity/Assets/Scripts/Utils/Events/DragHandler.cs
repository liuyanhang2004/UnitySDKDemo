using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackwings.Events
{
    public class DragHandler : MonoBehaviour, IDragHandler
    {
        public System.Action<PointerEventData> onTrigger;

        public void OnDrag(PointerEventData data)
        {
            onTrigger?.Invoke(data);
        }
    }
}