using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackwings.Events
{
    public class MoveHandler : MonoBehaviour, IMoveHandler
    {
        public System.Action<AxisEventData> onTrigger;

        public void OnMove(AxisEventData data)
        {
            onTrigger?.Invoke(data);
        }
    }
}