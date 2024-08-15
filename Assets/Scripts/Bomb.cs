using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class Bomb : MonoBehaviour
{
    #region Private Variables

    private float timer;
    private float tickRate = 2f;

    private bool _isPrefab;

    #endregion

    #region Private Methods

    private void Start()
    {
        if (GameClient.IsRed)
        {
            //Activate megamap bomb vision

            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        _isPrefab = transform.name.Contains("Clone") == false;

        if (GameHost.Instance != null && _isPrefab == false)
        {
            StartCoroutine(Timer());
        }
    }

    private void Update()
    {
        if (GameHost.Instance != null && _isPrefab == false)
        {
            timer += Time.deltaTime;
            if (timer >= tickRate)
            {
                timer = 0;

                Network.Instance.Send(new Play(18, transform.position));
            }
        }
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(10f);

        tickRate = 1.5f;

        yield return new WaitForSeconds(10f);

        tickRate = 1f;

        yield return new WaitForSeconds(10f);

        tickRate = 0.5f;

        yield return new WaitForSeconds(5f);

        tickRate = 0.25f;

        yield return new WaitForSeconds(2.5f);

        tickRate = 0.125f;
    }

    #endregion
}
