using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackwings.Events
{
    public class DropHandler : MonoBehaviour, IDropHandler
    {
        public System.Action<PointerEventData> onTrigger;

        public void OnDrop(PointerEventData data)
        {
            onTrigger?.Invoke(data);
        }
    }
}