using Events;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventTriggerX : MonoBehaviour
{
    public System.Action<PointerEventData> onPointerEnter
    {
        get => pointerEnterHandler.onTrigger;
        set => pointerEnterHandler.onTrigger = value;
    }

    public System.Action<PointerEventData> onPointerExit
    {
        get => pointerExitHandler.onTrigger;
        set => pointerExitHandler.onTrigger = value;
    }

    public System.Action<PointerEventData> onPointerDown
    {
        get => pointerDownHandler.onTrigger;
        set => pointerDownHandler.onTrigger = value;
    }

    public System.Action<PointerEventData> onPointerUp
    {
        get => pointerUpHandler.onTrigger;
        set => pointerUpHandler.onTrigger = value;
    }

    public System.Action<PointerEventData> onPointerClick
    {
        get => pointerClickHandler.onTrigger;
        set => pointerClickHandler.onTrigger = value;
    }

    public System.Action<PointerEventData> onInitializePotentialDrag
    {
        get => initializePotentialDragHandler.onTrigger;
        set => initializePotentialDragHandler.onTrigger = value;
    }

    public System.Action<PointerEventData> onBeginDrag
    {
        get => beginDragHandler.onTrigger;
        set => beginDragHandler.onTrigger = value;
    }

    public System.Action<PointerEventData> onDrag
    {
        get => dragHandler.onTrigger;
        set => dragHandler.onTrigger = value;
    }

    public System.Action<PointerEventData> onEndDrag
    {
        get => endDragHandler.onTrigger;
        set => endDragHandler.onTrigger = value;
    }

    public System.Action<PointerEventData> onDrop
    {
        get => dropHandler.onTrigger;
        set => dropHandler.onTrigger = value;
    }

    public System.Action<PointerEventData> onScroll
    {
        get => scrollHandler.onTrigger;
        set => scrollHandler.onTrigger = value;
    }

    public System.Action<BaseEventData> onUpdateSelected
    {
        get => updateSelectedHandler.onTrigger;
        set => updateSelectedHandler.onTrigger = value;
    }

    public System.Action<BaseEventData> onSelect
    {
        get => selectHandler.onTrigger;
        set => selectHandler.onTrigger = value;
    }

    public System.Action<BaseEventData> onDeselect
    {
        get => deselectHandler.onTrigger;
        set => deselectHandler.onTrigger = value;
    }

    public System.Action<AxisEventData> onMove
    {
        get => moveHandler.onTrigger;
        set => moveHandler.onTrigger = value;
    }

    public System.Action<BaseEventData> onSubmit
    {
        get => submitHandler.onTrigger;
        set => submitHandler.onTrigger = value;
    }

    public System.Action<BaseEventData> onCancel
    {
        get => cancelHandler.onTrigger;
        set => cancelHandler.onTrigger = value;
    }

    private PointerEnterHandler _pointerEnterHandler;
    private PointerEnterHandler pointerEnterHandler => _pointerEnterHandler ? _pointerEnterHandler : _pointerEnterHandler = gameObject.AddComponent<PointerEnterHandler>();

    private PointerExitHandler _pointerExitHandler;
    private PointerExitHandler pointerExitHandler => _pointerExitHandler ? _pointerExitHandler : _pointerExitHandler = gameObject.AddComponent<PointerExitHandler>();

    private PointerDownHandler _pointerDownHandler;
    private PointerDownHandler pointerDownHandler => _pointerDownHandler ? _pointerDownHandler : _pointerDownHandler = gameObject.AddComponent<PointerDownHandler>();

    private PointerUpHandler _pointerUpHandler;
    private PointerUpHandler pointerUpHandler => _pointerUpHandler ? _pointerUpHandler : _pointerUpHandler = gameObject.AddComponent<PointerUpHandler>();

    private PointerClickHandler _pointerClickHandler;
    private PointerClickHandler pointerClickHandler => _pointerClickHandler ? _pointerClickHandler : _pointerClickHandler = gameObject.AddComponent<PointerClickHandler>();

    private InitializePotentialDragHandler _initializePotentialDragHandler;
    private InitializePotentialDragHandler initializePotentialDragHandler => _initializePotentialDragHandler ? _initializePotentialDragHandler : _initializePotentialDragHandler = gameObject.AddComponent<InitializePotentialDragHandler>();

    private BeginDragHandler _beginDragHandler;
    private BeginDragHandler beginDragHandler => _beginDragHandler ? _beginDragHandler : _beginDragHandler = gameObject.AddComponent<BeginDragHandler>();

    private DragHandler _dragHandler;
    private DragHandler dragHandler => _dragHandler ? _dragHandler : _dragHandler = gameObject.AddComponent<DragHandler>();

    private EndDragHandler _endDragHandler;
    private EndDragHandler endDragHandler => _endDragHandler ? _endDragHandler : _endDragHandler = gameObject.AddComponent<EndDragHandler>();

    private DropHandler _dropHandler;
    private DropHandler dropHandler => _dropHandler ? _dropHandler : _dropHandler = gameObject.AddComponent<DropHandler>();

    private ScrollHandler _scrollHandler;
    private ScrollHandler scrollHandler => _scrollHandler ? _scrollHandler : _scrollHandler = gameObject.AddComponent<ScrollHandler>();

    private UpdateSelectedHandler _updateSelectedHandler;
    private UpdateSelectedHandler updateSelectedHandler => _updateSelectedHandler ? _updateSelectedHandler : _updateSelectedHandler = gameObject.AddComponent<UpdateSelectedHandler>();

    private SelectHandler _selectHandler;
    private SelectHandler selectHandler => _selectHandler ? _selectHandler : _selectHandler = gameObject.AddComponent<SelectHandler>();

    private DeselectHandler _deselectHandler;
    private DeselectHandler deselectHandler => _deselectHandler ? _deselectHandler : _deselectHandler = gameObject.AddComponent<DeselectHandler>();

    private MoveHandler _moveHandler;
    private MoveHandler moveHandler => _moveHandler ? _moveHandler : _moveHandler = gameObject.AddComponent<MoveHandler>();

    private SubmitHandler _submitHandler;
    private SubmitHandler submitHandler => _submitHandler ? _submitHandler : _submitHandler = gameObject.AddComponent<SubmitHandler>();

    private CancelHandler _cancelHandler;
    private CancelHandler cancelHandler => _cancelHandler ? _cancelHandler : _cancelHandler = gameObject.AddComponent<CancelHandler>();

    
}