using UnityEngine;
using UnityEngine.EventSystems;

namespace Blackwings.Events
{
    public class SubmitHandler : MonoBehaviour, ISubmitHandler
    {
        public System.Action<BaseEventData> onTrigger;

        public void OnSubmit(BaseEventData data)
        {
            onTrigger?.Invoke(data);
        }
    }
}