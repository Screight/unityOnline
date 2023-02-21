using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Game_Manager : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnPlyer1;
    [SerializeField]
    private GameObject spawnPlyer2;

    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Player", spawnPlyer1.transform.position, Quaternion.identity);
        }
        else 
        {
            PhotonNetwork.Instantiate("Player", spawnPlyer2.transform.position, Quaternion.identity);
        }
    }
}
