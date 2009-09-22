/*
 * Play     = Buttons.B
 * Back     = Buttons.Back
 * Center   = Buttons.A
 * Left     = Buttons.DPadLeft
 * Right    = Buttons.DPadRight
 * Up       = Buttons.DPadUp
 * Down     = Buttons.DPadDown
 * Touchpad = Buttons.LeftShoulder
 * Touch    = Buttons.LeftStick
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class InputState
{
    GamePadState current, old;

    public void Update()
    {
        old = current;
        current = GamePad.GetState(PlayerIndex.One);
    }

    public bool IsButtonPressed(Buttons button)
    {
        return current.IsButtonDown(button);
    }

    public bool IsNewButtonPress(Buttons button)
    {
        return current.IsButtonDown(button) && old.IsButtonUp(button);
    }

    public bool IsNewButtonRelease(Buttons button)
    {
        return current.IsButtonUp(button) && old.IsButtonDown(button);
    }

    //public bool IsNewButtonHold(Buttons button)
    //{
    //
    //}
    //
    //public bool IsNewButtonHoldRelease(Buttons button)
    //{
    //
    //}
    //
    //public bool IsNewButtonNotHoldRelease(Buttons button)
    //{
    //
    //}
}