using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IngameUI : UICtrl
{
    [SerializeField] private GameJoystick moveJoystick = null;
    [SerializeField] private GameJoystick attackJoystick = null;

    public override void Initialize()
    {
        base.Initialize();

        moveJoystick.Initialize();
        moveJoystick.gameObject.SetActive(false);
        attackJoystick.Initialize();
        attackJoystick.gameObject.SetActive(false);
    }

    public void SetMoveEndEvent(Action action) { moveJoystick.onEndEvent = action; }
    public void SetAttackEndEvent(Action action) { attackJoystick.onEndEvent = action; }
 
}
