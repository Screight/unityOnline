using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateImage : MonoBehaviour
{
    [SerializeField] float m_angularSpeed;
    [SerializeField] 
    private void Update()
    {
        transform.RotateAround(transform.position, new Vector3(0, 0, 1), Time.deltaTime * m_angularSpeed);
    }
}
