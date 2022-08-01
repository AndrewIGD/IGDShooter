using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapCreatorCameraDrag : MonoBehaviour, IDragHandler
{
    private MapCreatorCamera mainCamera;

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == 0 && mainCamera.Focused)
            Camera.main.transform.position -= (Vector3)eventData.delta * (Camera.main.orthographicSize * 0.0025f);
    }

    private void Start()
    {
        mainCamera = Camera.main.GetComponent<MapCreatorCamera>();
    }
}
