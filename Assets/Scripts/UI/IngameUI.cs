using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

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

    public Vector3 GetMoveDirection() { return moveJoystick.GetInputDirection(); }
    public Vector3 GetAttackDirection() { return attackJoystick.GetInputDirection(); }

    public void OnPointerMoveDown(BaseEventData eventData)
    {
        if (moveJoystick.gameObject.activeSelf == false) moveJoystick.gameObject.SetActive(true);
        moveJoystick.GetRectTransform().position = ((PointerEventData)eventData).position;
        moveJoystick.Down((PointerEventData)eventData);
    }

    public void OnPointerMoveUp(BaseEventData eventData)
    {
        moveJoystick.Up((PointerEventData)eventData);
        moveJoystick.GetRectTransform().localPosition = Vector3.zero;
    }

    public void OnMoveDrag(BaseEventData eventData)
    {
        moveJoystick.Drag((PointerEventData)eventData);
    }

    public void OnPointerAttackDown(BaseEventData eventData)
    {
        if (attackJoystick.gameObject.activeSelf == false) attackJoystick.gameObject.SetActive(true);
        attackJoystick.GetRectTransform().position = ((PointerEventData)eventData).position;
        attackJoystick.Down((PointerEventData)eventData);
    }

    public void OnPointerAttackUp(BaseEventData eventData)
    {
        attackJoystick.Up((PointerEventData)eventData);
        attackJoystick.GetRectTransform().localPosition = Vector3.zero;
    }

    public void OnAttackDrag(BaseEventData eventData)
    {
        attackJoystick.Drag((PointerEventData)eventData);
    }

}
