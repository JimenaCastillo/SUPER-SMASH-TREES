using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class BanditPlayer1 : MonoBehaviour, IPlayer
{

    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] private int playerIndex;

    public float m_reboundForce = 15.0f;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_Bandit m_groundSensor;
    private bool m_grounded = false;
    private bool m_combatIdle = false;
    private bool HitReceived;

    // Use this for initialization
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();
    }

    // Update is called once per frame
    void Update()
    {
        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = 0f;

        // Swap direction of sprite depending on walk direction
        if (Input.GetKey(KeyCode.A))
            inputX = -1f;
        else if (Input.GetKey(KeyCode.D))
            inputX = 1f;

        if (inputX > 0)
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (inputX < 0)
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        // ? No se mueve si est� golpeado
        if (!HitReceived)
        {
            m_body2d.linearVelocity = new Vector2(inputX * m_speed, m_body2d.linearVelocity.y);
        }

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeed", m_body2d.linearVelocity.y);

        // -- Handle Animations --
        //Hurt
        m_animator.SetBool("HitReceived", HitReceived);

        //Attack
        if (Input.GetKeyDown("s"))
        {
            m_animator.SetTrigger("Attack");
        }

        //Change between idle and combat idle
        else if (Input.GetKeyDown("f"))
            m_combatIdle = !m_combatIdle;

        //Jump
        else if (Input.GetKeyDown("w") && m_grounded)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.linearVelocity = new Vector2(m_body2d.linearVelocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
            m_animator.SetInteger("AnimState", 2);

        //Combat Idle
        else if (m_combatIdle)
            m_animator.SetInteger("AnimState", 1);

        //Idle
        else
            m_animator.SetInteger("AnimState", 0);
    }

    public void Damage1(Vector2 direction, int damage)
    {
        if (!HitReceived)
        {
            HitReceived = true;

            // ? Rebote horizontal dependiendo del lado del atacante
            float xDir = transform.position.x < direction.x ? -1f : 1f;
            Vector2 rebound = new Vector2(xDir, 0f);
            m_body2d.AddForce(rebound * m_reboundForce, ForceMode2D.Impulse);
        }
    }

    public void DeactivateDamage1()
    {
        HitReceived = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sword"))
        {
            // Intentamos obtener el componente BanditPlayer2 desde el objeto padre de la espada
            BanditPlayer2 banditPlayer = collision.gameObject.GetComponentInParent<BanditPlayer2>();

            if (banditPlayer != null)
            {
                // Llamamos a Damage1 para que BanditPlayer1 reciba el daño
                Vector2 directionDamage = new Vector2(collision.gameObject.transform.position.x, 0);
                Damage1(directionDamage, 1);
            }
            else
            {
                Debug.LogWarning("El objeto con etiqueta 'Sword' no tiene el componente BanditPlayer2.");
            }
        }
        else if (collision.CompareTag("Token"))
        {
            int tokenValue = collision.GetComponent<Token>().value;
            OnTokenCollected(tokenValue);  // Llamas al método de la interfaz
            Destroy(collision.gameObject);
        }
    }

    public void OnTokenCollected(int tokenValue)
    {
        // Aquí defines qué hacer cuando el jugador recolecta un token
        // Por ejemplo, insertar el valor en el árbol:

        int playerIndex = GetPlayerIndex();  // o un índice fijo si es un solo jugador
        ChallengeManager.Instance.CollectToken(playerIndex, tokenValue);
    }


    public int GetPlayerIndex()
    {
        return 0;
    }

    public Vector2 GetFacingDirection()
    {
        return transform.localScale.x > 0 ? Vector2.left : Vector2.right;
    }

    public void AddForce(Vector2 force)
    {
        m_body2d.AddForce(force, ForceMode2D.Impulse);
    }

}