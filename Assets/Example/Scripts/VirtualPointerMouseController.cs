using Gaze.VirtualPointer;
using UnityEngine;

public class VirtualPointerMouseController : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private VirtualPointer pointer = default;

    private Vector3 lastMousePosition;

    #endregion

    #region Private Methods

    private void Awake()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        var mouseDelta = Input.mousePosition - lastMousePosition;
        if(mouseDelta.magnitude != 0)
        {
            pointer.SetPointerPosition(Input.mousePosition);
        }
        if(Input.GetMouseButtonDown(0))
        {
            pointer.PointerDown = true;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            pointer.PointerDown = false;
        }
        lastMousePosition = Input.mousePosition;
    }

    #endregion
}
