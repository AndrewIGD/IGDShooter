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

    public void InterpretText(string text)
    {
        string[] lines = text.Split('\n');
        foreach (string line in lines)
        {
            string[] parameters = line.Split(' ');

            if (parameters[0].Contains("Victory"))
            {
                winScreen.SetActive(true);
                Freeze();
            }
            else if (parameters[0].Contains("Defeat"))
            {
                loseScreen.SetActive(true);
                Freeze();
            }
            else if (parameters[0].Contains("Draw"))
            {
                drawScreen.SetActive(true);
                Freeze();
            }
            else
                switch (parameters[0])
                {
                    case "CT":
                        if (GameClient.Instance.Team == 0)
                        {
                            playerImages[int.Parse(parameters[1], CultureInfo.InvariantCulture)].sprite = ctSprite;
                        }
                        break;
                    case "T":
                        if (GameClient.Instance.Team == 1)
                        {
                            playerImages[int.Parse(parameters[1], CultureInfo.InvariantCulture)].sprite = tSprite;                          
                        }
                        break;
                }

            players[int.Parse(parameters[1], CultureInfo.InvariantCulture)].SetActive(true);
            playerNames[int.Parse(parameters[1], CultureInfo.InvariantCulture)].text = parameters[2];
            playerText[int.Parse(parameters[1], CultureInfo.InvariantCulture)].text = "Kills: " + parameters[3] + "\nDeaths: " + parameters[4];
            playerAwards[int.Parse(parameters[1], CultureInfo.InvariantCulture)].text = string.Join(" ", line.Split(' ').Skip(5)).Split('$')[0];
            playerAwardsDescription[int.Parse(parameters[1], CultureInfo.InvariantCulture)].text = line.Split('$')[1];
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
