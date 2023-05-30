using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviourPun, IPunObservable
{
    [SerializeField] Race m_race;
    float m_health;
    int m_numberOfLives;

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

    SpriteRenderer m_renderer;

    Animator m_animator;

    HealthModel m_healthModel;
    Vector3 m_initialPosition;

    int m_playerIndex = -1;
    public int PlayerIndex { get { return m_playerIndex; } set { m_playerIndex = value; } }

    float m_currentTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pv = GetComponent<PhotonView>();

        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 20;

        m_model = new PlayerModel(gameObject);
        m_animator = GetComponent<Animator>();
        m_renderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        isFacingRight = true;
        m_numberOfLives = 3;

        if (pv.IsMine)
        {
            m_model.SetName(Network_Manager._NETWORK_MANAGER.User.Value.Name);
            m_race = DataManager.Instance.GetRace(Network_Manager._NETWORK_MANAGER.User.Value.RaceID).Value;
            m_health = m_race.MaxHealth;

            pv.RPC("SetEnemyPlayerName", RpcTarget.All, Network_Manager._NETWORK_MANAGER.User.Value.Name);
            pv.RPC("SetEnemyPlayerRace", RpcTarget.All, Network_Manager._NETWORK_MANAGER.User.Value.RaceID);
            pv.RPC("SetPlayerIndex", RpcTarget.All, m_playerIndex);
        }
    }

    private void Update()
    {
        if (pv.IsMine) {
            CheckInputs();

            m_currentTime += Time.deltaTime;
        }
        else { SmoothReplicate(); }
        m_animator.SetBool("isJumping", !IsGrounded());
    }

    [PunRPC]
    private void SetPlayerIndex(int value)
    {
        m_playerIndex = value;
        m_initialPosition = Game_Manager.Instance.GetSpawnTr(m_playerIndex).position;
        m_healthModel = Game_Manager.Instance.GetHealthModel(m_playerIndex);
    }

    [PunRPC]
    private void SetEnemyPlayerName(string value)
    {
        if (pv.IsMine) { return; }
        m_model.SetName(value);
        Debug.Log("Enemy: " + value);
    }

    [PunRPC]
    private void SetEnemyPlayerRace(int p_raceIndex)
    {
        if (pv.IsMine) { return; }
        m_race = DataManager.Instance.GetRace(p_raceIndex).Value;
        m_health = m_race.MaxHealth;
    }

    [PunRPC]
    private void SetAnimationRunningBool(bool p_isRunning)
    {
        if (pv.IsMine) { return; }
        m_animator.SetBool("isRunning", p_isRunning);
    }

    private void CheckInputs() 
    {
        desiredMovementAxis = Input.GetAxisRaw("Horizontal");

        m_animator.SetBool("isRunning", Mathf.Abs(desiredMovementAxis) > 0);
        pv.RPC("SetAnimationRunningBool", RpcTarget.All, Mathf.Abs(desiredMovementAxis) > 0);

        //desiredMovementAxis *= m_race.Speed * Time.deltaTime;

        if (Mathf.Abs(desiredMovementAxis) > 0) {
            float direction;
            if(desiredMovementAxis > 0) { direction = 1; }
            else { direction = -1; }
            rb.velocity = new Vector2(direction * m_race.Speed, rb.velocity.y);
        }
        else { rb.velocity = new Vector2(0, rb.velocity.y); }

        FlipPlayer();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, m_race.JumpForce);
                canDoubleJump = true;
            }
            else if (canDoubleJump) 
            {
                rb.velocity = new Vector2(rb.velocity.x, m_race.JumpForce);
                canDoubleJump = false;
            }
        }

        //Codigo disparo
        if (Input.GetKeyDown(KeyCode.E))
        {
            float period = 1 / (float)m_race.FireRate;
            if (m_currentTime > period)
            {
                Shoot();
                m_currentTime = 0;
            } 
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
            m_numberOfLives--;
            m_healthModel.SetHealth(m_numberOfLives);
            if (m_numberOfLives <= 0) { Destroy(this.gameObject); }
            else
            {
                m_health = m_race.MaxHealth;
                m_model.SetHealth(1);

                Bullet[] bulletArray = FindObjectsOfType<Bullet>();
                foreach (Bullet bullet in bulletArray) { Destroy(bullet.gameObject); }
            }
        }
        pv.RPC("NetworkDamage", RpcTarget.Others, p_value);
    }

    [PunRPC]
    private void NetworkDamage(float p_value) 
    {
        m_health -= p_value;
        m_model.SetHealth(m_health / m_race.MaxHealth);

        if (m_health <= 0)
        {
            m_numberOfLives--;
            m_healthModel.SetHealth(m_numberOfLives);
            if (m_numberOfLives <= 0) { Destroy(this.gameObject); }
            else
            {
                m_health = m_race.MaxHealth;
                m_model.SetHealth(1);
                transform.position = m_initialPosition;

                Bullet[] bulletArray = FindObjectsOfType<Bullet>();

                foreach(Bullet bullet in bulletArray) { Destroy(bullet.gameObject); }
            }
        }
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

            m_renderer.flipX = !isFacingRight;
            spawnBullet.localPosition *= -1;
        }
    }

    public void SetHealthMode(HealthModel p_model) { m_healthModel = p_model; }
    public void SetInitialPosition(Vector3 p_pos) { m_initialPosition = p_pos; }

}
