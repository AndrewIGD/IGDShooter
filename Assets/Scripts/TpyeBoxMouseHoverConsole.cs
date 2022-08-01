using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TpyeBoxMouseHoverConsole : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Console.Instance.MouseEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Console.Instance.MouseExit();
    }
}
