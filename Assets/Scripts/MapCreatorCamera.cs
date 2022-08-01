using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;
using UnityEngine.UI;

public class MapCreatorCamera : MonoBehaviour
{
    #region Public Variables

    [SerializeField] Tile currentTile;
    [SerializeField] Texture2D[] cursors;
    [SerializeField] TileDrag drag;
    [SerializeField] GameObject tilesParent;
    [SerializeField] GameObject tSpawn;
    [SerializeField] GameObject ctSpawn;

    public static MapCreatorCamera Instance;

    public GameObject TileParent => tilesParent;

    public string SaveName => _saveName;

    public bool Focused => _focused;

    public bool CanDrag => _canDrag;

    #endregion

    #region Private Variables

    private bool _canDrag = false;

    private bool _focused = true;

    private string _saveName;

    private Camera _camera;

    private Vector3 _mousePos;
    private Vector3 _dragOrigin;

    private Vector3 _posXBorder;
    private Vector3 _negXBorder;
    private Vector3 _posYBorder;
    private Vector3 _negYBorder;

    private Tile _copiedTile;

    private Vector3 _tilePrevPos;
    private Vector3 _tilePrevSize;

    private Vector3 _origin;
    private Vector3 _diference;
    private bool _drag = false;

    #endregion

    public void Focus(Tile tile) { _focused = true; currentTile = tile; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _saveName = FindObjectOfType<LoadMap>().GetComponentInChildren<Text>().text;

        _camera = GetComponent<Camera>();

        drag = new TileDrag();

        tilesParent = GameObject.Find("Tiles");

        if (File.Exists(Application.dataPath + "\\Maps\\" + _saveName + "\\megamap.igd"))
        {
            string data = File.ReadAllText(Application.dataPath + "\\Maps\\" + _saveName + "\\megamap.igd");
            try
            {
                float size = float.Parse(data, CultureInfo.InvariantCulture);

                FindObjectOfType<ChangeMegamapSize>().SetSize(size);
            }
            catch
            {

            }
        }

        if (File.Exists(Application.dataPath + "\\Maps\\" + _saveName + "\\data.igd"))
        {
            string data = File.ReadAllText(Application.dataPath + "\\Maps\\" + _saveName + "\\data.igd");

            foreach (string line in data.Split('\n'))
            {
                string[] parameters = line.Split(' ');

                switch (parameters[0])
                {
                    case "Tile":
                        {
                            parameters[1] = Application.dataPath + "\\" + parameters[1];

                            GameObject obj = Instantiate(MapCreatorButtons.Instance.TilePrefab);

                            obj.transform.position = new Vector3(float.Parse(parameters[2], CultureInfo.InvariantCulture), float.Parse(parameters[3], CultureInfo.InvariantCulture), float.Parse(parameters[4], CultureInfo.InvariantCulture));
                            obj.transform.eulerAngles = new Vector3(float.Parse(parameters[11], CultureInfo.InvariantCulture), float.Parse(parameters[12], CultureInfo.InvariantCulture), float.Parse(parameters[13], CultureInfo.InvariantCulture));
                            obj.transform.localScale = new Vector2(float.Parse(parameters[5], CultureInfo.InvariantCulture), float.Parse(parameters[6], CultureInfo.InvariantCulture));

                            Texture2D tex = new Texture2D(1, 1);
                            WWW www = new WWW(parameters[1]);
                            www.LoadImageIntoTexture(tex);

                            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();

                            spriteRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));


                            Tile tile = obj.GetComponent<Tile>();

                            tile.bombsite = bool.Parse(parameters[7]);
                            tile.tSpawn = bool.Parse(parameters[8]);
                            tile.ctSpawn = bool.Parse(parameters[9]);

                            BoxCollider2D box = obj.AddComponent<BoxCollider2D>();

                            box.isTrigger = bool.Parse(parameters[10]);

                            obj.transform.parent = MapCreatorButtons.Instance.TileParent.transform;

                            tile.texturePath = parameters[1];

                            tile.pixelArt = bool.Parse(parameters[14]);

                            if (tile.pixelArt)
                                spriteRenderer.sprite.texture.filterMode = FilterMode.Point;

                            break;
                        }
                    case "CtSpawn":
                        {
                            GameObject obj = Instantiate(ctSpawn);

                            obj.transform.parent = MapCreatorButtons.Instance.TileParent.transform;

                            obj.transform.position = new Vector3(float.Parse(parameters[1], CultureInfo.InvariantCulture), float.Parse(parameters[2], CultureInfo.InvariantCulture), 10001); ;

                            break;
                        }
                    case "TSpawn":
                        {
                            GameObject obj = Instantiate(tSpawn);

                            obj.transform.parent = MapCreatorButtons.Instance.TileParent.transform;

                            obj.transform.position = new Vector3(float.Parse(parameters[1], CultureInfo.InvariantCulture), float.Parse(parameters[2], CultureInfo.InvariantCulture), 10001);

                            break;
                        }
                }

            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveCommand();
        }

