using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackwings.Events
{
    public class BeginDragHandler : MonoBehaviour, IBeginDragHandler
    {
        public System.Action<PointerEventData> onTrigger;

        public void OnBeginDrag(PointerEventData data)
        {
            onTrigger?.Invoke(data);
        }
    }
}