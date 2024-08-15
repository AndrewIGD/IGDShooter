using UnityEngine;

public class AddMapButton : MonoBehaviour
{
    public GameObject mapName;

    public void ToggleModal()
    {
        mapName.SetActive(true);   
    }
}
