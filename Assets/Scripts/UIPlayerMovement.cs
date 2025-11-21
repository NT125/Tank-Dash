using UnityEngine;

public class UIPlayerMovement : MonoBehaviour
{
    public PlayerMovement player; // arrastrá tu jugador acá en el inspector

    public void OnLeftDown()
    {
        player.uiAxis = -1f;
    }

    public void OnLeftUp()
    {
        player.uiAxis = 0f;
    }

    public void OnRightDown()
    {
        player.uiAxis = 1f;
    }

    public void OnRightUp()
    {
        player.uiAxis = 0f;
    }

    public void OnJumpDown()
    {
        player.uiJumpPressed = true;
    }
}

