using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gaze.VirtualPointer
{
    public class VirtualPointer : MonoBehaviour
    {
        #region Fields

        private EventSystem evSystem;

        private Selectable target;
        private Selectable pointerDownTarget;

        private IEndDragHandler dragEndHandler;
        private IDragHandler dragHandler;

        private List<RaycastResult> raycastResults = new List<RaycastResult>();
        private bool pointerDown;

        #endregion

        #region Private Properties

        private Selectable Target
        {
            get
            {
                return target;
            }
            set
            {
                if(value != target)
                {
                    if (target)
                    {
                        TriggerExit(target);
                        TriggerPointerUp(target);
                    }
                    if (value)
                    {
                        TriggerEnter(value);
                    }

                    target = value;
                }                
            }
        }
        private EventSystem EvSystem
        {
            get
            {
                if(!evSystem)
                {
                    evSystem = EventSystem.current != null ? EventSystem.current : gameObject.AddComponent<EventSystem>();
                    EventSystem.current = evSystem;
                }
                return evSystem;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Defines the pointer press state
        /// </summary>

        public bool PointerDown
        {
            get
            {
                return pointerDown;
            }
            set
            {
                if (value)
                {
                    GetDraggable();
                    if (Target)
                    {
                        //Unity Draggable elements will freak out if you call OnPointerDown on them!
                        //Seriously, take a look at the ScrollBar ClickRepeat shit, it makes no sense!!!
                        if (dragHandler == null)
                        {
                            TriggerPointerDown(Target);
                        }
                        pointerDownTarget = Target;
                    }
                }
                else
                {
                    EndDrag();
                    if (Target)
                    {
                        TriggerPointerUp(Target);
                        if (Target == pointerDownTarget)
                        {
                            TriggerSelect(Target);
                        }
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Moves the pointer
        /// </summary>
        /// <param name="movement">
        /// Vector2 composed by the horizontal and vertical movement
        /// </param>
        public void MovePointer(Vector2 movement)
        {
            var newPosition = transform.position + (Vector3)movement;
            newPosition.x = Mathf.Clamp(newPosition.x, 0, Screen.width);
            newPosition.y = Mathf.Clamp(newPosition.y, 0, Screen.height);

            transform.position = newPosition;
            GetTarget();
            TriggerDrag();
        }

        /// <summary>
        /// Sets the pointer position
        /// </summary>
        /// <param name="position">
        /// The pointer position
        /// </param>

        public void SetPointerPosition(Vector3 position)
        {
            var newPosition = position;
            newPosition.x = Mathf.Clamp(newPosition.x, 0, Screen.width);
            newPosition.y = Mathf.Clamp(newPosition.y, 0, Screen.height);

            transform.position = newPosition;
            GetTarget();
            TriggerDrag();
        }

        #endregion

        #region Private Methods

        private void Awake()
        {
            var rectTransform = GetComponent<RectTransform>();
            if(!rectTransform)
            {
                var canvas = GetComponentInParent<Canvas>();
                if (!canvas)
                {
                    Debug.LogError("Pointer not within a canvas!", gameObject);
                    Destroy(this);
                    return;
                }
                else
                {
                    rectTransform = gameObject.AddComponent<RectTransform>();
                }
            }
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
        }

        private void GetTarget()
        {
            var pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = transform.position;

            EvSystem.RaycastAll(pointerData, raycastResults);

            foreach(var result in raycastResults)
            {
                var selectable = result.gameObject.GetComponentInParent<Selectable>();
                if(selectable)
                {
                    if(selectable.interactable)
                    {
                        Target = selectable;
                        return;
                    }
                }
            }
            Target = null;
        }

        private void GetDraggable()
        {
            var pointerData = new PointerEventData(EvSystem);
            pointerData.position = transform.position;

            EvSystem.RaycastAll(pointerData, raycastResults);
            foreach (var result in raycastResults)
            {
                dragHandler = result.gameObject.GetComponentInParent<IDragHandler>();
                if (dragHandler != null)
                {
                    dragEndHandler = result.gameObject.GetComponentInParent<IEndDragHandler>();
                    result.gameObject.GetComponentInParent<IBeginDragHandler>().OnBeginDrag(pointerData);
                    result.gameObject.GetComponentInParent<IInitializePotentialDragHandler>().OnInitializePotentialDrag(pointerData);
                    dragHandler.OnDrag(pointerData);
                    return;
                }
            }
        }

        private void EndDrag()
        {
            if(dragEndHandler != null)
            {
                var pointerData = new PointerEventData(EvSystem);
                pointerData.position = transform.position;

                dragEndHandler.OnEndDrag(pointerData);
            }
            dragEndHandler = null;
            dragHandler = null;
        }

        private void TriggerEnter(Selectable selectable)
        {
            var pointerData = new PointerEventData(EvSystem);
            pointerData.position = transform.position;
            selectable.OnPointerEnter(pointerData);
        }

        private void TriggerExit(Selectable selectable)
        {
            var pointerData = new PointerEventData(EvSystem);
            pointerData.position = transform.position;
            selectable.OnPointerExit(pointerData);
        }

        private void TriggerPointerDown(Selectable selectable)
        {
            var pointerData = new PointerEventData(EvSystem);
            pointerData.position = transform.position;
            selectable.OnPointerDown(pointerData);
        }

        private void TriggerPointerUp(Selectable selectable)
        {
            var pointerData = new PointerEventData(EvSystem);
            pointerData.position = transform.position;
            selectable.OnPointerUp(pointerData);
        }

        private void TriggerSelect(Selectable selectable)
        {
            var pointerData = new PointerEventData(EvSystem);
            pointerData.position = transform.position;
            selectable.OnSelect(pointerData);

            var clickable = selectable.GetComponent<IPointerClickHandler>();

            if (clickable != null)
            {
                clickable.OnPointerClick(pointerData);
            }

            else
            {
                var submitable = selectable.GetComponent<ISubmitHandler>();

                if (submitable != null)
                {
                    submitable.OnSubmit(pointerData);
                }
            }
        }

        private void TriggerDrag()
        {
            if (dragHandler != null)
            {
                var pointerData = new PointerEventData(EvSystem);
                pointerData.position = transform.position;
                dragHandler.OnDrag(pointerData);
            }
        }

        #endregion

    }
}