using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackwings.Events
{
    public class EndDragHandler : MonoBehaviour, IEndDragHandler
    {
        public System.Action<PointerEventData> onTrigger;

        public void OnEndDrag(PointerEventData data)
        {
            onTrigger?.Invoke(data);
        }
    }
}