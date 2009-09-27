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
using System.Collections.Generic;
using System;

public class InputState
{
    GamePadState current, old;
    TimeSpan oldTime, currentTime;
    List<Buttons> zuneButtons = new List<Buttons>(8);
    Dictionary<Buttons, TimeSpan> timeToLastPress = new Dictionary<Buttons, TimeSpan>();
    double holdTime = 1.0, repeatTime = .15;

    public InputState()
    {
        zuneButtons.Add(Buttons.A);
        zuneButtons.Add(Buttons.B);
        zuneButtons.Add(Buttons.Back);
        zuneButtons.Add(Buttons.DPadDown);
        zuneButtons.Add(Buttons.DPadUp);
        zuneButtons.Add(Buttons.DPadLeft);
        zuneButtons.Add(Buttons.DPadRight);
        //zuneButtons.Add(Buttons.LeftShoulder);

        foreach (Buttons button in zuneButtons)
        {
            timeToLastPress.Add(button, TimeSpan.Zero);
        }
    }

    public void Update(GameTime gameTime)
    {
        old = current;
        current = GamePad.GetState(PlayerIndex.One);
        oldTime = currentTime;
        currentTime = gameTime.TotalGameTime;

        foreach (Buttons button in zuneButtons)
        {
            if (IsNewButtonPress(button)) timeToLastPress[button] = gameTime.TotalGameTime;
        }
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

    public bool IsNewButtonHold(Buttons button)
    {
        return IsButtonPressed(button) && (currentTime - timeToLastPress[button]).TotalSeconds >= holdTime &&
            (oldTime - timeToLastPress[button]).TotalSeconds < holdTime;
    }

    public bool IsNewButtonHoldRelease(Buttons button)
    {
        return IsNewButtonRelease(button) && (oldTime - timeToLastPress[button]).TotalSeconds >= holdTime;
    }

    public bool IsNewButtonNotHoldRelease(Buttons button)
    {
        return IsNewButtonRelease(button) && (oldTime - timeToLastPress[button]).TotalSeconds < holdTime;
    }

    public bool IsNewButtonHoldRepeat(Buttons button)
    {
        return IsButtonPressed(button) && (oldTime - timeToLastPress[button]).TotalSeconds >= holdTime &&
            Math.Floor((oldTime - timeToLastPress[button]).TotalSeconds / repeatTime) <
            Math.Floor((currentTime - timeToLastPress[button]).TotalSeconds / repeatTime);
    }

    public bool IsNewButtonTick(Buttons button)
    {
        return IsNewButtonPress(button) || IsNewButtonHold(button) || IsNewButtonHoldRepeat(button);
    }

    public Vector2 TouchpadPosition
    {
        get { return current.ThumbSticks.Left; }
    }

    public bool HasTouchpad
    {
        get { return GamePad.GetCapabilities(PlayerIndex.One).HasLeftXThumbStick; }
    }
}