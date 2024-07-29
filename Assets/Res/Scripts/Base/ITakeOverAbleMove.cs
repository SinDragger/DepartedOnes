using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITakeOverAbleMove 
{
    void TakeOver();
    void ReturnBack();
    void InputMoveDirection(Vector2 vec);
}
