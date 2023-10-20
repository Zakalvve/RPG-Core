using RPG.Core;
using RPG.Core.ExtensionMethods;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace InventorySystem
{
    //Trachs mouse behaviour within a root visual element.
    //When drag behaviour is detected drag events are triggered and sent to the element in which the drag behaviour occured
    public sealed class MouseDragController : IResolver
    {
        public Vector2 DragMousePosition;
        public bool Enabled { get; set; } = true;
        //only elements that have the class specified by DragID applied will trigger drag events
        public string DragID
        {
            get => _dragId;
            set => _dragId = value;
        }
        //Elements that have the class specified by ActionID applied will trigger DragClick events when the conditions are met
        public string DragActionID
        {
            get => _dragActionId;
            set => _dragActionId = value;
        }

        //private
        private VisualElement _root;
        private VisualElement _dragTarget;
        private string _dragId = "draggable";
        private string _dragActionId = "dragable-action";
        private bool _isClicked = false;
        private bool _isDragging = false;
        private Vector2 _mousePositionAtDragStart;
        private float _deviation = 27f;
        private float _timeClicked = 0f;
        private float _delay = 0.3f;

        public bool IsDragging => _isDragging;
        public MouseDragController(UIDocument UI)
        {
            _root = UI.rootVisualElement;
            _root.RegisterCallback<MouseDownEvent>(HandleMouseDown);
            _root.RegisterCallback<MouseUpEvent>(HandleMouseUp);
            _root.RegisterCallback<DetachFromPanelEvent>(OnDisable);
            _root.RegisterCallback<MouseMoveEvent>(HandleMouseMove);
        }
        ~MouseDragController()
        {
            _root.UnregisterCallback<MouseDownEvent>(HandleMouseDown);
            _root.UnregisterCallback<MouseUpEvent>(HandleMouseUp);
            _root.UnregisterCallback<DetachFromPanelEvent>(OnDisable);
            _root.UnregisterCallback<MouseMoveEvent>(HandleMouseMove);
        }
        private void OnDisable(DetachFromPanelEvent e)
        {
            _root.UnregisterCallback<MouseDownEvent>(HandleMouseDown);
            _root.UnregisterCallback<MouseUpEvent>(HandleMouseUp);
            _root.UnregisterCallback<DetachFromPanelEvent>(OnDisable);
            _root.UnregisterCallback<MouseMoveEvent>(HandleMouseMove);
        }
        private void HandleMouseDown(MouseDownEvent e)
        {
            if (e.button != 0) return;
            if (!_isDragging)
            {
                if (FindDragableElement(e.mousePosition,out VisualElement target))
                {
                    _dragTarget = target;
                    if (TargetIsActionElement(target))
                    {
                        _timeClicked = Time.time;
                        _mousePositionAtDragStart = e.mousePosition;
                    }
                    else
                    {
                        _isDragging = true;
                        RaiseDragEvent(e.mousePosition,e.localMousePosition, target);
                    }
                }
            }

            DragMousePosition = e.mousePosition;
            _isClicked = true;
        }
        private void HandleMouseMove(MouseMoveEvent e)
        {
            if (!_isDragging && _isClicked)
            {
                if (_dragTarget != null && TargetIsActionElement(_dragTarget))
                {
                    if (Time.time > _timeClicked + _delay || Vector2.Distance(_mousePositionAtDragStart,e.mousePosition) > _deviation || e.mouseDelta.magnitude > _deviation)
                    {
                        _isDragging = true;
                        DragMousePosition = e.mousePosition;
                        RaiseDragEvent(e.mousePosition,e.localMousePosition,_dragTarget);
                    }
                }
            }

            if (_isDragging)
            {
                DragMousePosition = e.mousePosition;
                RaiseDragMoveEvent(e.mousePosition,e.localMousePosition,_dragTarget);
            }
        }
        private void HandleMouseUp(MouseUpEvent e)
        {
            if (e.button != 0) return;
            if (_isDragging)
            {
                RaiseDragEndEvent(e.mousePosition,e.localMousePosition,_dragTarget);
                _isDragging = false;
            }
            else if (Vector2.Distance(_mousePositionAtDragStart,e.mousePosition) < _deviation || Time.time < _timeClicked + _delay)
            {
                if (FindDragableElement(e.mousePosition,out VisualElement clickTarget))
                {
                    if (clickTarget == _dragTarget) RaiseDelayedClickEvent(e.mousePosition,e.localMousePosition,clickTarget);
                }
                _isDragging = false;
            }
            _isClicked = false;
        }
        private void RaiseDragEvent(Vector2 mousePosition,Vector2 localMousePosition, VisualElement target)
        {
            var e = DragStartEvent.GetPooled(mousePosition, localMousePosition);
            e.target = target;
            _root.schedule.Execute(() => e.target.SendEvent(e));
        }
        private void RaiseDragMoveEvent(Vector2 mousePosition,Vector2 localMousePosition,VisualElement target)
        {
            if (target == null) return;
            var e = DragMoveEvent.GetPooled(mousePosition,localMousePosition);
            e.target = target;
            _root.schedule.Execute(() => e.target.SendEvent(e));
        }
        private void RaiseDragEndEvent(Vector2 mousePosition,Vector2 localMousePosition,VisualElement target)
        {
            var e = DragEndEvent.GetPooled(mousePosition, localMousePosition, this);
            e.target = target;
            _root.schedule.Execute(() => e.target.SendEvent(e));
        }
        private void RaiseDelayedClickEvent(Vector2 mousePosition,Vector2 localMousePosition,VisualElement target)
        {
            var e = DragClickEvent.GetPooled(mousePosition,localMousePosition);
            e.target = target;
            _root.schedule.Execute(() => e.target.SendEvent(e));
        }
        private bool TargetIsActionElement(VisualElement target)
        {
            return target.ClassListContains(_dragActionId);
        }
        private bool FindElementBy(Vector2 position,out VisualElement found, Func<VisualElement,bool> predicate)
        {
            found = _root.panel.Pick(position);
            var element = found;
            while (element != null && predicate(element))
            {
                element = element.parent;
            }

            if (element != null)
            {
                found = element;
                return true;
            }
            return false;
        }
        public bool FindDragableElement(Vector2 position,out VisualElement found)
        {
            return FindElementById(_dragId,position,out found);
        }
        private bool FindElementById(string id,Vector2 position,out VisualElement found)
        {
            return FindElementBy(position, out found, (element) => { return !element.ClassListContains(id); });
        }
        //if the drop event is resolved we can null our drag target
        public void Resolve()
        {
            _dragTarget = null;
        }
        //if the drop event is rejected we ensure out internal state is unchanged
        public void Reject()
        {
            _isDragging = true;
        }
    }

    public class DragStartEvent : EventBase<DragStartEvent>
    {
        public Vector2 mousePosition { get; protected set; }
        public Vector2 localMousePosition { get; protected set; }
        public DragStartEvent() => Init();
        protected override void Init()
        {
            base.Init();
            bubbles = true;
            tricklesDown = false;
        }

        public static DragStartEvent GetPooled(Vector2 mousePosition, Vector2 localMousePosition)
        {
            DragStartEvent pooled = EventBase<DragStartEvent>.GetPooled();
            pooled.mousePosition = mousePosition;
            pooled.localMousePosition = localMousePosition;
            return pooled;
        }
    }
    public class DragMoveEvent : EventBase<DragMoveEvent>
    {
        public Vector2 mousePosition { get; protected set; }
        public Vector2 localMousePosition { get; protected set; }
        public DragMoveEvent() => Init();
        protected override void Init()
        {
            base.Init();
            bubbles = true;
            tricklesDown = false;
        }

        public static DragMoveEvent GetPooled(Vector2 mousePosition,Vector2 localMousePosition)
        {
            DragMoveEvent pooled = EventBase<DragMoveEvent>.GetPooled();
            pooled.mousePosition = mousePosition;
            pooled.localMousePosition = localMousePosition;
            return pooled;
        }
    }
    public class DragEndEvent : EventBase<DragEndEvent>
    {
        public IResolver resolver;
        public Vector2 mousePosition { get; protected set; }
        public Vector2 localMousePosition { get; protected set; }
        public DragEndEvent() => Init();
        protected override void Init()
        {
            base.Init();
            bubbles = true;
            tricklesDown = false;
        }

        public static DragEndEvent GetPooled(Vector2 mousePosition,Vector2 localMousePosition, IResolver r)
        {
            DragEndEvent pooled = EventBase<DragEndEvent>.GetPooled();
            pooled.mousePosition = mousePosition;
            pooled.localMousePosition = localMousePosition;
            pooled.resolver = r;
            return pooled;
        }
    }
    public class DragClickEvent : EventBase<DragClickEvent>
    {
        public Vector2 mousePosition { get; protected set; }
        public Vector2 localMousePosition { get; protected set; }
        public DragClickEvent() => Init();
        protected override void Init()
        {
            base.Init();
            bubbles = true;
            tricklesDown = false;
        }

        public static DragClickEvent GetPooled(Vector2 mousePosition,Vector2 localMousePosition)
        {
            DragClickEvent pooled = EventBase<DragClickEvent>.GetPooled();
            pooled.mousePosition = mousePosition;
            pooled.localMousePosition = localMousePosition;
            return pooled;
        }
    }
}
