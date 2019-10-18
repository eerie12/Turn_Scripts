using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCameraController : MonoBehaviour
{
    Vector3 startPos;
    Quaternion startRot;

    public bool camEvent = false;
    public Transform MainCam;

    PlayerControllerrbody playerMovement;


    Coroutine coroutine;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerrbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.eventFlags[0])
        {
            if (!playerMovement.eventStart)
            {
                transform.position = MainCam.position;
                transform.rotation = MainCam.rotation;
            }
        }
        
        
    }

    public void CamStartSetting()
    {
        startPos = MainCam.position;
        startRot = MainCam.rotation;

    }

    public void CameraTargetting(Transform p_Target, float p_CamSpeed = 0.15f,bool p_isReset = false, bool p_isFinish = false)
    {
        
        if (!p_isReset)
        {
            if (p_Target != null)
            {
                StopAllCoroutines();
                coroutine = StartCoroutine(CameraTargettingCoroutine(p_Target, p_CamSpeed));
                camEvent = false;

            }
        }
        else
        {
            if(coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            StartCoroutine(CameraResetCoroutine(p_CamSpeed,p_isFinish));
            camEvent = false;

        }
        
        

    }

    IEnumerator CameraTargettingCoroutine(Transform p_Target, float p_CamSpeed = 0.15f)
    {
        Vector3 t_TargetPos = p_Target.position;
        Vector3 t_TargetFrontPos = t_TargetPos + p_Target.forward;
        Vector3 t_Direction = (t_TargetPos - t_TargetFrontPos).normalized;

        while (transform.position != t_TargetFrontPos || Quaternion.Angle(transform.rotation, Quaternion.LookRotation(t_Direction)) >= 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, t_TargetFrontPos, p_CamSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(t_Direction), p_CamSpeed);
            yield return null;

        }
        camEvent = true;

    }

    IEnumerator CameraResetCoroutine(float p_CamSpeed = 0.1f, bool p_isFinish = false)
    {
        
        yield return new WaitForSeconds(0.5f);
       
        playerMovement.playerBody.SetActive(true);
        while (transform.position != startPos || Quaternion.Angle(transform.rotation, startRot) >= 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, p_CamSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, startRot, p_CamSpeed);
            yield return null;

        }
        transform.position = startPos;

        if (p_isFinish)
        {
            //対話が終了したらリセット
            playerMovement.FieldCamMain();
            playerMovement.eventStart = false;
            if (playerMovement.storyEvent)
            {
                playerMovement.storyEvent = false;
            }
            
            playerMovement.SettingUI(true);
            if (!GameManager.instance.eventFinish)
            {
            SoundManager.instance.PlaySound("Town", 0);
            }

        }

        camEvent = true;
    }
}
