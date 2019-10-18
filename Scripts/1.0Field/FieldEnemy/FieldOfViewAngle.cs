using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewAngle : MonoBehaviour
{
    [SerializeField] private float viewAngle;//FieldEnemyが見る角度
    [SerializeField] private float viewDistance;//FieldEnemyが見る距離
    [SerializeField] private LayerMask targetMask;

    private PlayerControllerrbody PlayerController;

    void Start()
    {
        PlayerController = FindObjectOfType<PlayerControllerrbody>();
    }

    public Vector3 GetTargetPos()//探知したPlayerを位置をFieldEnemyに伝達
    {
        return PlayerController.transform.position;
       
    }

    public bool View()//視野に入るPlayerを探知
    {

        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance, targetMask);

        for(int i = 0; i< _target.Length; i++)
        {
            Transform _targetTf = _target[i].transform;
            if(_targetTf.name == "PlayerCharacter")
            {
                Vector3 _direction = (_targetTf.position - transform.position).normalized;
                float _angle = Vector3.Angle(_direction, transform.forward);

                if (_angle < viewAngle * 0.5f)
                {
                    RaycastHit _hit;
                    if(Physics.Raycast(transform.position + transform.up*0.5f, _direction,out _hit, viewDistance))
                    {
                        if(_hit.transform.name == "PlayerCharacter")
                        {                          
                         
                            return true;
                        }
                        
                    }
                }
            }
        }
        return false;
    }
}
