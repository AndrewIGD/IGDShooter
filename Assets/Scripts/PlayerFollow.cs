using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    private List<GameObject> _exceptions = new List<GameObject>();

    private Player _target;

    private int index = 0;

    private int team;

    public void AddException(GameObject obj) => _exceptions.Add(obj);

    public void GetTarget(Player target) => _target = target;

    public void ClearTarget()
    {
        _target = null;
    }

    private void Start()
    {
        // set the desired aspect ratio (the values in this example are
        // hard-coded for 16:9, but you could make them into public
        // variables instead so you can set them at design time)
        float targetaspect = 16.0f / 9.0f;

        // determine the game window's current aspect ratio
        float windowaspect = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetaspect;

        // obtain camera component so we can modify its viewport
        Camera camera = GetComponent<Camera>();

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }

    private void LateUpdate()
    {
        if (_target != null)
            if (_target.Dead)
                _target = null;

        if (_target == null)
            GetNewTarget();

        if (_target == null)
            return;

        team = _target.Team;
        transform.position = new Vector3(_target.transform.position.x, _target.transform.position.y, -10);
    }

    private void GetNewTarget()
    {
        if (Chat.Instance.Focused == false && ConsoleCanvas.Instance.Content.activeInHierarchy == false)
        {
            if (Input.GetMouseButtonDown(0) && DragMegamap.Instance == null)
            {
                index++;
            }
            if (Input.GetMouseButtonDown(1))
            {
                index--;
            }
        }

        List<Player> players = new List<Player>();
        foreach (Player player in GameClient.Instance.AlivePlayers)
        {
            if (player.Controllable && (player.Team == GameClient.Instance.Team || GameClient.Instance.Team == 2) && _exceptions.Contains(player.gameObject) == false)
            {
                players.Add(player);
            }
        }

        if (index >= players.Count)
        {
            index = 0;
        }

        if (index < 0)
        {
            index = players.Count - 1;
        }

        try
        {
            transform.position = new Vector3(players[index].transform.position.x, players[index].transform.position.y, -10);
        }
        catch
        {
            if(GameHost.Instance.bombObject != null)
                transform.position = new Vector3(GameHost.Instance.bombObject.transform.position.x, GameHost.Instance.bombObject.transform.position.y, -10);
        }
    }
}
