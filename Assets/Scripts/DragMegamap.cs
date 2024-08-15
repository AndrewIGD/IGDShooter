using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragMegamap : MonoBehaviour, IDragHandler
{
    public static DragMegamap Instance;

    public void OnDrag(PointerEventData eventData)
    {
        if(eventData.button == 0)
        this.transform.position += (Vector3)eventData.delta;
    }

    private void Zoom(float amount)
    {
        if (amount > 0 && transform.localScale.x != 8)
        {
            CalculatePosition(amount);
        }
        else if (amount < 0 && transform.localScale.x != 1)
        {
            CalculatePosition(amount);
        }
    }

    private void CalculatePosition(float amount)
    {
        Vector2 mousePosition = Input.mousePosition;

        GameObject mousePos = new GameObject();
        mousePos.transform.parent = transform;
        mousePos.transform.position = Input.mousePosition;

        transform.localScale = new Vector2(transform.localScale.x + amount * 4, transform.localScale.y + amount * 4);

        transform.position = transform.position - (mousePos.transform.position - (Vector3)mousePosition);

        Destroy(mousePos);
    }

    // Start is called before the first frame update
    private void Start()
    {
        Instance = this;

        GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }

    // Update is called once per frame
    private void Update()
    {
        Zoom(Input.GetAxis("Mouse ScrollWheel"));

        if (transform.localScale.x < 1)
            transform.localScale = new Vector2(1, 1);
        if (transform.localScale.x > 8)
            transform.localScale = new Vector2(8, 8);

        float minX = -960 * (transform.localScale.x-1);
        float maxX = 960 * (transform.localScale.x-1);
        float minY = -512 * (transform.localScale.y-1);
        float maxY = 512 * (transform.localScale.y-1);

        if (transform.localPosition.x < minX)
        {
            transform.localPosition = new Vector2(minX, transform.localPosition.y);
        }
        if (transform.localPosition.x > maxX)
        {
            transform.localPosition = new Vector2(maxX, transform.localPosition.y);
        }
        if (transform.localPosition.y < minY)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, minY);
        }
        if (transform.localPosition.y > maxY)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, maxY);
        }
    }
}
