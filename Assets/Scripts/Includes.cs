using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct Race
{
    [SerializeField] float m_speed;
    [SerializeField] float m_maxHealth;
    [SerializeField] float m_damage;
    [SerializeField] float m_jumpForce;
    [SerializeField] float m_fireRate;

    public Race(bool p_initialize = false)
    {
        m_speed = 0;
        m_maxHealth = 0;
        m_damage = 0;
        m_jumpForce = 0;
        m_fireRate = 0;
    }

    public float Speed { get { return m_speed; } set { m_speed = value; } }
    public float MaxHealth { get { return m_maxHealth; } set { m_maxHealth = value; } }
    public float Damage { get { return m_damage; } set { m_damage = value; } }
    public float JumpForce { get { return m_jumpForce; } set { m_jumpForce = value; } }
    public float FireRate { get { return m_fireRate; } set { m_fireRate = value; } }

}
