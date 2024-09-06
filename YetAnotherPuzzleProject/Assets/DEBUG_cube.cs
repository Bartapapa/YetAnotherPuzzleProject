using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG_cube : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Sc_GameManager.instance != null)
        {
            int playerCount = Sc_GameManager.instance.PlayerManager.CurrentPlayers.Count;
            transform.position = transform.position + ((Vector3.up * playerCount) * Time.deltaTime);
        }

    }
}
