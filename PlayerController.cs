using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
public class PlayerController : MonoBehaviour
{
    #region [CONSTS&STATIC]
    public static float globalTime;
    public static int globalTimeInt;
    public static int globalTimeIntForSkills;
    public static Transform playerTrans;
    public static PlayerController instance {get; private set; }
    #endregion

    #region [INSPECTOR]
    [SerializeField] private float MOVEMENT_BASE_SPEED, CROSSHAIR_DISTANSE, movementSpeed, offset;
    [SerializeField] private Vector2 movementDirection, movementAndroid, movementPC, aim;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject plob;
    [SerializeField] private Transform crosshair, playerMark;
    [SerializeField] private Image dush;
    [SerializeField] private Joystick joystick;
    #endregion

    private Dictionary<Type, IPlayerBehaviour> behavioursMap;
    private IPlayerBehaviour behaviourCurrent;
    private Camera cachedCamera;
    private Rigidbody2D rb;
    private bool act = true;

    #region [MONO]
    private void Awake()
    {
        instance = this;    
        playerTrans = this.transform;    
        cachedCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        if (InterfaceMainGame.loadkey)
        {
            cachedCamera.transform.position = playerTrans.position + new Vector3(0, 0, -1);
        }
        InitBehaviour();
        SetBehaviourIdle();
    }

    private void InitBehaviour(){
        behavioursMap = new Dictionary<Type, IPlayerBehaviour>();

        behavioursMap[typeof(PlayerBehaviourIdle)] = new PlayerBehaviourIdle();
        behavioursMap[typeof(PlayerBehaviourDialoge)] = new PlayerBehaviourDialoge();
        behavioursMap[typeof(PlayerBehaviorSit)] = new PlayerBehaviorSit();
    }

    private void SetBehaviour(IPlayerBehaviour newBehaviour){
        if( behaviourCurrent != null){
            behaviourCurrent.Exit();
        }
        behaviourCurrent = newBehaviour;
        behaviourCurrent.Enter();
    }

    private IPlayerBehaviour GetBehaviour<T>() where T :IPlayerBehaviour{
        return behavioursMap[typeof(T)];
    }

    private void Update()
    {
        if( behaviourCurrent != null){
            behaviourCurrent.Update();
        }
        GlobalTimeUpdate();
    }

    public void SetBehaviourIdle(){
        SetBehaviour(GetBehaviour<PlayerBehaviourIdle>());
    }

    public void SetBehaviourDialog(){
        SetBehaviour(GetBehaviour<PlayerBehaviourDialoge>());
    }

    private void GlobalTimeUpdate(){
        globalTime += Time.deltaTime;
        globalTimeInt = (int)globalTime;
        globalTimeIntForSkills = globalTimeInt * 4;
    }
    #endregion


    public void Move()
    {
        movementPC = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        movementAndroid = new Vector2(joystick.Horizontal, joystick.Vertical); 
        if(movementAndroid.magnitude < 0.5f){
            movementAndroid = Vector2.zero;
        }
        movementDirection = movementPC.magnitude >= movementAndroid.magnitude ? movementPC : movementAndroid;
        movementSpeed = Mathf.Clamp(movementDirection.magnitude, 0.0f, 1.0f);
        movementDirection.Normalize();
        rb.velocity = movementDirection * movementSpeed * MOVEMENT_BASE_SPEED;
    }

    public void MoveMark()
    {
        playerMark.position = new Vector3(playerTrans.position.x, playerTrans.position.y, 99);
        if (movementSpeed > 0.5f)
        {
            playerMark.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg - 90);
        }
    }

    public void Animate()
    {
        if (movementDirection != Vector2.zero)
        {
            animator.SetFloat("Horizontal", movementDirection.x);
            animator.SetFloat("Vertical", movementDirection.y);
        }
        animator.SetFloat("Speed", movementSpeed);
    }

    public void Aim()
    {
        if (Input.GetMouseButton(1))
        {
            aim = cachedCamera.ScreenToWorldPoint(Input.mousePosition) - playerTrans.position;
            aim.Normalize();
            crosshair.localPosition = aim * CROSSHAIR_DISTANSE;
            crosshair.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg + offset);
            crosshair.gameObject.SetActive(true);
            if (Input.GetMouseButton(0))
            {
                if (act)
                {
                    StartCoroutine(In());
                }
            }
            else if (Input.GetKeyDown("space")) {
                useSkill();
            }
        }
        else
        {
            crosshair.gameObject.SetActive(false);
        }
        dush.fillAmount = (globalTimeIntForSkills - coolDown) / 100f;
    }

    private IEnumerator In()
    {
        act = false;
        GameObject plob1 = Instantiate(plob, crosshair.position, Quaternion.identity);
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

    private int coolDown = -100;
    private int maxCoolDown = 100;

    private void useSkill(){
        if(globalTimeIntForSkills - coolDown < maxCoolDown){
            return;
        }
        playerTrans.position += new Vector3(aim.x, aim.y, 0) * 100;
        coolDown = globalTimeIntForSkills;
    }
}
