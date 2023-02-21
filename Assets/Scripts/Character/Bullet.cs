using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    private float speed = 10f;
    float m_damage = 5;
    Vector3 m_direction;

    private PhotonView pv;
    Vector3 m_bulletPos = Vector3.zero;

    public void Initialize(bool p_isFacingRight)
    {
        if(p_isFacingRight)
            m_direction = Vector3.right;
        else m_direction = Vector3.left;
    }

    private void Awake()
    {
        pv = GetComponent<PhotonView>();

        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 20;
    }

    private void Update()
    {
        if (pv.IsMine) { transform.position += m_direction * Time.deltaTime * speed; }
        else {
            transform.position = Vector3.Lerp(transform.position, m_bulletPos, Time.deltaTime * 20);
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) { stream.SendNext(transform.position); }
        else if (stream.IsReading) {
            m_bulletPos = (Vector3)stream.ReceiveNext();
        }
    }

    private void OnBecameInvisible()
    {
        pv.RPC("NetworkDestroy", RpcTarget.All);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision");
        Character character = collision.GetComponent<Character>();

        if (character == null) { return; }

        Debug.Log("Has colisionado con: " + collision);

        character.Damage(m_damage);

        pv.RPC("NetworkDestroy", RpcTarget.All);
    }

    [PunRPC]
    public void NetworkDestroy() 
    {
        Destroy(this.gameObject);
    }
}
