using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class TileData : MonoBehaviour
{
    [SerializeField] InputField xPos;
    [SerializeField] InputField yPos;
    [SerializeField] InputField zPos;
    [SerializeField] InputField xRot;
    [SerializeField] InputField yRot;
    [SerializeField] InputField zRot;
    [SerializeField] InputField xScale;
    [SerializeField] InputField yScale;

    [SerializeField] Toggle bombsite;
    [SerializeField] Toggle tSpawn;
    [SerializeField] Toggle ctSpawn;
    [SerializeField] Toggle boxCollider;
    [SerializeField] Toggle pixelArt;

    [SerializeField] GameObject content;
    [SerializeField] GameObject content2;

    public static TileData Instance;

    public List<Tile> Tiles;

    public Tile CurrentTileActive;

    public GameObject CurrentObj;

    private void Awake()
    {
        Instance = this;
    }

    public void ActivateTileData(GameObject obj)
    {
        CurrentObj = obj;

        xPos.text = obj.transform.position.x.ToString(CultureInfo.InvariantCulture);
        yPos.text = obj.transform.position.y.ToString(CultureInfo.InvariantCulture);
        zPos.text = obj.transform.position.z.ToString(CultureInfo.InvariantCulture);
        xRot.text = obj.transform.eulerAngles.x.ToString(CultureInfo.InvariantCulture);
        yRot.text = obj.transform.eulerAngles.y.ToString(CultureInfo.InvariantCulture);
        zRot.text = obj.transform.eulerAngles.z.ToString(CultureInfo.InvariantCulture);
        xScale.text = obj.transform.localScale.x.ToString(CultureInfo.InvariantCulture);
        yScale.text = obj.transform.localScale.y.ToString(CultureInfo.InvariantCulture);

        bombsite.isOn = obj.GetComponent<Tile>().bombsite;
        tSpawn.isOn = obj.GetComponent<Tile>().tSpawn;
        ctSpawn.isOn = obj.GetComponent<Tile>().ctSpawn;
        boxCollider.isOn = !obj.GetComponent<BoxCollider2D>().isTrigger;
        pixelArt.isOn = obj.GetComponent<Tile>().pixelArt;

        content.SetActive(true);
        content2.SetActive(true);
    }

    public void DeactivateTileData()
    {
        content.SetActive(false);
        content2.SetActive(false);
    }

    public void UpdateData()
    {
        if (CurrentObj != null)
        {
            xPos.text = CurrentObj.transform.position.x.ToString(CultureInfo.InvariantCulture);
            yPos.text = CurrentObj.transform.position.y.ToString(CultureInfo.InvariantCulture);
            zPos.text = CurrentObj.transform.position.z.ToString(CultureInfo.InvariantCulture);
            xRot.text = CurrentObj.transform.eulerAngles.x.ToString(CultureInfo.InvariantCulture);
            yRot.text = CurrentObj.transform.eulerAngles.y.ToString(CultureInfo.InvariantCulture);
            zRot.text = CurrentObj.transform.eulerAngles.z.ToString(CultureInfo.InvariantCulture);
            xScale.text = CurrentObj.transform.localScale.x.ToString(CultureInfo.InvariantCulture);
            yScale.text = CurrentObj.transform.localScale.y.ToString(CultureInfo.InvariantCulture);

            bombsite.isOn = CurrentObj.GetComponent<Tile>().bombsite;
            tSpawn.isOn = CurrentObj.GetComponent<Tile>().tSpawn;
            ctSpawn.isOn = CurrentObj.GetComponent<Tile>().ctSpawn;
            boxCollider.isOn = !CurrentObj.GetComponent<BoxCollider2D>().isTrigger;
            pixelArt.isOn = CurrentObj.GetComponent<Tile>().pixelArt;
        }
    }

    public void ChangeX(float data)
    {
        UndoData undoData = new UndoData(CurrentObj, CurrentObj.transform.position, CurrentObj.transform.eulerAngles, CurrentObj.transform.localScale, new Vector3(data, CurrentObj.transform.position.y, CurrentObj.transform.position.z), CurrentObj.transform.eulerAngles, CurrentObj.transform.localScale, CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().pixelArt, CurrentObj.GetComponent<Tile>().pixelArt);

        Undo.NewCommand(undoData);
        UnsavedChanges.Instance.Unsave();
        CurrentObj.transform.position = new Vector3(data, CurrentObj.transform.position.y, CurrentObj.transform.position.z);
    }
    public void ChangeY(float data)
    {
        UndoData undoData = new UndoData(CurrentObj, CurrentObj.transform.position, CurrentObj.transform.eulerAngles, CurrentObj.transform.localScale, new Vector3(CurrentObj.transform.position.x, data, CurrentObj.transform.position.z), CurrentObj.transform.eulerAngles, CurrentObj.transform.localScale, CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().pixelArt, CurrentObj.GetComponent<Tile>().pixelArt);

        Undo.NewCommand(undoData);
        UnsavedChanges.Instance.Unsave();
        CurrentObj.transform.position = new Vector3(CurrentObj.transform.position.x, data, CurrentObj.transform.position.z);
    }
    public void ChangeZ(float data)
    {
        if (data < 0)
            data = 0;
        if (data > 32767)
            data = 32767;

        UndoData undoData = new UndoData(CurrentObj, CurrentObj.transform.position, CurrentObj.transform.eulerAngles, CurrentObj.transform.localScale, new Vector3(CurrentObj.transform.position.x, CurrentObj.transform.position.y, data), CurrentObj.transform.eulerAngles, CurrentObj.transform.localScale, CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().pixelArt, CurrentObj.GetComponent<Tile>().pixelArt);

        Undo.NewCommand(undoData);
        UnsavedChanges.Instance.Unsave();
        CurrentObj.transform.position = new Vector3(CurrentObj.transform.position.x, CurrentObj.transform.position.y, data);

        if (data <= 10000)
            CurrentObj.GetComponent<SpriteRenderer>().sortingLayerName = "AbovePlayer";
        else CurrentObj.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
    }
    public void ChangeRotX(float data)
    {
        if (data < 0)
        {
            data = 360 + data;
            xRot.text = data.ToString(CultureInfo.InvariantCulture);
        }



        UndoData undoData = new UndoData(CurrentObj, CurrentObj.transform.position, CurrentObj.transform.eulerAngles, CurrentObj.transform.localScale, CurrentObj.transform.position, new Vector3(data, CurrentObj.transform.eulerAngles.y, CurrentObj.transform.eulerAngles.z), CurrentObj.transform.localScale, CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().pixelArt, CurrentObj.GetComponent<Tile>().pixelArt);

        Undo.NewCommand(undoData);
        UnsavedChanges.Instance.Unsave();
        CurrentObj.transform.eulerAngles = new Vector3(data, CurrentObj.transform.eulerAngles.y, CurrentObj.transform.eulerAngles.z);
    }
    public void ChangeRotY(float data)
    {
        if (data < 0)
        {
            data = 360 + data;
            yRot.text = data.ToString(CultureInfo.InvariantCulture);
        }

        UndoData undoData = new UndoData(CurrentObj, CurrentObj.transform.position, CurrentObj.transform.eulerAngles, CurrentObj.transform.localScale, CurrentObj.transform.position, CurrentObj.transform.eulerAngles = new Vector3(CurrentObj.transform.eulerAngles.x, data, CurrentObj.transform.eulerAngles.z), CurrentObj.transform.localScale, CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().pixelArt, CurrentObj.GetComponent<Tile>().pixelArt);

        Undo.NewCommand(undoData);
        UnsavedChanges.Instance.Unsave();
        CurrentObj.transform.eulerAngles = new Vector3(CurrentObj.transform.eulerAngles.x, data, CurrentObj.transform.eulerAngles.z);
    }
    public void ChangeRotZ(float data)
    {
        if (data < 0)
        {
            data = 360 + data;
            zRot.text = data.ToString(CultureInfo.InvariantCulture);
        }

        UndoData undoData = new UndoData(CurrentObj, CurrentObj.transform.position, CurrentObj.transform.eulerAngles, CurrentObj.transform.localScale, CurrentObj.transform.position, CurrentObj.transform.eulerAngles = new Vector3(CurrentObj.transform.eulerAngles.x, CurrentObj.transform.eulerAngles.y, data), CurrentObj.transform.localScale, CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().pixelArt, CurrentObj.GetComponent<Tile>().pixelArt);

        Undo.NewCommand(undoData);
        UnsavedChanges.Instance.Unsave();
        CurrentObj.transform.eulerAngles = new Vector3(CurrentObj.transform.eulerAngles.x, CurrentObj.transform.eulerAngles.y, data);
    }
    public void ChangeScaleX(float data)
    {
        UndoData undoData = new UndoData(CurrentObj, CurrentObj.transform.position, CurrentObj.transform.eulerAngles, CurrentObj.transform.localScale, CurrentObj.transform.position, CurrentObj.transform.eulerAngles, new Vector3(data, CurrentObj.transform.localScale.y), CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().pixelArt, CurrentObj.GetComponent<Tile>().pixelArt);

        Undo.NewCommand(undoData);
        UnsavedChanges.Instance.Unsave();
        CurrentObj.transform.localScale = new Vector3(data, CurrentObj.transform.localScale.y);
    }
    public void ChangeScaleY(float data)
    {
        UndoData undoData = new UndoData(CurrentObj, CurrentObj.transform.position, CurrentObj.transform.eulerAngles, CurrentObj.transform.localScale, CurrentObj.transform.position, CurrentObj.transform.eulerAngles, new Vector3(CurrentObj.transform.localScale.x, data), CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().pixelArt, CurrentObj.GetComponent<Tile>().pixelArt);

        Undo.NewCommand(undoData);
        UnsavedChanges.Instance.Unsave();
        CurrentObj.transform.localScale = new Vector3(CurrentObj.transform.localScale.x, data);
    }

    public void ChangeBombSite(bool data)
    {
        if (Input.GetKeyDown(KeyCode.Z) == false && Input.GetKeyDown(KeyCode.Y) == false && CurrentObj != null)
        {
            UndoData undoData = new UndoData(CurrentObj, CurrentObj.transform.position, CurrentObj.transform.eulerAngles, CurrentObj.transform.localScale, CurrentObj.transform.position, CurrentObj.transform.eulerAngles, CurrentObj.transform.localScale, CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, data, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().pixelArt, CurrentObj.GetComponent<Tile>().pixelArt);
            Undo.NewCommand(undoData);
            UnsavedChanges.Instance.Unsave();
            CurrentObj.GetComponent<Tile>().bombsite = data;
        }
    }
    public void ChangeTSpawn(bool data)
    {
        if (Input.GetKeyDown(KeyCode.Z) == false && Input.GetKeyDown(KeyCode.Y) == false && CurrentObj != null)
        {
            UndoData undoData = new UndoData(CurrentObj, CurrentObj.transform.position, CurrentObj.transform.eulerAngles, CurrentObj.transform.localScale, CurrentObj.transform.position, CurrentObj.transform.eulerAngles, CurrentObj.transform.localScale, CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().bombsite, data, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().pixelArt, CurrentObj.GetComponent<Tile>().pixelArt);
            Undo.NewCommand(undoData);
            UnsavedChanges.Instance.Unsave();
            CurrentObj.GetComponent<Tile>().tSpawn = data;
        }
    }
    public void ChangeCtSpawn(bool data)
    {
        if (Input.GetKeyDown(KeyCode.Z) == false && Input.GetKeyDown(KeyCode.Y) == false && CurrentObj != null)
        {
            UndoData undoData = new UndoData(CurrentObj, CurrentObj.transform.position, CurrentObj.transform.eulerAngles, CurrentObj.transform.localScale, CurrentObj.transform.position, CurrentObj.transform.eulerAngles, CurrentObj.transform.localScale, CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, data, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().pixelArt, CurrentObj.GetComponent<Tile>().pixelArt);
            Undo.NewCommand(undoData);
            UnsavedChanges.Instance.Unsave();
            CurrentObj.GetComponent<Tile>().ctSpawn = data;
        }
    }
    public void ChangeBoxCollider(bool data)
    {
        if (Input.GetKeyDown(KeyCode.Z) == false && Input.GetKeyDown(KeyCode.Y) == false && CurrentObj != null)
        {
            UndoData undoData = new UndoData(CurrentObj, CurrentObj.transform.position, CurrentObj.transform.eulerAngles, CurrentObj.transform.localScale, CurrentObj.transform.position, CurrentObj.transform.eulerAngles, CurrentObj.transform.localScale, CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, !data, CurrentObj.GetComponent<Tile>().pixelArt, CurrentObj.GetComponent<Tile>().pixelArt);
            Undo.NewCommand(undoData);
            UnsavedChanges.Instance.Unsave();
            CurrentObj.GetComponent<BoxCollider2D>().isTrigger = !data;
            if (GameObject.Find("SeeColls").GetComponent<MapCreatorButtons>().Hidden == false)
            {
                CurrentObj.GetComponent<Tile>().greenExtents.gameObject.SetActive(data);
            }
        }
    }

    public void ChangePixelArt(bool data)
    {
        if (Input.GetKeyDown(KeyCode.Z) == false && Input.GetKeyDown(KeyCode.Y) == false && CurrentObj != null)
        {
            UndoData undoData = new UndoData(CurrentObj, CurrentObj.transform.position, CurrentObj.transform.eulerAngles, CurrentObj.transform.localScale, CurrentObj.transform.position, CurrentObj.transform.eulerAngles, CurrentObj.transform.localScale, CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().bombsite, CurrentObj.GetComponent<Tile>().tSpawn, CurrentObj.GetComponent<Tile>().ctSpawn, CurrentObj.GetComponent<BoxCollider2D>().enabled, CurrentObj.GetComponent<Tile>().pixelArt, data);
            Undo.NewCommand(undoData);
            UnsavedChanges.Instance.Unsave();
            if (data)
            {
                CurrentObj.GetComponent<SpriteRenderer>().sprite.texture.filterMode = FilterMode.Point;
            }
            else
            {
                CurrentObj.GetComponent<SpriteRenderer>().sprite.texture.filterMode = FilterMode.Bilinear;
            }
            CurrentObj.GetComponent<Tile>().pixelArt = data;
        }
    }
    // Update is called once per frame
}
