using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MovementButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    [SerializeField]
    private float horizontalValueToAdd;
    [SerializeField]
    private float verticalValueToAdd;

    public void OnPointerDown(PointerEventData eventData) {
        InputMobile.Instance.AddInputValue(-horizontalValueToAdd, -verticalValueToAdd);
    }

    public void OnPointerUp(PointerEventData eventData) {
        InputMobile.Instance.AddInputValue(horizontalValueToAdd, verticalValueToAdd);
    }

}
