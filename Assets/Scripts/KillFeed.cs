using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillFeed : MonoBehaviour
{
    [SerializeField] GameObject killFeed;
    [SerializeField] GameObject listObj;
    [SerializeField] Image[] deathTypes;

    public void AddKillFeed(string killerName, string deadName, int type, string killerTeam, string deadTeam)
    {
        GameObject newKillFeed = Instantiate(killFeed);
        newKillFeed.transform.parent = listObj.transform;
        newKillFeed.SetActive(true);

        foreach (Transform child in newKillFeed.transform)
        {
            Text childText = child.GetComponent<Text>();

            switch (child.name)
            {
                case "Killer":
                    {
                        childText.text = killerName;
                        if (killerTeam == "0")
                            childText.color = new Color32(0, 255, 255, 255);
                        else childText.color = new Color32(255, 0, 0, 255);

                        break;
                    }
                case "Dead":
                    {
                        childText.text = deadName;
                        if (deadTeam == "0")
                            childText.color = new Color32(0, 255, 255, 255);
                        else childText.color = new Color32(255, 0, 0, 255);

                        break;
                    }
                case "Weapon":
                    {
                        RectTransform rect = child.GetComponent<RectTransform>();

                        child.GetComponent<Image>().sprite = deathTypes[type].sprite;
                        rect.sizeDelta = new Vector2(rect.sizeDelta.x, deathTypes[type].sprite.rect.height);

                        break;
                    }
            }
        }

        Destroy(newKillFeed, 5f);
    }
}
