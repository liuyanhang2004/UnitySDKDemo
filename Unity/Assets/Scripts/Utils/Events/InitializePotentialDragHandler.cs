using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackwings.Events
{
    public class InitializePotentialDragHandler : MonoBehaviour, IInitializePotentialDragHandler
    {
        public System.Action<PointerEventData> onTrigger;

        public void OnInitializePotentialDrag(PointerEventData data)
        {
            onTrigger?.Invoke(data);
        }
    }
}