using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TeamSelect : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] int team;
    [SerializeField] GameObject teamSelect;


    public void OnPointerClick(PointerEventData eventData)
    {
        GameClient.Instance.ClientSetTeam(team);

        GameClient.Instance.selectedTeam = true;

        teamSelect.SetActive(false);
    }
    void Awake()
    {
        if (GameClient.Instance.selectedTeam)
            teamSelect.SetActive(false);

        GameClient.Instance.teamSelect = teamSelect;
    }
}
