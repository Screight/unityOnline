using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
public struct HealthModel
{
    [SerializeField] GameObject[] m_healthGOArray;

    public void SetHealth(int p_value)
    {
        for(int i = 0; i < m_healthGOArray.Length; i++)
        {
            m_healthGOArray[i].SetActive(i < p_value);
        }
    }

}

public class Game_Manager : Singleton<Game_Manager>
{
    [SerializeField] Transform[] m_spawnTrArray;
    [SerializeField] HealthModel[] m_healthModelArray;

    protected override void Awake()
    {
        base.Awake();
        if (PhotonNetwork.IsMasterClient)
        {
            Character character = PhotonNetwork.Instantiate("Player_1", m_spawnTrArray[0].position, Quaternion.identity).GetComponent<Character>();
            character.PlayerIndex = 0;
        }
        else 
        {
            Character character = PhotonNetwork.Instantiate("Player_2", m_spawnTrArray[1].position, Quaternion.identity).GetComponent<Character>();
            character.PlayerIndex = 1;
        }
    }

    public Transform GetSpawnTr(int p_index) { return m_spawnTrArray[p_index]; }
    public HealthModel GetHealthModel(int p_index) { return m_healthModelArray[p_index]; }

}
