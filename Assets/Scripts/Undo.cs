using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Undo : MonoBehaviour
{
    public static Undo mainUndoManager;

    private static List<UndoCommand> commands;

    private static int undoIndex = 0;

    public static void UndoCommand()
    {
        if (commands.Count != 0)
        {
            UnsavedChanges.Instance.Unsave();

            commands[undoIndex--].Undo();
            if (undoIndex < 0)
                undoIndex = 0;
        }
    }

    public static void RedoCommand()
    {
        if (commands.Count - 1 != undoIndex)
        {
            UnsavedChanges.Instance.Unsave();

            commands[++undoIndex].Redo();
        }
    }

    public static void NewCommand(UndoCommand command, [CallerMemberName] string memberName = "")
    {
        for(int i = commands.Count - 1; i > undoIndex; i--)
        {
            commands.RemoveAt(i);
        }

        commands.Add(command);

        undoIndex = commands.Count - 1;
    }

    private void Start()
    {
        commands = new List<UndoCommand>();

        UndoCommand com = new UndoCommand();
        commands.Add(com);
    }
}

public class UndoCommand : MonoBehaviour
{
    public virtual void Undo()
    {

    }

    public virtual void Redo()
    {

    }
}

public class UndoMove : UndoCommand
{
    public UndoMove(GameObject obj, Vector3 prevPos, Vector3 currentPos)
    {
        targetObj = obj;
        this.prevPos = prevPos;
        this.currentPos = currentPos;
    }

    GameObject targetObj;

    Vector3 prevPos;
    Vector3 currentPos;

    public override void Undo()
    {
        targetObj.transform.position = prevPos;
    }

    public override void Redo()
    {
        targetObj.transform.position = currentPos;
    }
}

public class UndoResize : UndoCommand
{
    public UndoResize(GameObject obj, Vector3 prevPos, Vector3 currentPos, Vector3 prevSize, Vector3 currentSize)
    {
        targetObj = obj;
        this.prevPos = prevPos;
        this.currentPos = currentPos;
        this.prevSize = prevSize;
        this.currentSize = currentSize;
    }

    GameObject targetObj;

    Vector3 prevPos;
    Vector3 currentPos;

    Vector3 prevSize;
    Vector3 currentSize;

    public override void Undo()
    {
        targetObj.transform.position = prevPos;
        targetObj.transform.localScale = prevSize;
    }

    public override void Redo()
    {
        targetObj.transform.position = currentPos;
        targetObj.transform.localScale = currentSize;
    }
}

public class UndoDelete : UndoCommand
{
    public UndoDelete(GameObject obj)
    {
        targetObj = obj;
    }

    GameObject targetObj;

    public override void Undo()
    {
        targetObj.SetActive(true);
    }

    public override void Redo()
    {
        targetObj.SetActive(false);
    }
}

public class UndoAddObj : UndoCommand
{
    public UndoAddObj(GameObject obj)
    {
        targetObj = obj;
    }

    GameObject targetObj;

    public override void Undo()
    {
        targetObj.SetActive(false);
    }

    public override void Redo()
    {
        targetObj.SetActive(true);
    }
}

public class UndoSetZ : UndoCommand
{
    public UndoSetZ(GameObject obj, float prevZ, float currentZ)
    {
        targetObj = obj;
        this.prevZ = prevZ;
        this.currentZ = currentZ;
    }

    GameObject targetObj;

    float prevZ;
    float currentZ;

    public override void Undo()
    {
        targetObj.transform.position = new Vector3(targetObj.transform.position.x, targetObj.transform.position.y, prevZ);
    }

    public override void Redo()
    {
        targetObj.transform.position = new Vector3(targetObj.transform.position.x, targetObj.transform.position.y, currentZ);
    }
}

