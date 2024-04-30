using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackwings.Events
{
    public class PointerExitHandler : MonoBehaviour, IPointerExitHandler
    {
        public System.Action<PointerEventData> onTrigger;

        public void OnPointerExit(PointerEventData data)
        {
            onTrigger?.Invoke(data);
        }
    }
}