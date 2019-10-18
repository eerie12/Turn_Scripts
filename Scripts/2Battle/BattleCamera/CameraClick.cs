using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClick : MonoBehaviour
{
    private BattleStateMachine BSM;
    private HeroStateMachine HSM;
    private EnemyStateMachine ESM;
    public Animator cameraAnim;
    public bool rayCast;
    public bool playerTurn = false;

    //攻撃の時のcamera移動
    public float cameraSpeed;
    private Vector3 disValue;

    public Transform pivot;

    [SerializeField] private GameObject[] battleUI;



    //public List<GameObject> targetCharacter = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        HSM = FindObjectOfType<HeroStateMachine>();
        //ESM = FindObjectOfType<EnemyStateMachine>();
        
        for(int i = 0; i< battleUI.Length; i++)
        {
            battleUI[i].SetActive(false);
        }
        
        

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0) && BSM.click && BSM.enemyClicked)
        {
            rayCast = true;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out hit) || hit.collider.tag == "DeadEnemy")

                return;




            var targetCharacter = hit.collider.GetComponent<EnemyStateMachine>();
            if (targetCharacter != null)
            {
                foreach (GameObject enemy in BSM.EnemysInBattle) //enemysinbattleの中のenemyprefabsを呼ぶ
                {
                    BSM.Input2(targetCharacter.gameObject);
                    rayCast = false;
                }
            }

        }
        Anim();


    }

    void Anim()
    {
        cameraAnim.SetBool("PlayerTurn", playerTurn);
    }

    /*
    void CameraTransrate()
    {
        transform.LookAt(pivot);
    }
    */
    /*
    void CameraTransrate()
    {
        if(HSM.actionStarted)
        {
            Debug.Log("cameratransrate");
            
            this.transform.position = Vector3.Lerp(this.transform.position, HSM.transform.position + disValue, cameraSpeed);
           
        }

    }
    */
    
    void BattleUIStart()
    {
        for (int i = 0; i < battleUI.Length; i++)
        {
            battleUI[i].SetActive(true);
            /*
            if(BSM.EnemysInBattle[i].tag == "EnemyBoss")
            {
                EnemyStateMachine ESM = BSM.EnemysInBattle[i].GetComponent<EnemyStateMachine>();
                ESM.cur_cooldown = ESM.max_cooldown;
            }
            */
            
            
        }
    }
    
}