public class UndoData : UndoCommand
{
    public UndoData(GameObject obj, Vector3 prevPos, Vector3 prevRot, Vector3 prevSize, Vector3 currentPos, Vector3 currentRot, Vector3 currentSize, bool prevBomb, bool prevT, bool prevCt, bool prevBC, bool currentBomb, bool currentT, bool currentCt, bool currentBC, bool prevPixelArt, bool currentPixelArt)
    {
        targetObj = obj;
        this.prevPos = prevPos;
        this.prevRot = prevRot;
        this.prevSize = prevSize;
        this.prevBomb = prevBomb;
        this.prevT = prevT;
        this.prevCt = prevCt;
        this.prevBC = prevBC;
        this.currentPos = currentPos;
        this.currentRot = currentRot;
        this.currentSize = currentSize;
        this.currentBomb = currentBomb;
        this.currentT = currentT;
        this.currentCt = currentCt;
        this.currentBC = currentBC;
        this.prevPixelArt = prevPixelArt;
        this.currentPixelArt = currentPixelArt;
    }

    GameObject targetObj;

    Vector3 prevPos;
    Vector3 currentPos;
    Vector3 prevRot;
    Vector3 currentRot;
    Vector3 prevSize;
    Vector3 currentSize;
    bool prevBomb;
    bool currentBomb;
    bool prevT;
    bool currentT;
    bool prevCt;
    bool currentCt;
    bool prevBC;
    bool currentBC;
    bool prevPixelArt;
    bool currentPixelArt;

    public override void Undo()
    {
        targetObj.transform.position = prevPos;
        targetObj.transform.localEulerAngles = prevRot;
        targetObj.transform.localScale = prevSize;

        Tile tile = targetObj.GetComponent<Tile>();

        tile.bombsite = prevBomb;
        tile.tSpawn = prevT;
        tile.ctSpawn = prevCt;
        if(prevBC != currentBC)
            targetObj.GetComponent<BoxCollider2D>().isTrigger = !targetObj.GetComponent<BoxCollider2D>().isTrigger;
        tile.pixelArt = prevPixelArt;
        if(prevPixelArt != currentPixelArt)
        {
            if(prevPixelArt)
            {
                targetObj.GetComponent<SpriteRenderer>().sprite.texture.filterMode = FilterMode.Point;
            }
            else
            {
                targetObj.GetComponent<SpriteRenderer>().sprite.texture.filterMode = FilterMode.Bilinear;
            }
        }

        if (targetObj.transform.position.z <= 10000)
            targetObj.GetComponent<SpriteRenderer>().sortingLayerName = "AbovePlayer";
        else targetObj.GetComponent<SpriteRenderer>().sortingLayerName = "Default";

        TileData.Instance.UpdateData();
    }

    public override void Redo()
    {
        targetObj.transform.position = currentPos;
        targetObj.transform.localEulerAngles = currentRot;
        targetObj.transform.localScale = currentSize;

        Tile tile = targetObj.GetComponent<Tile>();

        tile.bombsite = currentBomb;
        tile.tSpawn = currentT;
        tile.ctSpawn = currentCt;
        if (prevBC != currentBC)
            targetObj.GetComponent<BoxCollider2D>().isTrigger = !targetObj.GetComponent<BoxCollider2D>().isTrigger;
        tile.pixelArt = currentPixelArt;
        if (prevPixelArt != currentPixelArt)
        {
            if (currentPixelArt)
            {
                targetObj.GetComponent<SpriteRenderer>().sprite.texture.filterMode = FilterMode.Point;
            }
            else
            {
                targetObj.GetComponent<SpriteRenderer>().sprite.texture.filterMode = FilterMode.Bilinear;
            }
        }

        if (targetObj.transform.position.z <= 10000)
            targetObj.GetComponent<SpriteRenderer>().sortingLayerName = "AbovePlayer";
        else targetObj.GetComponent<SpriteRenderer>().sortingLayerName = "Default";

        TileData.Instance.UpdateData();
    }
}



