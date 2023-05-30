using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DataManager : Singleton<DataManager>
{
    Dictionary<int, Race> m_idToRace;

    protected override void Awake()
    {
        base.Awake();
        m_idToRace = new Dictionary<int, Race>();
        DontDestroyOnLoad(gameObject);
    }

    public void HandleRacesResponseFromServer(string p_data){
        string[] stringList = p_data.Split(";");

        if (stringList[1] == "0") {
            Debug.LogWarning("Error when getting races data from the Database.");
            return;
        }

        for(int i = 2; i < stringList.Length; i++)
        {
            string[] raceDataList = stringList[i].Split(":");
            Race race = new Race();
            race.ID = int.Parse(raceDataList[0]);
            race.Name = raceDataList[1];
            race.MaxHealth = float.Parse(raceDataList[2]);
            race.Damage = float.Parse(raceDataList[3]);
            race.Speed = float.Parse(raceDataList[4]);
            race.JumpForce = float.Parse(raceDataList[5]);
            race.FireRate = float.Parse(raceDataList[6]);

            AddRace(race);
        }
        Debug.Log("Races saved.");
    }

    void AddRace(Race p_race)
    {
        if (m_idToRace.ContainsKey(p_race.ID)) { return; }
        m_idToRace.Add(p_race.ID, p_race);
    }

    public Race? GetRace(int p_raceID)
    {
        if (m_idToRace.ContainsKey(p_raceID)) { return m_idToRace[p_raceID]; }
        else { return null; }
    }

    public List<Race> RaceList { get { return new List<Race>(m_idToRace.Values); } }

}
