using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerModel
{
    TMPro.TextMeshProUGUI m_nameTMP;
    Image m_healthIcon;

    public PlayerModel(GameObject p_parent)
    {
        Transform canvasTr = p_parent.transform.Find("Canvas");

        m_nameTMP = canvasTr.Find("NameTMP").GetComponent<TMPro.TextMeshProUGUI>();
        m_healthIcon = canvasTr.Find("HealthIMG").GetComponent<Image>();
    }

    public void SetName(string p_value) { m_nameTMP.text = p_value; }
    public void SetHealth(float p_value) { m_healthIcon.fillAmount = p_value; }

}
