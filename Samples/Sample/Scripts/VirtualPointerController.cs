using Gaze.VirtualPointer;
using UnityEngine;

public class VirtualPointerController : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private VirtualPointer pointer = default;

    [SerializeField]
    private float sensitivity = 15;

    [SerializeField]
    private string horizontalAxisName = "Horizontal";

    [SerializeField]
    private string verticalAxisName = "Vertical";

    [SerializeField]
    private string pointerButtonName = "Fire1";

    #endregion

    #region Private Methods

    private void Awake()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        var movement = new Vector2(Input.GetAxis(horizontalAxisName),
            Input.GetAxis(verticalAxisName));

        if(movement.magnitude != 0)
        {
            pointer.MovePointer(movement*sensitivity);
        }

        if(Input.GetButtonDown(pointerButtonName))
        {
            pointer.PointerDown = true;
        }
        else if(Input.GetButtonUp(pointerButtonName))
        {
            pointer.PointerDown = false;
        }
    }

    #endregion
}
