using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    #region Public Variables
    public string texturePath;

    public GameObject[] borders;
    public BoxCollider2D box;

    public SpriteRenderer spriteExtents;
    public SpriteRenderer spriteGreenExtents;
    public Outline spriteExtentsOutline;
    public Outline greenExtents;

    public bool bombsite;
    public bool tSpawn;
    public bool ctSpawn;

    public bool pixelArt;

    #endregion

    #region Private Variables

    private MapCreatorCamera mainCamera;

    private TileData td;

    private SpriteRenderer spriteRenderer;

    private bool selected = false;

    private Outline outline;

    private float clicked = 0;

    private float clicktime = 0;

    private float clickdelay = 0.5f;

    private bool drag = false;

    private Vector3 offset;

    private Vector3 prevPos;

    #endregion

    // Start is called before the first frame update
    public void Start()
    {
        mainCamera = Camera.main.GetComponent<MapCreatorCamera>();

        td = TileData.Instance;

        spriteRenderer = GetComponent<SpriteRenderer>();

        outline = GetComponent<Outline>();

        if (spriteRenderer.sprite != null)
        {
            spriteExtents.transform.localPosition = new Vector3(0, 0, 1);

            spriteExtents.transform.localScale = new Vector2(spriteRenderer.bounds.size.x / spriteExtents.bounds.size.x, spriteRenderer.bounds.size.y / spriteExtents.bounds.size.y);

            spriteGreenExtents.transform.localPosition = new Vector3(0, 0, 1);

            spriteGreenExtents.transform.localScale = new Vector2(spriteRenderer.bounds.size.x / spriteExtents.bounds.size.x, spriteRenderer.bounds.size.y / spriteExtents.bounds.size.y);

            borders = new GameObject[4];

            box = GetComponent<BoxCollider2D>();

            GameObject posX = new GameObject();
            posX.transform.parent = transform;
            posX.transform.localPosition = new Vector2(box.size.x / 2, 0);
            borders[0] = posX;

            GameObject posY = new GameObject();
            posY.transform.parent = transform;
            posY.transform.localPosition = new Vector2(0, box.size.y / 2);
            borders[1] = posY;

            GameObject negX = new GameObject();
            negX.transform.parent = transform;
            negX.transform.localPosition = new Vector2(-box.size.x / 2, 0);
            borders[2] = negX;

            GameObject negY = new GameObject();
            negY.transform.parent = transform;
            negY.transform.localPosition = new Vector2(0, -box.size.y / 2);
            borders[3] = negY;
        }
    }

    public void Select()
    {
        TileData.Instance.CurrentTileActive.Deselect();

        if (transform.name.Contains("Ct") == false && transform.name.Contains("Tero") == false)
            td.ActivateTileData(gameObject);

        outline.eraseRenderer = false;

        spriteExtentsOutline.eraseRenderer = false;

        selected = true;

        mainCamera.Focus(this);
    }

    public void Deselect()
    {
        if (selected)
            td.DeactivateTileData();

        outline.eraseRenderer = true;

        spriteExtentsOutline.eraseRenderer = true;

        selected = false;
    }

    public void SetZ()
    {
        float z = 32768f;

        foreach (Transform tile in MapCreatorCamera.Instance.TileParent.transform)
        {
            if (tile.position.z < z && tile.gameObject.activeInHierarchy && tile != transform && tile.name.Contains("Ct") == false && tile.name.Contains("Tero") == false)
                z = tile.position.z;
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, z - 1);
    }

    public void OnMouseDown()
    {
        if (mainCamera.CanDrag == false)
        {
            if (selected == false)
            {
                clicked++;
                if (clicked > 2 || Time.time - clicktime > clickdelay)
                {
                    clicked = 1;
                    clicktime = 0;
                }
                if (clicked == 1) clicktime = Time.time;

                if (clicked > 1 && Time.time - clicktime < clickdelay)
                {
                    clicked = 0;
                    clicktime = 0;
                    Select();
                }
            }
            else
            {
                drag = true;

                prevPos = transform.position;

                offset = transform.position - MousePos.Position;
            }
        }
    }

    private void OnMouseDrag()
    {
        if (drag)
        {
            transform.position = new Vector3((MousePos.Position + offset).x, (MousePos.Position + offset).y, transform.position.z);
        }
    }

    private void OnMouseUp()
    {
        if (drag)
        {
            UnsavedChanges.Instance.Unsave();

            td.UpdateData();

            UndoMove moveCommand = new UndoMove(gameObject, prevPos, transform.position);

            Undo.NewCommand(moveCommand);
        }

        drag = false;
    }
}
