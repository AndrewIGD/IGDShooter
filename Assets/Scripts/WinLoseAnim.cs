using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.Linq;

public class WinLoseAnim : MonoBehaviour
{
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject loseScreen;
    [SerializeField] GameObject drawScreen;
    [SerializeField] GameObject[] players;
    [SerializeField] Image[] playerImages;
    [SerializeField] Text[] playerNames;
    [SerializeField] Text[] playerText;
    [SerializeField] Text[] playerAwards;
    [SerializeField] Text[] playerAwardsDescription;
    [SerializeField] Sprite ctSprite;
    [SerializeField] Sprite tSprite;
    [SerializeField] Text countingText;

    float t = 19;
    bool counting = false;

    public void StartCounting()
    {
        counting = true;
    }

    private void Update()
    {
        if (counting)
        {
            t -= Time.deltaTime;
            countingText.text = "Match Closing in " + (int)t + "...";
            if (t <= 0)
            {
                Escape.Instance.Terminate();
            }
        }
    }

    public void InterpretText(CT ct)
    {
        playerImages[ct.Id].sprite = ctSprite;

        players[ct.Id].SetActive(true);
        playerNames[ct.Id].text = ct.Name;
        playerText[ct.Id].text = "Kills: " + ct.Kills + "\nDeaths: " + ct.Deaths;
        playerAwards[ct.Id].text = ct.Title;
        playerAwardsDescription[ct.Id].text = ct.Description;
    }

    public void InterpretText(T t)
    {
        playerImages[t.Id].sprite = tSprite;

        players[t.Id].SetActive(true);
        playerNames[t.Id].text = t.Name;
        playerText[t.Id].text = "Kills: " + t.Kills + "\nDeaths: " + t.Deaths;
        playerAwards[t.Id].text = t.Title;
        playerAwardsDescription[t.Id].text = t.Description;
    }

    public void InterpretText(string text)
    {
        if (text.Contains("Victory"))
        {
            winScreen.SetActive(true);
            Freeze();
        }
        else if (text.Contains("Defeat"))
        {
            loseScreen.SetActive(true);
            Freeze();
        }
        else if (text.Contains("Draw"))
        {
            drawScreen.SetActive(true);
            Freeze();
        }
    }

    private void Freeze()
    {
        GetComponent<Animator>().Play("fadeIn");
        foreach (Player player in GameClient.Instance.AlivePlayers)
        {
            player.Freeze();
        }
    }
}
