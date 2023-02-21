using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviourPun, IPunObservable
{
    [SerializeField] Race m_race;
    float m_health;

    [SerializeField]  Vector3 boxSize;
    [SerializeField] float maxDistance;
    [SerializeField] LayerMask layerMask;
    bool canDoubleJump;
    [SerializeField]
    Transform spawnBullet;
    bool isFacingRight;

    Rigidbody2D rb;
    float desiredMovementAxis = 0f;

    PhotonView pv;
    Vector3 enemyPosition = Vector3.zero;

    PlayerModel m_model;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pv = GetComponent<PhotonView>();

        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 20;

        m_model = new PlayerModel(gameObject);
    }

    private void Start()
    {
        isFacingRight = true;
        m_health = m_race.MaxHealth;
    }

    private void Update()
    {
        if (pv.IsMine) { CheckInputs(); }
        else { SmoothReplicate(); }
    }

    private void CheckInputs() 
    {
        desiredMovementAxis = Input.GetAxis("Horizontal");
        desiredMovementAxis *= m_race.Speed * Time.deltaTime;

        transform.Translate(new Vector3(desiredMovementAxis, 0f, 0f));
        FlipPlayer();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsGrounded())
            {
                rb.velocity = Vector3.up * m_race.JumpForce;
                canDoubleJump = true;
            }
            else if (canDoubleJump) 
            {
                rb.velocity = Vector3.up * m_race.JumpForce;
                canDoubleJump = false;
            }
        }

        //Codigo disparo
        if (Input.GetKeyDown(KeyCode.E))
        {
            Shoot();
        }
    }

    private void SmoothReplicate() 
    {
        desiredMovementAxis = enemyPosition.x - transform.position.x;
        transform.position = Vector3.Lerp(transform.position, enemyPosition, Time.deltaTime * 20);
        FlipPlayer();
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting){ stream.SendNext(transform.position); }
        else if (stream.IsReading){ enemyPosition = (Vector3)stream.ReceiveNext(); }
    }

    private void Shoot() 
    {
        PhotonNetwork.Instantiate("Bullet", spawnBullet.position, transform.rotation).GetComponent<Bullet>().Initialize(isFacingRight);
    }

    public void Damage(float p_value) 
    {
        m_health -= p_value;
        m_model.SetHealth(m_health/m_race.MaxHealth);

        if (m_health <= 0)
        {
            Destroy(this.gameObject);
        }
        pv.RPC("NetworkDamage", RpcTarget.All);
    }

    [PunRPC]
    private void NetworkDamage() 
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position - transform.up * maxDistance, boxSize);
    }

    private bool IsGrounded() 
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, maxDistance, layerMask))
        { return true; }
        else { return false; }
    }

    void FlipPlayer()
    {
        if (!isFacingRight && desiredMovementAxis > 0 || isFacingRight && desiredMovementAxis < 0)
        {
            isFacingRight = !isFacingRight;

            Vector3 newScale = transform.localScale;
            newScale.x *= -1;
            transform.localScale = newScale;
        }
    }
}
