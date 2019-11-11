using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SunDragControllerMobile : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{

    Canvas canvas;

    [SerializeField]
    private Image sunSpace;

    [SerializeField]
    private SunControllerMobile sun;

    private bool allowingMovement = false;

    void Start()
    {
        canvas = transform.GetComponentInParent<Canvas>();
    }

    void Update()
    {

    }

    public void OnDrag(PointerEventData ped)
    {
        if (sun.movementType != SunControllerMobile.SunMovementType.DRAG)
            return;

        Vector2 pos = Vector2.zero;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(sunSpace.rectTransform, ped.position, ped.pressEventCamera, out pos))
        {
            float size = canvas.scaleFactor * 1920f;
            float halfSize = size / 2f;
            float sunPositionPercent = ((pos.x + halfSize) / (size));
            Debug.Log("OnDrag! scaled position = " + ((pos.x + halfSize) / (size)));
            sun.UpdateDragMovement(sunPositionPercent);
        }
    }

    public void OnPointerDown(PointerEventData ped)
    {
        if (sun.movementType != SunControllerMobile.SunMovementType.DRAG)
            return;

        Vector2 pos = Vector2.zero;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(sunSpace.rectTransform, ped.position, ped.pressEventCamera, out pos))
        {
            //allowingMovement = true;
            Debug.Log("Allowing movement");
        }
    }

    public void OnPointerUp(PointerEventData ped)
    {
        if (sun.movementType != SunControllerMobile.SunMovementType.DRAG)
            return;

        Vector2 pos = Vector2.zero;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(sunSpace.rectTransform, ped.position, ped.pressEventCamera, out pos))
        {
            //allowingMovement = false;
            Debug.Log("Not allowing movement");
        }
    }

}
