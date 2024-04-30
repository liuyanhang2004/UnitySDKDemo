using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackwings.Events
{
    public class PointerClickHandler : MonoBehaviour, IPointerClickHandler
    {
        public System.Action<PointerEventData> onTrigger;

        public void OnPointerClick(PointerEventData data)
        {
            onTrigger?.Invoke(data);
        }
    }
}