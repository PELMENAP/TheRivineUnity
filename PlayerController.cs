using UnityEngine;
using System.Collections;
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float MOVEMENT_BASE_SPEED, CROSSHAIR_DISTANSE, movementSpeed, offset;
    [Space]
    [Header("Информация о персонаже")]
    public Vector2 movementDirection;
    [Space]
    private Rigidbody2D rb;
    public Animator animator;
    public GameObject crosshair;
    public GameObject plob;
    public GameObject playerMark;

    public static int seed = 0;
    private bool act = true;
    Camera cachedCamera;
    Transform playerTrans;
    void Awake()
    {
        playerTrans = transform;
        cachedCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Moss")
        {
            EventSystem.current.MossTrigerEnter();
        }
    }

    private void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Moss")
        {
            EventSystem.current.MossTrigerExit();
        }
    }

    public static void GetSeed(int i)
    {
        seed = i;
    }
    private IEnumerator In()
    {
        act = false;
        print(seed);
        print(data.seed);
        print(data.x);
        print(data.y);
        // GameObject plob1 = Instantiate(plob, crosshair.transform.position, Quaternion.identity);
        // RaycastHit2D[] hits = Physics2D.RaycastAll(plob1.transform.position + new Vector3(0, 0, -1), transform.forward);
        // for (int i = 0; i < hits.Length; i++)
        // {
        //     GameObject obj = hits[i].collider.gameObject;
        //     if (obj.tag == "Chunk")
        //     {
        //         plob1.transform.SetParent(obj.transform);
        //     }
        // }
        yield return new WaitForSeconds(1);
        act = true;
    }

    void Update()
    {
        if (cachedCamera.enabled)
        {
            ProcessInputs();
            Move();
            MoveMark();
            Animate();
            Aim();
        }
    }

    void ProcessInputs()
    {
        movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        movementSpeed = Mathf.Clamp(movementDirection.magnitude, 0.0f, 1.0f);
        movementDirection.Normalize();
    }

    void Move()
    {
        rb.velocity = movementDirection * movementSpeed * MOVEMENT_BASE_SPEED;
    }

    private void MoveMark()
    {
        playerMark.transform.position = new Vector3(transform.position.x, transform.position.y, 99);
        if (movementSpeed > 0.5f)
        {
            playerMark.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg - 90);
        }
    }
    void Animate()
    {
        if (movementDirection != Vector2.zero)
        {
            animator.SetFloat("Horizontal", movementDirection.x);
            animator.SetFloat("Vertical", movementDirection.y);
        }
        animator.SetFloat("Speed", movementSpeed);
    }

    public void Save()
    {
        data.x = playerTrans.position.x;
        data.y = playerTrans.position.y;
        data.z = playerTrans.position.z;
        data.seed = seed;
        SaveLoad.Save(data);
    }
    public void Load()
    {
        SaveLoad.Load(ref data);
        seed = data.seed;
    }

    void Aim()
    {
        Vector2 aim = cachedCamera.ScreenToWorldPoint(Input.mousePosition) - playerTrans.position;
        if (Input.GetMouseButton(1))
        {
            aim.Normalize();
            crosshair.transform.localPosition = aim * CROSSHAIR_DISTANSE;
            crosshair.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg + offset);
            crosshair.SetActive(true);
            if (Input.GetMouseButton(0))
            {
                if (act)
                {
                    StartCoroutine(In());
                }
            }
        }
        else
        {
            crosshair.SetActive(false);
        }
    }

    [System.Serializable]
    public class Data
    {
        public float x, y, z;
        public int seed;
    }
    [SerializeField] public Data data;
}
