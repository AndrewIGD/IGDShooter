using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CancelSelect : MonoBehaviour
{
    private MapCreatorCamera _mainCamera;

    private float _clicked = 0;
    private float _clickTime = 0;
    private float _clickDelay = 0.5f;

    public void OnMouseDown()
    {
        if (_mainCamera.CanDrag == false)
        {
            _clicked++;
            if (_clicked > 2 || Time.time - _clickTime > _clickDelay)
            {
                //First Click

                _clicked = 1;
                _clickTime = 0;
            }

            if (_clicked == 1) _clickTime = Time.time; //Second Click

            if (_clicked > 1 && Time.time - _clickTime < _clickDelay)
            {
                //Double Click Triggered

                _clicked = 0;
                _clickTime = 0;

                TileData.Instance.CurrentTileActive.Deselect();

                _mainCamera.Focus(null);
                TileData.Instance.CurrentObj = null;
            }
        }
    }

    void Start()
    {
        _mainCamera = Camera.main.GetComponent<MapCreatorCamera>();
    }
}
