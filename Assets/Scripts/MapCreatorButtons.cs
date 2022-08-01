using SFB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapCreatorButtons : MonoBehaviour
{
    [SerializeField] GameObject tile;
    [SerializeField] GameObject tileParent;

    public static MapCreatorButtons Instance;

    public GameObject TileParent => tileParent;

    public GameObject TilePrefab => tile;

    public GameObject ctSpawn;

    public GameObject tSpawn;

    public bool Hidden => _hidden;

    private bool _hidden = true;

    private Text _text;


    public void NewTile()
    {
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
        };

        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);

        GameObject obj = Instantiate(this.tile);

        obj.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);

        Texture2D tex = new Texture2D(1, 1);
        WWW www = new WWW(paths[0]);
        www.LoadImageIntoTexture(tex);

        obj.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        BoxCollider2D objCollider = obj.AddComponent<BoxCollider2D>();
        objCollider.isTrigger = true;

        obj.transform.parent = tileParent.transform;

        Tile tile = obj.GetComponent<Tile>();
        tile.SetZ();
        tile.texturePath = paths[0];

        UnsavedChanges.Instance.Unsave();

        UndoAddObj addObjCommand = new UndoAddObj(obj);

        Undo.NewCommand(addObjCommand);
    }

    public void Leave()
    {
        SceneManager.LoadScene("MapEditorMenu");
    }

    public void SeeColls()
    {
        _hidden = !_hidden;

        foreach (Tile tile in TileData.Instance.Tiles)
        {
            if (tile.box.isTrigger == false)
            {
                tile.greenExtents.gameObject.SetActive(!_hidden);
            }
        }

        if (_hidden)
            _text.text = "See colliders";
        else _text.text = "Hide colliders";
    }

    public void AddTSpawn()
    {
        GameObject obj = Instantiate(tSpawn);

        obj.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 10001);

        obj.transform.parent = tileParent.transform;

        UnsavedChanges.Instance.Unsave();

        UndoAddObj addObjCommand = new UndoAddObj(obj);

        Undo.NewCommand(addObjCommand);
    }

    public void AddCTSpawn()
    {
        GameObject obj = Instantiate(ctSpawn);

        obj.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 10001);

        obj.transform.parent = tileParent.transform;

        UnsavedChanges.Instance.Unsave();

        UndoAddObj addObjCommand = new UndoAddObj(obj);

        Undo.NewCommand(addObjCommand);
    }

    private void Start()
    {
        _text = GetComponentInChildren<Text>();
    }
    private void Awake()
    {
        Instance = this;
    }
}
