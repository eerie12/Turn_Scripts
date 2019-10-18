using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FieldEnemy : MonoBehaviour
{

    #region FieldEnemy変数
    FieldOfViewAngle theViewAngle;

    [SerializeField] private string monsterName;//FieldEnemyの区分用   
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private Transform startPosition;
    [SerializeField] private Quaternion startRotation;

    private bool isWalking;
    private bool isAction;
    private bool isEscape;
    private bool isDead;
    private bool isChasing;
    private bool isReturn;
    private bool isStun;

    [SerializeField] private int fieldHp;//stunの発生->advancedbattle
    [SerializeField] private float walkTime;
    [SerializeField] private float waitTime;
    [SerializeField] private float escapeTime;
    [SerializeField] private float ChaseTime;//追撃時間
    [SerializeField] private float currentChaseTime;//現在の追撃時間
    [SerializeField] private float ChaseDelayTime;//追撃ディレイ
    [SerializeField] private float startlookRadius;
    private float currentTime;//現在の待機時間

    NavMeshAgent nav;
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody rbody;
    [SerializeField] private BoxCollider boxCol;
    private AudioSource theAudio;
    [SerializeField] private AudioClip Sound_mon_Damage;
    [SerializeField] private ParticleSystem fieldMon_Particle;

    private Vector3 destination;

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        theAudio = GetComponent<AudioSource>();
        theViewAngle = GetComponent<FieldOfViewAngle>();
        
        isAction = true;
        currentTime = waitTime;
        startRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead )
        {           
            if(startPosition != null)
            {
                float dist = Vector3.Distance(startPosition.position, theViewAngle.GetTargetPos());

                if (dist < startlookRadius)
                {
                    if (theViewAngle.View() && !isStun)
                    {
                        Move();
                        Chase(theViewAngle.GetTargetPos());
                        StopAllCoroutines();
                        StartCoroutine(ChaseTargetCoroutine());
                    }

                }
            }
            
        }       
    }

    #region Chast,Move,Reset設定
    private void Move()
    {
        if (isWalking || isEscape)
        {           
            nav.SetDestination(transform.position + destination * 3f);
        }       
    }
   
    private void ElapseTime()
    {
        if (isAction)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0 && !isChasing && !isReturn)
                ReSet();
        }
    }

    private void ReSet()
    {
        fieldMon_Particle.Stop();
        fieldMon_Particle.Clear();
        isWalking = false; isEscape = false; isReturn = false; isAction = true;
        nav.speed = walkSpeed;
        nav.ResetPath();
        anim.SetBool("walking", isWalking);
    }

    private void RandomAction()
    {
        int _random = Random.Range(0, 1); //待機、歩く

        if(_random ==0)
        {
            Wait();
        }
        else if(_random == 1)
        {
            TryWalk();
        }
    }

    private void Wait()
    {
        currentTime = waitTime;

    }
    private void TryWalk()
    {
        isWalking = true;
        anim.SetBool("walking", isWalking);
        currentTime = walkTime;
        nav.speed = walkSpeed;
    }

    public void Chase(Vector3 _targetPos)
    {
        destination = _targetPos;
        isWalking = false;
        isEscape = true;
        isChasing = true;
        nav.speed = runSpeed;
        nav.SetDestination(destination);
        anim.SetBool("walking", isEscape);

    }

    IEnumerator ChaseTargetCoroutine()
    {
        currentChaseTime = 0;
        while (currentChaseTime < ChaseTime)
        {
            Chase(theViewAngle.GetTargetPos());
            yield return new WaitForSeconds(ChaseDelayTime);
            currentChaseTime += ChaseDelayTime;
        }
        isChasing = false;
        isEscape = false;
        anim.SetBool("walking", isEscape);

        nav.ResetPath();
        yield return new WaitForSeconds(ChaseDelayTime);

        Chase(startPosition.position);
        yield return new WaitUntil(() => transform.position == new Vector3(startPosition.position.x, transform.position.y, startPosition.position.z));

        transform.rotation = startRotation;
        nav.ResetPath();
        isEscape = false;
        isChasing = false;
        anim.SetBool("walking", isEscape);
    }

    #endregion

    #region stun設定

    IEnumerator Stun()
    {
        if (isStun)
        {
            yield break;
        }
        isStun = true;
        ReSet();
        fieldMon_Particle.Play();

        if (monsterName != "Boss" && monsterName != "Golem_Middle" && monsterName != "Golem_M3")
        {
            GameManager.instance.advancedBattle = true;
        }
        
        yield return new WaitForSeconds(5f);        
        fieldMon_Particle.Stop();
        fieldMon_Particle.Clear();
        if (GameManager.instance.advancedBattle)
        {
            GameManager.instance.advancedBattle = false;
        }
        
        isStun = false;
        if(startPosition != null)
        {
            Chase(startPosition.position);
            yield return new WaitUntil(() => transform.position == new Vector3(startPosition.position.x, transform.position.y, startPosition.position.z));
            transform.rotation = startRotation;
        }        
        nav.ResetPath();        
        isChasing = false;
        isEscape = false;
        anim.SetBool("walking", isEscape);
        
    }
    
    public void Damage(int _dmg, Vector3 _targetPos)
    {
        if (!isDead)
        {
            Time.timeScale = 0.4f;
            fieldMon_Particle.Play();
            anim.SetTrigger("damage");
            StopAllCoroutines();
            isStun = false;
            StartCoroutine(Stun());
        }                      
    }

    private void Dead()
    {
        isWalking = false;
        isEscape = false;
        isDead = true;
    }


    #endregion
}
