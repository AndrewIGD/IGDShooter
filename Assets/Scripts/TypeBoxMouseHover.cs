using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TypeBoxMouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Chat.Instance.MouseEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Chat.Instance.MouseExit();
    }
}
