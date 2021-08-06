using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameJoystick : MonoBehaviour
{
    [SerializeField] private Image knobImage = null;

    private Image bgImage = null;
    private RectTransform cachedTransform = null;
    private Vector3 inputVector = Vector3.zero;
    public System.Action onEndEvent = null;
    private float joystickHandleRatio = 2;
    public static GameJoystick current = null;

    public void Initialize()
    {
        bgImage = GetComponent<Image>();
    }

    public RectTransform GetRectTransform() 
    {
        if (cachedTransform == null) cachedTransform = GetComponent<RectTransform>();
        return cachedTransform;
    }

    public Vector3 GetInputDirection()
    {
        return inputVector;
    }

    public void Down(PointerEventData eventData)
    {
        if (knobImage != null)
            knobImage.rectTransform.anchoredPosition = Vector3.zero;

        current = this;

        Drag(eventData);
    }

    public void Up(PointerEventData eventData)
    {
        if (onEndEvent != null) onEndEvent();

        inputVector = Vector3.zero;
        current = null;

        if (knobImage != null) knobImage.rectTransform.anchoredPosition = Vector3.zero;
    }


    public void Drag(PointerEventData eventData)
    {
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImage.rectTransform , eventData.position , eventData.pressEventCamera , out Vector2 localPoint))
        {
            localPoint.x = (localPoint.x / bgImage.rectTransform.sizeDelta.x);
            localPoint.y = (localPoint.y / bgImage.rectTransform.sizeDelta.y);

            inputVector.x = localPoint.x;
            inputVector.y = localPoint.y;
            inputVector.z = 0;

            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            Vector3 position = Vector3.zero;

            position.x = inputVector.x * (bgImage.rectTransform.sizeDelta.x / joystickHandleRatio);
            position.y = inputVector.y * (bgImage.rectTransform.sizeDelta.y / joystickHandleRatio);

            knobImage.rectTransform.anchoredPosition = position;
        }
    }
}
