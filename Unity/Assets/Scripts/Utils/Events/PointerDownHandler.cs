using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackwings.Events
{
    public class PointerDownHandler : MonoBehaviour, IPointerDownHandler
    {
        public System.Action<PointerEventData> onTrigger;

        public void OnPointerDown(PointerEventData data)
        {
            onTrigger?.Invoke(data);
        }
    }
}