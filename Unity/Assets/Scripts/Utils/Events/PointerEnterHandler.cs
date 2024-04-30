using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackwings.Events
{
    public class PointerEnterHandler : MonoBehaviour, IPointerEnterHandler
    {
        public System.Action<PointerEventData> onTrigger;

        public void OnPointerEnter(PointerEventData data)
        {
            onTrigger?.Invoke(data);
        }
    }
}