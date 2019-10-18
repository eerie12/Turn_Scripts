using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerrbody : MonoBehaviour
{    
    //statusUIと連結(sp)の管理
    StatusController theStatusController;   
    HealthManager HM;
    //DialogueManagerの管理
    DialogueManager DM;
    //touchstick
    public VirtualJoystick joystick;

    enum MovementState
    {
        Move,
        Attack,
    }
    MovementState PlayerState;

    #region Playerの行動変数

    //動き
    [SerializeField] private float moveSpeed;
    private float startMoveSpeed;
    public CharacterController controller;
    public GameObject playerModel;
    private Vector3 moveDirection;
    public float gravityScale;    
    public float jumpForce;
    public float startScale;
    //start時のanimation
    public bool startAnim = false;
    public bool jump      = false;
    //tired
    public bool tiredStart = false;
    //回転
    public Transform pivot;
    public float rotateSpeed = 25f;
    private float startRoatateSpeed;
    private float starty;
    //knockback
    public float knockBackForce;
    public float knockBackTime;
    private float knockBackCounter;
    public bool knockBack  = false;
    public bool startEvent = false;
    public bool storyEvent = false;
    public bool bossEvent  = false;
    //fieldのアニメーション
    public Animator anim;
    //fieldAttackの時武器objectを表示する
    [SerializeField] private GameObject fieldSword;

    #endregion

    #region ojectとのEvent
    //objectとの接触
    Coin_Ani coin_Animation;
    public GameObject objectMessage;
    [SerializeField] Text messageText;
    public GameObject boxMessage;
    public GameObject quest_Button;
    public GameObject status_Button;
    public GameObject playerBody;
    [SerializeField] public Camera[] fieldCams;
    [SerializeField] GameObject ui_Joystick;
    [SerializeField] GameObject ui_CameraJoystick;
    [SerializeField] GameObject ui_FieldAttack;
    [SerializeField] GameObject ui_Jump;
    [SerializeField] GameObject ui_Pause;
    [SerializeField] GameObject ui_Quest;
    [SerializeField] GameObject ui_Help;
    public bool eventStart = false;
    public bool box_Open   = false;
    [SerializeField] private ParticleSystem pickUp_Particle;
    public ParticleSystem heal_Particle;

    //dialogueを表示するに必要なこと
    public GameObject NPC;
    public GameObject treasure_Box;
    InterectionEvent IE;

    #endregion

   


    // Start is called before the first frame update
    void Start()
    {

        transform.position = GameManager.instance.nextPlayerPosition;
        controller = GetComponent<CharacterController>();
        theStatusController = FindObjectOfType<StatusController>();
        DM = FindObjectOfType<DialogueManager>();
        HM = FindObjectOfType<HealthManager>();
        coin_Animation = FindObjectOfType<Coin_Ani>();
        startMoveSpeed = moveSpeed;
        startRoatateSpeed = rotateSpeed;
        fieldSword.SetActive(false);
        quest_Button.SetActive(false);
        status_Button.SetActive(false);
        FieldCamMain();
        PlayerState = MovementState.Move;
    }

    // Update is called once per frame
    void Update()
    {
        switch (PlayerState)
        {
            case (MovementState.Move):
                if (knockBackCounter <= 0)
                {
                    float yStore = moveDirection.y;                    
                    moveDirection = (transform.forward * joystick.Vertical()) + (transform.right * joystick.Horizontal());
                    moveDirection = moveDirection.normalized * moveSpeed;
                    moveDirection.y = yStore;

                    if (controller.isGrounded)
                    {
                        
                        moveDirection = (transform.forward * joystick.Vertical()) + (transform.right * joystick.Horizontal());
                        if (jump)
                        {
                            startScale = 0;                            
                            moveDirection.y = 0f;
                            if (theStatusController.GetCurrentSP() > 0 && !tiredStart)
                            {
                                theStatusController.DecreaseStamina(200);
                                moveDirection.y = jumpForce;
                                startScale = jumpForce;
                                SoundManager.instance.PlaySound("JumpSound", 1);
                            }
                        }
                        else
                        {                           
                            moveDirection = moveDirection.normalized * moveSpeed;
                        }
                        jump = false;

                    }
                    moveDirection.y = moveDirection.y + (Physics.gravity.y * gravityScale * Time.deltaTime);
                }
                else
                {
                    knockBackCounter -= Time.deltaTime;
                }
                controller.Move(moveDirection * Time.deltaTime);

                Rotation();
                KnockDown();
                Anim();

                break;
            case (MovementState.Attack):
                break;
        }

    }

    #region Playerの行動処理

    void Rotation()
    {
        //joystickによる方向回転
        if (((joystick.Vertical() != 0 || Input.GetAxis("Vertical") != 0) || (joystick.Horizontal() != 0)|| Input.GetAxis("Horizontal") != 0))
          {
            transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotateSpeed * Time.deltaTime);

          }
    }

    public void Jump()
    {
        //buttonによるJumpButton
        if (PlayerState == MovementState.Move && controller.isGrounded)
        {
            jump = true;
        }      

    }

    void KnockDown()
    {
        //fieldでspが0になったら一時停止
        if (theStatusController.GetCurrentSP() == 0)
        {
            tiredStart = true;          
        }       
        StartCoroutine(Down());
    }
    
    IEnumerator Down()
    {
        //一時停止のCoroutine
        if (!tiredStart)
        {
            yield break;
        }
        else
        {
            moveSpeed = 0f;
            rotateSpeed = 0f;
            if (theStatusController.GetCurrentSP() != 0)
            {
                tiredStart = false;
                moveSpeed = startMoveSpeed;
                rotateSpeed = startRoatateSpeed;
            }                                         
        }

    }

    public void Knockback(Vector3 direction)
    {
        knockBack = true;
        knockBackCounter = knockBackTime;
        moveDirection = direction * knockBackForce;
        moveDirection.y = knockBackForce;
    }

    public void FieldAttack()
    {
        //FieldEnemyをstunさせる
        if (PlayerState == MovementState.Move && controller.isGrounded && tiredStart == false && knockBack == false)
        {
            fieldSword.SetActive(true);
            anim.SetTrigger("fieldAttack");
            PlayerState = MovementState.Attack;
        }
    }

    public void StateMove()
    {
        fieldSword.SetActive(false);
        PlayerState = MovementState.Move;
    }

    void Anim()
    {
        anim.SetBool("isGrounded", controller.isGrounded);
        anim.SetFloat("Speed", (Mathf.Abs(joystick.Vertical()) + Mathf.Abs(joystick.Horizontal())));        
        anim.SetBool("tired", tiredStart);
        anim.SetBool("begin", startAnim);

    }

    #endregion

    #region Objectとの接続

    void OnTriggerEnter(Collider other)
    {
        //NPCとの会話
        if (other.tag == "Interaction")
        {
            NPC = null;
            NPC = other.gameObject;

            theStatusController.questquestObjectTag = null;
            theStatusController.questquestObjectTag = other.tag;

            theStatusController.questObject = null;
            theStatusController.questObject = other.gameObject;

            objectMessage.SetActive(true);
            var interactionObject = other.gameObject.GetComponent<InteractionType>();
            messageText.text = interactionObject.GetMessage();

        }
        //敵との接続
        else if (other.tag == "FieldEnemy" || other.tag == "FieldEnemyB")
        {
            Region region = other.gameObject.GetComponent<Region>();
            GameManager.instance.curRegions = region;
            GameManager.instance.canGetEncounter = true;         
            GameManager.instance.fieldMonName.Add(other.gameObject.name);

        }
        //宝箱との接続
        else if (other.tag == "Box")
        {
            treasure_Box = null;
            treasure_Box = other.gameObject;
           
            theStatusController.questquestObjectTag = null;
            theStatusController.questquestObjectTag = other.tag;

            theStatusController.questObject = null;
            theStatusController.questObject = other.gameObject;
            boxMessage.SetActive(true);
        }
        //QuestObjectとの接続
        else if (other.tag == "QuestObjectA")
        {
            theStatusController.questquestObjectTag = null;
            theStatusController.questquestObjectTag = other.tag;

            theStatusController.questObject = null;
            theStatusController.questObject = other.gameObject;

            quest_Button.SetActive(true);
        }
        //StatusObjectとの接続
        else if (other.tag == "StatusObject")
        {
            theStatusController.questquestObjectTag = null;
            theStatusController.questquestObjectTag = other.tag;

            theStatusController.questObject = null;
            theStatusController.questObject = other.gameObject;

            status_Button.SetActive(true);
        }
        //最初のEvent
        else if (other.tag == "StartEvent" && !GameManager.instance.eventFlags[0])
        {            
            NPC = null;
            NPC = other.gameObject;

            var interactionObject = other.gameObject.GetComponent<InteractionType>();
            messageText.text = interactionObject.GetMessage();
            
            HM.blackScreenObject.SetActive(true);
            HM.blackScreen.color = new Color(HM.blackScreen.color.r, HM.blackScreen.color.g, HM.blackScreen.color.b, 1.0f);
            
            StartCoroutine(StartWaitCollision());
            eventStart = true;
            
            FieldCamEvent();
            startEvent = false;            
        }
        //FieldでHeal
        else if (other.tag == "Field_Heal")
        {
            if (!heal_Particle.isPlaying)
            {
                SoundManager.instance.PlaySound("Recovery_Field", 2);
                heal_Particle.Play();
                HM.HealthPlayer(200);
            }

        }

    }

    void OnTriggerStay(Collider other)
    {
        //buttonの処理
        if (other.tag == "Interaction")
        {
            if (!eventStart)
            {
                if (PlayerState == MovementState.Attack)
                {
                    objectMessage.SetActive(false);
                }
                else if (PlayerState == MovementState.Move)
                {
                    objectMessage.SetActive(true);
                }
            }
            
        }        
        else if(other.tag == "QuestObjectA")
        {
            if (PlayerState == MovementState.Attack)
            {
                quest_Button.SetActive(false);
            }
            else if (PlayerState == MovementState.Move)
            {
                quest_Button.SetActive(true);
            }
        }
        else if (other.tag == "Box")
        {
            if (PlayerState == MovementState.Attack)
            {
                boxMessage.SetActive(false);
            }
            else if (PlayerState == MovementState.Move)
            {
                boxMessage.SetActive(true);
            }
        }
        else if (other.tag == "StatusObject")
        {
            if (PlayerState == MovementState.Attack)
            {
                status_Button.SetActive(false);
            }
            else if (PlayerState == MovementState.Move)
            {
                status_Button.SetActive(true);
            }            
        }
    }

    void OnTriggerExit(Collider other)
    {
        //buttonの処理
        if (other.tag == "Interaction")
        {
            objectMessage.SetActive(false);
        }
        //playerの状態をreset
        else if (other.tag == "FieldEnemy" || other.tag == "FieldEnemyB")
        {
            GameManager.instance.canGetEncounter = false;
        }
        else if (other.tag == "Box")
        {
            boxMessage.SetActive(false);
        }
        else if(other.tag == "QuestObjectA")
        {
            quest_Button.SetActive(false);
        }
        else if(other.tag == "StatusObject")
        {
            status_Button.SetActive(false);
        }
        //starteventを削除対象にする
        else if(other.tag == "StartEvent")
        {
            if (GameManager.instance.fieldMonName.Count <=0)
            {
                GameManager.instance.fieldMonName.Add(other.gameObject.name);
            }
            
            GameManager.instance.eventFinish = true;
            Destroy(other.gameObject);
        }
        //BossEventを削除対象にする
        else if (other.tag == "EventBoss")
        {
            Destroy(other.gameObject);
        }
    }

    //start会話EventCoroutine
    public IEnumerator StartWaitCollision()
    {
        if (!GameManager.instance.eventFlags[0])
        {
            yield return new WaitUntil(() => !LoadingSlideManager.loadingFinished);
            SettingUI(false);
            playerBody.SetActive(false);
            HM.fadeSpeed = 0.8f;
            yield return new WaitForSeconds(0.5f);
            HM.isFadeFromBlack = true;
            yield return new WaitUntil(() => !HM.isFadeFromBlack);
            HM.fadeSpeed = 2f;

            DM.ShowDialogue(NPC.transform.GetComponent<InterectionEvent>().GetDialogue());
        }
    }
    //会話EventCoroutine
    public IEnumerator WaitCollision()
    {
        yield return new WaitUntil(() => !HM.isFadeToDialogueBackground);

        playerBody.SetActive(false);
        DM.ShowDialogue(NPC.transform.GetComponent<InterectionEvent>().GetDialogue());
    }
    //Eventの時のPlayerUI処理
    public void SettingUI(bool p_flag)
    {
        ui_Joystick.SetActive(p_flag);
        ui_CameraJoystick.SetActive(p_flag);
        ui_FieldAttack.SetActive(p_flag);
        ui_Jump.SetActive(p_flag);
        ui_Pause.SetActive(p_flag);
        ui_Quest.SetActive(p_flag);
        ui_Help.SetActive(p_flag);
        joystick.inputVector = Vector3.zero;
        joystick.joystickImg.rectTransform.anchoredPosition = Vector3.zero;
    }

    #endregion

    #region UI_buttonで行う処理

    public void ClickQuestButton()
    {
        eventStart = true;
        storyEvent = true;
        HM.isFadeToDialogueBackground = true;
        objectMessage.SetActive(false);
        StartCoroutine(WaitCollision());
        FieldCamEvent();
        SoundManager.instance.PlaySound("Dialogue", 1);


    }    

    public void OpenTreasureBox()
    {
        treasure_Box.tag = "OpenBox";
        boxMessage.SetActive(false);
        pickUp_Particle.Play();
        treasure_Box.GetComponent<TreasureBox>().BoxOpen();
        GameManager.instance.fieldMonName.Add(treasure_Box.gameObject.name);
        coin_Animation.Coin_Add_Ani();
    }

    #endregion

    #region eventによるCamera処理
    public void FieldCamMain()
    {
        fieldCams[0].enabled = true;
        fieldCams[1].enabled = false;
        if (fieldCams[2])
        {
            fieldCams[2].enabled = false;
        }
    }

    public void FieldCamEvent()
    {
        fieldCams[0].enabled = false;
        fieldCams[1].enabled = true;
        if (fieldCams[2])
        {
            fieldCams[2].enabled = false;
        }
        
    }
    public void FieldCam_BossEvent()
    {
        fieldCams[0].enabled = false;
        fieldCams[1].enabled = false;
        if (fieldCams[2])
        {
            fieldCams[2].enabled = true;
        }            
    }

    #endregion
}
