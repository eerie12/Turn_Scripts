using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEvent : MonoBehaviour
{
    FieldCameraController theEventCam;
    PlayerControllerrbody playerMovement;
    HealthManager HM;

    #region evnetParticle

    [SerializeField] private ParticleSystem[] eventParticle;
    [SerializeField] private ParticleSystem eventParticleB;
    [SerializeField] private ParticleSystem event_C_Obj_Particle;
    [SerializeField] private ParticleSystem[] eventParticleC;
    [SerializeField] private ParticleSystem[] eventParticle_Boss;

    #endregion

    #region evnetObject

    [SerializeField] private GameObject[] mapEvent;
    [SerializeField] private GameObject[] mapEventB;
    [SerializeField] private GameObject[] mapEventC;
    [SerializeField] private GameObject mapEventD;
    [SerializeField] private GameObject mapEvent_Boss;
    [SerializeField] private GameObject mapEvent_Boss_Obj;
    [SerializeField] private GameObject boss_EventRotation_Point;

    #endregion

    #region event変数

    [SerializeField] private float eventC_Speed;
    [SerializeField] private float rotationSpeed;
    private bool eventC = false;

    [SerializeField] private Transform[] mapEventTransform;
    private bool eventStart = false;

    #endregion 

    // Start is called before the first frame update
    void Start()
    {
        theEventCam = FindObjectOfType<FieldCameraController>();
        //CMS = FindObjectOfType<CameraShake>();
        playerMovement = FindObjectOfType<PlayerControllerrbody>();
        HM = FindObjectOfType<HealthManager>(); ;

    }

   

    private void Update()
    {

            //Map_Event_Camera_Reset();

    }

    #region Map_Event_Camera設定

    public void Map_Event_Camera()
    {
        playerMovement.FieldCamEvent();
        playerMovement.eventStart = true;
        playerMovement.SettingUI(false);
        StartCoroutine(Map_Event());

    }

    public void Map_Event_Camera_Reset()
    {
        playerMovement.FieldCamEvent();
        playerMovement.eventStart = true;
        playerMovement.SettingUI(false);
        StartCoroutine(Map_Event_Reset());
    }

    public void Map_Event_CameraB()
    {

        playerMovement.FieldCamEvent();
        playerMovement.eventStart = true;
        playerMovement.SettingUI(false);
        StartCoroutine(Map_EventB());
    }

    public void Map_Event_CameraC()
    {

        playerMovement.FieldCamEvent();
        playerMovement.eventStart = true;
        playerMovement.SettingUI(false);
        StartCoroutine(Map_EventC());
    }

    public void Map_Event_Camera_Boss()
    {
        playerMovement.FieldCamEvent();
        //playerMovement.FieldCam_BossEvent();
        playerMovement.eventStart = true;
        playerMovement.SettingUI(false);
        StartCoroutine(Map_Event_Boss());
    }

    #endregion 

    #region Map_Event_Cor

    public IEnumerator Map_Event()
    {
        if (eventStart)
        {
            yield break;
        }

        eventStart = true;

        theEventCam.CamStartSetting();
        theEventCam.CameraTargetting(mapEventTransform[0]);
        yield return new WaitUntil(() => theEventCam.camEvent);

        for (int i = 0; i < mapEvent.Length; i++)
        {
            eventParticle[i].Play();
            yield return new WaitForSeconds(0.1f);
            mapEvent[i].SetActive(true);
            //GameManager.instance.fieldObjectName_alive.Add(gameObject.name);
            yield return new WaitForSeconds(0.8f);
        }
        theEventCam.camEvent = false;

        yield return new WaitForSeconds(0.5f);
        theEventCam.CameraTargetting(null, 0.1f, true, true);
        yield return new WaitUntil(() => theEventCam.camEvent);

        eventStart = false;

    }

    public IEnumerator Map_Event_Reset()
    {
        if (eventStart)
        {
            yield break;
        }

        eventStart = true;

        theEventCam.CamStartSetting();
        theEventCam.CameraTargetting(mapEventTransform[1]);
        yield return new WaitUntil(() => theEventCam.camEvent);

        for (int i = 0; i < mapEvent.Length; i++)
        {
            eventParticle[i].Play();
            yield return new WaitForSeconds(0.1f);
            mapEvent[i].SetActive(false);
            yield return new WaitForSeconds(0.8f);
        }
        theEventCam.camEvent = false;

        yield return new WaitForSeconds(0.5f);

        theEventCam.CameraTargetting(null, 0.1f, true, true);
        yield return new WaitUntil(() => theEventCam.camEvent);

        eventStart = false;

    }

    public IEnumerator Map_EventB()
    {
        if (eventStart)
        {
            yield break;
        }

        eventStart = true;

        theEventCam.CamStartSetting();
        theEventCam.CameraTargetting(mapEventTransform[2]);
        yield return new WaitUntil(() => theEventCam.camEvent);
        yield return new WaitForSeconds(0.2f);
        eventParticleB.Play();
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.fieldMonName.Add(event_C_Obj_Particle.gameObject.name);
        event_C_Obj_Particle.Stop();
        event_C_Obj_Particle.Clear();
        for (int i = 0; i < mapEventB.Length; i++)
        {
            
            mapEventB[i].SetActive(false);
            GameManager.instance.fieldMonName.Add(mapEventB[i].gameObject.name);
        }
        theEventCam.camEvent = false;

        yield return new WaitForSeconds(0.5f);

        theEventCam.CameraTargetting(null, 0.1f, true, true);
        yield return new WaitUntil(() => theEventCam.camEvent);

        eventStart = false;

    }

    public IEnumerator Map_EventC()
    {
        if (eventStart)
        {
            yield break;
        }

        eventStart = true;

        CameraShake CMS = playerMovement.fieldCams[1].GetComponent<CameraShake>();

        theEventCam.CamStartSetting();
        theEventCam.CameraTargetting(mapEventTransform[3]);
        yield return new WaitUntil(() => theEventCam.camEvent);
        yield return new WaitForSeconds(1f);

        for(int i = 0;i < mapEventC.Length; i++)
        {

            mapEventC[i].SetActive(true);

            Vector3 stopPosition = new Vector3(mapEventC[i].transform.position.x, mapEventC[i].transform.position.y - 5f, mapEventC[i].transform.position.z);
           

            //eventC_Particle.set
            while (MoveTowardTarget(mapEventC[i], stopPosition, eventC_Speed))
            {

                yield return null;
            }
            StartCoroutine(CMS.Shake(0.15f, 0.6f));
            eventParticleC[i].Play();
            yield return new WaitForSeconds(0.1f);
        }
 
        yield return new WaitForSeconds(0.5f);
        mapEventD.SetActive(true);

        theEventCam.CameraTargetting(null, 0.1f, true, true);
        yield return new WaitUntil(() => theEventCam.camEvent);

        eventStart = false;
    }


    public IEnumerator Map_Event_Boss()
    {
        if (eventStart)
        {
            yield break;
        }

        eventStart = true;
        theEventCam.CamStartSetting();
        mapEvent_Boss_Obj.SetActive(true);


        CameraShake CMS = playerMovement.fieldCams[1].GetComponent<CameraShake>();
        CameraShake CMS2 = playerMovement.fieldCams[2].GetComponent<CameraShake>();
        AttakedTime bossMove = mapEvent_Boss.GetComponent<AttakedTime>();


        HM.isFadeToBlack = true;
        yield return new WaitUntil(() => !HM.isFadeToBlack);
        Debug.Log("blackout");
        theEventCam.gameObject.transform.position = mapEventTransform[4].transform.position;
        theEventCam.gameObject.transform.rotation = mapEventTransform[4].transform.rotation;
        SoundManager.instance.StopBGM();

        HM.isFadeFromBlack = true;
        yield return new WaitUntil(() => !HM.isFadeFromBlack);
        yield return new WaitForSeconds(1f);
        StartCoroutine(CMS.Shake(2f, 0.25f));
        SoundManager.instance.PlaySound("Event_Boss_Start", 2);
        yield return new WaitForSeconds(1.5f);

        mapEvent_Boss.SetActive(true);
        playerMovement.FieldCam_BossEvent();

        Vector3 stopPosition = new Vector3(mapEvent_Boss.transform.position.x, mapEvent_Boss.transform.position.y - 14f, mapEvent_Boss.transform.position.z);
        theEventCam.CameraTargetting(mapEventTransform[5]);
        StartCoroutine(CMS2.Shake(2.5f, 0.05f));
        while (MoveTowardTarget(mapEvent_Boss, stopPosition, eventC_Speed-12.5f))
        {

            yield return null;
        }
        for (int i = 0; i < eventParticle_Boss.Length; i++)
        {
            eventParticle_Boss[i].Play();
        }
        bossMove.Boss_WildJump();
        StartCoroutine(CMS2.Shake(0.2f, 0.6f));
        SoundManager.instance.PlaySound("Event_Boss_Middle", 1);
        SoundManager.instance.PlaySound("Event_Boss_Middle0.1", 1);
        
        yield return new WaitForSeconds(0.2f);
        playerMovement.FieldCamEvent();
        StartCoroutine(CMS.Shake(0.2f, 0.6f));


        yield return new WaitForSeconds(0.5f);

        bossMove.Boss_WildEvent();

        yield return new WaitForSeconds(1.6f);
        StartCoroutine(CMS.Shake(1f, 0.4f));

        yield return new WaitForSeconds(2f);
        SoundManager.instance.PlaySound("Town", 0);

        theEventCam.CameraTargetting(null, 0.1f, true, true);
        yield return new WaitUntil(() => theEventCam.camEvent);

        eventStart = false;     
    }

    #endregion

    #region その他の設定

    private bool MoveTowardTarget(GameObject eventC,Vector3 target, float speed)
    {
        Vector3 dir = target - eventC.transform.position;//方向を求める
        return target != (eventC.transform.position = Vector3.MoveTowards(eventC.transform.position, target, speed * Time.deltaTime));
    }

    private bool RotationTowardTarget(GameObject Boss_Obj, Vector3 target, float rotation_Speed)
    {

        Vector3 dir = target - Boss_Obj.transform.position;//方向を求める   
        return Boss_Obj.transform.rotation != (Boss_Obj.transform.rotation = Quaternion.Lerp(Boss_Obj.transform.rotation, Quaternion.LookRotation(dir), rotation_Speed * Time.deltaTime));
    }

    #endregion
}
