using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackwings.Events
{
    public class PointerUpHandler : MonoBehaviour, IPointerUpHandler
    {
        public System.Action<PointerEventData> onTrigger;

        public void OnPointerUp(PointerEventData data)
        {
            onTrigger?.Invoke(data);
        }
    }
}