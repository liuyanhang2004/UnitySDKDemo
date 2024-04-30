using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackwings.Events
{
    public class ScrollHandler : MonoBehaviour, IScrollHandler
    {
        public System.Action<PointerEventData> onTrigger;

        public void OnScroll(PointerEventData data)
        {
            onTrigger?.Invoke(data);
        }
    }
}