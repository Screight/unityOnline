using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct User
{
    string m_name;
    int m_raceID;

    public User(string p_name, int p_raceID)
    {
        m_name = p_name;
        m_raceID = p_raceID;
    }

    public string Name { get { return m_name; } set { m_name = value; } }
    public int RaceID { get { return m_raceID; } set { m_raceID = value; } }

}

[Serializable]
public struct Race
{
    int m_id;
    [SerializeField] float m_speed;
    [SerializeField] float m_maxHealth;
    [SerializeField] float m_damage;
    [SerializeField] float m_jumpForce;
    [SerializeField] float m_fireRate;
    [SerializeField] string m_name;

    public Race(bool p_initialize = false)
    {
        m_id = -1;
        m_speed = 0;
        m_maxHealth = 0;
        m_damage = 0;
        m_jumpForce = 0;
        m_fireRate = 0;
        m_name = "";
    }

    public float Speed { get { return m_speed; } set { m_speed = value; } }
    public float MaxHealth { get { return m_maxHealth; } set { m_maxHealth = value; } }
    public float Damage { get { return m_damage; } set { m_damage = value; } }
    public float JumpForce { get { return m_jumpForce; } set { m_jumpForce = value; } }
    public float FireRate { get { return m_fireRate; } set { m_fireRate = value; } }
    public string Name { get { return m_name; } set { m_name = value; } }
    public int ID { get { return m_id; } set { m_id = value; } }

}
