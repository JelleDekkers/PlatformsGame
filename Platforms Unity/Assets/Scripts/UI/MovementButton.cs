using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MovementButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    [SerializeField]
    private IntVector2 moveDirectionValue;

    public void OnPointerDown(PointerEventData eventData) {
        InputMobile.Instance.AddInputValue(-moveDirectionValue.x, -moveDirectionValue.z);
    }

    public void OnPointerUp(PointerEventData eventData) {
        InputMobile.Instance.AddInputValue(moveDirectionValue.x, moveDirectionValue.z);
    }

}
