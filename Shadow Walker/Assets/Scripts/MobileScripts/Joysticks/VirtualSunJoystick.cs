using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualSunJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler 
{
    private Image outerCircle;
    private Image innerCircle;

    private Vector3 inputVector { set; get; }

    private void Start()
    {
        outerCircle = GetComponent<Image>();
        innerCircle = transform.GetChild(0).GetComponent<Image>();
    }

    public virtual void OnDrag(PointerEventData ped)
    {
        Vector2 pos = Vector2.zero;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(outerCircle.rectTransform, ped.position, ped.pressEventCamera, out pos))
        {
            //Debug.Log("Clicked Within the joystick image");
            //pos.x = (pos.x / innerCircle.rectTransform.sizeDelta.x);
            //pos.y = (pos.y / innerCircle.rectTransform.sizeDelta.y);
            pos.x = (pos.x / outerCircle.rectTransform.sizeDelta.x);
            pos.y = (pos.y / outerCircle.rectTransform.sizeDelta.y);


            float x = (outerCircle.rectTransform.pivot.x == 1) ? pos.x * 2 + 1 : pos.x * 2 - 1;
            float y = (outerCircle.rectTransform.pivot.y == 1) ? pos.y * 2 + 1 : pos.y * 2 - 1;

            inputVector = new Vector3(x, y, 0);
            //inputVector = new Vector3(pos.x * 2f, 0f, pos.y * 2f);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;
            Debug.Log(inputVector);

            innerCircle.rectTransform.anchoredPosition = new Vector3(inputVector.x * (outerCircle.rectTransform.sizeDelta.x / 2.5f), inputVector.y * (outerCircle.rectTransform.sizeDelta.y / 2.5f));
        }
    }

    public virtual void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);
    }

    public virtual void OnPointerUp(PointerEventData ped)
    {
        inputVector = Vector3.zero;
        innerCircle.rectTransform.anchoredPosition = Vector3.zero;
    }

    public float Horizontal()
    {
        return inputVector.x;
    }

    public float Vertical()
    {
        return inputVector.y;
    }
}
