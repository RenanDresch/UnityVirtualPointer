using UnityEngine;
using UnityEngine.UI;

public class ColorSwitcher : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private Image image = default;

    #endregion

    #region Public Methods

    public void SetRedColor()
    {
        image.color = Color.red ;
    }

    public void SetBlueColor()
    {
        image.color = Color.blue;
    }

    public void SetGreenColor()
    {
        image.color = Color.green;
    }

    #endregion
}