        if (Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.LeftControl))
        {
            Undo.UndoCommand();
        }

        if (Input.GetKeyDown(KeyCode.Y) && Input.GetKey(KeyCode.LeftControl))
        {
            Undo.RedoCommand();
        }


        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            _mousePos = _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _camera.transform.position.z));
            _dragOrigin = _camera.ScreenToWorldPoint(new Vector3(UnityEngine.Screen.width / 2, UnityEngine.Screen.height / 2, _camera.transform.position.z));
            transform.position = new Vector3(transform.position.x + ((_mousePos.x - _dragOrigin.x) / (_camera.orthographicSize * (1 / (_camera.orthographicSize / 10)))), transform.position.y + ((_mousePos.y - _dragOrigin.y) / (_camera.orthographicSize * (1 / (_camera.orthographicSize / 10)))), transform.position.z);
            _camera.orthographicSize -= _camera.orthographicSize / 10;
            if (_camera.orthographicSize < 0.1f)
                _camera.orthographicSize = 0.1f;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            _mousePos = _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _camera.transform.position.z));
            _dragOrigin = _camera.ScreenToWorldPoint(new Vector3(UnityEngine.Screen.width / 2, UnityEngine.Screen.height / 2, _camera.transform.position.z));
            transform.position = new Vector3(transform.position.x - ((_mousePos.x - _dragOrigin.x) / (_camera.orthographicSize * (1 / (_camera.orthographicSize / 10)))), transform.position.y - ((_mousePos.y - _dragOrigin.y) / (_camera.orthographicSize * (1 / (_camera.orthographicSize / 10)))), transform.position.z);

            _mousePos = _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0 - _camera.transform.position.z));
            _dragOrigin = _camera.transform.position;
            _camera.orthographicSize += _camera.orthographicSize / 10;

            if (_camera.orthographicSize > 300f)
                _camera.orthographicSize = 300f;
        }

        if (drag.IsDragging())
        {
            if (drag.posXDrag)
            {
                currentTile.transform.position = new Vector3((_negXBorder + MousePos.Position).x / 2, currentTile.transform.position.y, currentTile.transform.position.z);

                currentTile.transform.localScale = new Vector3(Vector2.Distance(new Vector2(_negXBorder.x, 0), new Vector2(MousePos.Position.x, 0)) / currentTile.box.size.x, currentTile.transform.localScale.y, 1);
            }

            if (drag.negXDrag)
            {
                currentTile.transform.position = new Vector3((_posXBorder + MousePos.Position).x / 2, currentTile.transform.position.y, currentTile.transform.position.z);

                currentTile.transform.localScale = new Vector3(Vector2.Distance(new Vector2(_posXBorder.x, 0), new Vector2(MousePos.Position.x, 0)) / currentTile.box.size.x, currentTile.transform.localScale.y, 1);
            }

            if (drag.posYDrag)
            {
                currentTile.transform.position = new Vector3(currentTile.transform.position.x, (_negYBorder + MousePos.Position).y / 2, currentTile.transform.position.z);

                currentTile.transform.localScale = new Vector3(currentTile.transform.localScale.x, Vector2.Distance(new Vector2(0, _negYBorder.y), new Vector2(0, MousePos.Position.y)) / currentTile.box.size.y, 1);
            }

            if (drag.negYDrag)
            {
                currentTile.transform.position = new Vector3(currentTile.transform.position.x, (_posYBorder + MousePos.Position).y / 2, currentTile.transform.position.z);

                currentTile.transform.localScale = new Vector3(currentTile.transform.localScale.x, Vector2.Distance(new Vector2(0, _posYBorder.y), new Vector2(0, MousePos.Position.y)) / currentTile.box.size.y, 1);
            }

            if (Input.GetMouseButtonUp(0))
            {
                UnsavedChanges.Instance.Unsave();

                TileData.Instance.UpdateData();

                drag.Set(false);

                UndoResize resizeCommand = new UndoResize(currentTile.gameObject, _tilePrevPos, currentTile.transform.position, _tilePrevSize, currentTile.transform.localScale);

                Undo.NewCommand(resizeCommand);
            }
        }

        if (currentTile != null)
        {
            if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftControl))
            {
                Copy();
            }
            if (Input.GetKeyDown(KeyCode.V) && Input.GetKey(KeyCode.LeftControl) && _copiedTile != null)
            {
                Paste();
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                IncreaseZ();
            }
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                DecreaseZ();
            }

            if (Input.GetKeyDown(KeyCode.D) && Input.GetKey(KeyCode.LeftControl))
            {
                Duplicate();
            }

            Vector3 mousePos = MousePos.Position;
            Vector3 tilePos = currentTile.transform.position;
            Vector3 tileSize = currentTile.GetComponent<SpriteRenderer>().bounds.extents;

            Vector3 mousePosRelativeToTile = mousePos - tilePos;

            Vector3 mousePosRelativeToBounds = mousePosRelativeToTile - tileSize;

            bool ok = false;

            bool negX = false;
            bool negY = false;
            bool posX = false;
            bool posY = false;

            if (drag.IsDragging() == false && currentTile.gameObject.name.Contains("Ct") == false && currentTile.gameObject.name.Contains("Tero") == false && currentTile.transform.eulerAngles.z < 1 && currentTile.transform.eulerAngles.x < 1 && currentTile.transform.eulerAngles.y < 1)
            {
                if (mousePosRelativeToBounds.x < 0 && mousePosRelativeToBounds.x > -currentTile.GetComponent<BoxCollider2D>().size.x * currentTile.transform.localScale.x - 0.25f * (_camera.orthographicSize / 5f) && mousePosRelativeToBounds.y < 0.25f * (_camera.orthographicSize / 5f) && mousePosRelativeToBounds.y > -currentTile.GetComponent<BoxCollider2D>().size.y * currentTile.transform.localScale.y)
                {
                    if (mousePosRelativeToBounds.x < 0 && mousePosRelativeToBounds.x > -0.25f * (_camera.orthographicSize / 5f))
                    {
                        _canDrag = true;
                        posX = true;
                        ok = true;
                        if (Input.GetMouseButtonDown(0))
                        {
                            _tilePrevPos = currentTile.transform.position;
                            _tilePrevSize = currentTile.transform.localScale;

                            drag.posXDrag = true;

                            _negXBorder = currentTile.borders[2].transform.position;
                        }
                    }
                    else if (mousePosRelativeToBounds.x < -currentTile.GetComponent<BoxCollider2D>().size.x * currentTile.transform.localScale.x && mousePosRelativeToBounds.x > -currentTile.GetComponent<BoxCollider2D>().size.x * currentTile.transform.localScale.x - 0.25f * (_camera.orthographicSize / 5f))
                    {
                        _canDrag = true;
                        negX = true;
                        ok = true;
                        if (Input.GetMouseButtonDown(0))
                        {
                            _tilePrevPos = currentTile.transform.position;
                            _tilePrevSize = currentTile.transform.localScale;

                            drag.negXDrag = true;

                            _posXBorder = currentTile.borders[0].transform.position;
                        }
                    }
                    if (mousePosRelativeToBounds.y > 0 && mousePosRelativeToBounds.y < 0.25f * (_camera.orthographicSize / 5f))
                    {
                        _canDrag = true;
                        posY = true;
                        ok = true;
                        if (Input.GetMouseButtonDown(0))
                        {
                            _tilePrevPos = currentTile.transform.position;
                            _tilePrevSize = currentTile.transform.localScale;

                            drag.posYDrag = true;

                            _negYBorder = currentTile.borders[3].transform.position;
                        }
                    }
                    else if (mousePosRelativeToBounds.y > -currentTile.GetComponent<BoxCollider2D>().size.y * currentTile.transform.localScale.y && mousePosRelativeToBounds.y < -currentTile.GetComponent<BoxCollider2D>().size.y * currentTile.transform.localScale.y + 0.25f * (_camera.orthographicSize / 5f))
                    {
                        _canDrag = true;
                        negY = true;
                        ok = true;
                        if (Input.GetMouseButtonDown(0))
                        {
                            _tilePrevPos = currentTile.transform.position;
                            _tilePrevSize = currentTile.transform.localScale;

                            drag.negYDrag = true;

                            _posYBorder = currentTile.borders[1].transform.position;
                        }
                    }
                }
                if ((posX || negX) && ((posY || negY) == false))
                {
                    UnityEngine.Cursor.SetCursor(cursors[1], Vector2.zero, CursorMode.Auto);
                }
                else if ((posY || negY) && ((posX || negX) == false))
                {
                    UnityEngine.Cursor.SetCursor(cursors[2], Vector2.zero, CursorMode.Auto);
                }
                else if ((posX && posY) || (negX && negY))
                {
                    UnityEngine.Cursor.SetCursor(cursors[4], Vector2.zero, CursorMode.Auto);
                }
                else if ((posX && negY) || (negX && posY))
                {
                    UnityEngine.Cursor.SetCursor(cursors[3], Vector2.zero, CursorMode.Auto);
                }
                else UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                if (ok == false)
                {
                    _canDrag = false;
                }
            }
        }
        else _canDrag = false;
    }

    private void Duplicate()
    {
        GameObject newTile = Instantiate(currentTile.gameObject);

        newTile.transform.parent = tilesParent.transform;

        newTile.transform.position = new Vector3(newTile.transform.position.x, newTile.transform.position.y, newTile.transform.position.z - 1);

        currentTile = newTile.GetComponent<Tile>();

        currentTile.Start();

        currentTile.Select();
    }

    private void DecreaseZ()
    {
        float oldZ = currentTile.transform.position.z;
        if (Input.GetKey(KeyCode.LeftControl))
        {
            float z = -1f;

            foreach (Transform tile in tilesParent.transform)
            {
                if (tile.position.z > z && tile.gameObject.activeInHierarchy && tile.name.Contains("Ct") == false && tile.name.Contains("Tero") == false)
                    z = tile.position.z;
            }

            currentTile.transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, z + 1);
        }
        else currentTile.transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, currentTile.transform.position.z + 1);

        if (currentTile.transform.position.z > 32767)
            currentTile.transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, 32767);
        else
        {
            UndoSetZ zCommand = new UndoSetZ(currentTile.gameObject, oldZ, currentTile.transform.position.z);

            Undo.NewCommand(zCommand);
        }
    }

    private void IncreaseZ()
    {
        UnsavedChanges.Instance.Unsave();
        float oldZ = currentTile.transform.position.z;
        if (Input.GetKey(KeyCode.LeftControl))
        {
            float z = 32768f;

            foreach (Transform tile in tilesParent.transform)
            {
                if (tile.position.z < z && tile.gameObject.activeInHierarchy && tile.name.Contains("Ct") == false && tile.name.Contains("Tero") == false)
                    z = tile.position.z;
            }

            currentTile.transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, z - 1);
        }
        else currentTile.transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, currentTile.transform.position.z - 1);

        if (currentTile.transform.position.z < 0)
            currentTile.transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, 0);
        else
        {
            UndoSetZ zCommand = new UndoSetZ(currentTile.gameObject, oldZ, currentTile.transform.position.z);

            Undo.NewCommand(zCommand);
        }
    }

    private void Delete()
    {
        UnsavedChanges.Instance.Unsave();

        currentTile.gameObject.SetActive(false);

        UndoDelete deleteCommand = new UndoDelete(currentTile.gameObject);

        Undo.NewCommand(deleteCommand);
    }

    private void Paste()
    {
        UnsavedChanges.Instance.Unsave();
        GameObject newTile = Instantiate(_copiedTile.gameObject);

        newTile.SetActive(true);

        newTile.transform.parent = tilesParent.transform;

        newTile.transform.position = new Vector3(transform.position.x, transform.position.y, newTile.transform.position.z - 1);

        currentTile = newTile.GetComponent<Tile>();

        currentTile.Start();

        currentTile.Select();
    }

    private void Copy()
    {
        _copiedTile = Instantiate(currentTile);
        _copiedTile.gameObject.SetActive(false);
    }

    private void SaveCommand()
    {
        string text = "";

        Directory.CreateDirectory(Application.dataPath + "\\Cache");

        string cachePath = Application.dataPath + "\\Cache";

        List<string> texturePaths = new List<string>();

        List<int> textureIndexes = new List<int>();

        int texIndex = 0;

        foreach (Transform child in tilesParent.transform)
        {
            try
            {
                if (child.gameObject.activeInHierarchy && child.name.Contains("Ct") == false && child.name.Contains("Tero") == false)
                {

                    if (texturePaths.Contains(child.GetComponent<Tile>().texturePath) == false)
                    {
                        texturePaths.Add(child.GetComponent<Tile>().texturePath);

                        textureIndexes.Add(texIndex);

                        File.Copy(child.GetComponent<Tile>().texturePath, cachePath + "\\tex" + texIndex++.ToString(CultureInfo.InvariantCulture));

                    }

                    for (int i = 0; i < texturePaths.Count; i++)
                    {
                        if (texturePaths[i] == child.GetComponent<Tile>().texturePath)
                        {
                            child.GetComponent<Tile>().texturePath = Application.dataPath + "\\Maps\\" + _saveName + "\\Textures\\tex" + textureIndexes[i].ToString(CultureInfo.InvariantCulture);
                        }
                    }
                }
            }
            catch
            {

            }
        }

        try
        {
            Directory.Delete(Application.dataPath + "\\Maps\\" + _saveName + "\\Textures", true);
        }
        catch
        {

        }

        Directory.CreateDirectory(Application.dataPath + "\\Maps");
        Directory.CreateDirectory(Application.dataPath + "\\Maps\\" + _saveName);
        Directory.CreateDirectory(Application.dataPath + "\\Maps\\" + _saveName + "\\Textures");

        for (int i = 0; i < texturePaths.Count; i++)
        {
            File.Copy(Application.dataPath + "\\Cache\\tex" + i.ToString(CultureInfo.InvariantCulture), Application.dataPath + "\\Maps\\" + _saveName + "\\Textures\\tex" + textureIndexes[i].ToString(CultureInfo.InvariantCulture));
        }

        Directory.Delete(Application.dataPath + "\\Cache", true);

        foreach (Transform child in tilesParent.transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                if (child.name.Contains("Ct"))
                    text += "CtSpawn " + child.transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + child.transform.position.y.ToString(CultureInfo.InvariantCulture) + "\n";
                else if (child.name.Contains("Tero"))
                    text += "TSpawn " + child.transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + child.transform.position.y.ToString(CultureInfo.InvariantCulture) + "\n";
                else text += "Tile " + ("Maps\\" + child.GetComponent<Tile>().texturePath.Split(new string[] { @"Maps\" }, System.StringSplitOptions.None).Last()) + " " + child.transform.position.x.ToString(CultureInfo.InvariantCulture) + " " + child.transform.position.y.ToString(CultureInfo.InvariantCulture) + " " + child.transform.position.z.ToString(CultureInfo.InvariantCulture) + " " + child.transform.localScale.x.ToString(CultureInfo.InvariantCulture) + " " + child.transform.localScale.y.ToString(CultureInfo.InvariantCulture) + " " + child.GetComponent<Tile>().bombsite.ToString(CultureInfo.InvariantCulture) + " " + child.GetComponent<Tile>().tSpawn.ToString(CultureInfo.InvariantCulture) + " " + child.GetComponent<Tile>().ctSpawn.ToString(CultureInfo.InvariantCulture) + " " + child.GetComponent<BoxCollider2D>().isTrigger.ToString(CultureInfo.InvariantCulture) + " " + child.transform.eulerAngles.x.ToString(CultureInfo.InvariantCulture) + " " + child.transform.eulerAngles.y.ToString(CultureInfo.InvariantCulture) + " " + child.transform.eulerAngles.z.ToString(CultureInfo.InvariantCulture) + " " + child.GetComponent<Tile>().pixelArt.ToString(CultureInfo.InvariantCulture) + "\n";
            }
        }

        File.WriteAllText(Application.dataPath + "\\Maps\\" + _saveName + "\\data.igd", text);

        UnsavedChanges.Instance.Saved();
    }

    void LateUpdate()
    {

        if (Input.GetMouseButton(0) && _focused)
        {
            _diference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if (_drag == false)
            {
                _drag = true;
                _origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            _drag = false;
        }
        if (_drag == true)
        {
            Camera.main.transform.position = _origin - _diference;
        }

        if (transform.position.x + _camera.orthographicSize * _camera.aspect >= 300 * _camera.aspect)
        {
            transform.position = new Vector3(300 * _camera.aspect - _camera.orthographicSize * _camera.aspect, transform.position.y, -10);
        }
        else if (transform.position.x - _camera.orthographicSize * _camera.aspect <= -300 * _camera.aspect)
        {
            transform.position = new Vector3(-300 * _camera.aspect + _camera.orthographicSize * _camera.aspect, transform.position.y, -10);
        }
        if (transform.position.y + _camera.orthographicSize >= 300)
        {
            transform.position = new Vector3(transform.position.x, 300 - _camera.orthographicSize, -10);
        }
        else if (transform.position.y - _camera.orthographicSize <= -300)
        {
            transform.position = new Vector3(transform.position.x, -300 + _camera.orthographicSize, -10);
        }
    }

    public class TileDrag
    {

        public bool posXDrag = false;
        public bool negXDrag = false;
        public bool posYDrag = false;
        public bool negYDrag = false;

        public bool IsDragging()
        {
            return posXDrag || negXDrag || posYDrag || negYDrag;
        }
        public void Set(bool value)
        {
            posXDrag = negXDrag = posYDrag = negYDrag = value;
        }
    }
}

