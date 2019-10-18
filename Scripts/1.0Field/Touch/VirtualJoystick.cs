using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour,IDragHandler,IPointerUpHandler,IPointerDownHandler
{
    //[SerializeField] private RectTransform rect_Background;
    //[SerializeField] private RectTransform rect_Joystick;
    private Image bgImag;
    public Image joystickImg;
    public Vector3 inputVector;
    private CharacterController playercontroll;

    public Vector2 pos;

    public float fix_x;
    public float fix_y;


    //public Vector3 InputDirection { set; get; }//プロパティ

    void Start()
    {
       // radius = rect_Background.rect.width * 0.5f;

        bgImag = GetComponent<Image>();
        joystickImg = transform.GetChild(0).GetComponent<Image>();
        playercontroll = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();



    }
    public virtual void OnDrag(PointerEventData ped)
    {
        //Vector2 value = ped.position - (Vector2)rect_Background.position;
        //rect_Joystick.localPosition = value;
        //value = Vector2.ClampMagnitude(value, radius);

        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImag.rectTransform,ped.position,ped.pressEventCamera,out pos))
        {
         
            pos.x = (pos.x / bgImag.rectTransform.sizeDelta.x);
            pos.y = (pos.y / bgImag.rectTransform.sizeDelta.y);

            inputVector = new Vector3(pos.x+ fix_x, 0,pos.y+fix_y);
            inputVector = (inputVector.magnitude >1.0f) ? inputVector.normalized:inputVector;


            joystickImg.rectTransform.anchoredPosition = new Vector3(inputVector.x * (bgImag.rectTransform.sizeDelta.x/3), inputVector.z * (bgImag.rectTransform.sizeDelta.y/3));


        }
    }
    public virtual void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);
    }
    public virtual void OnPointerUp(PointerEventData ped)
    {
        inputVector = Vector3.zero;
        joystickImg.rectTransform.anchoredPosition = Vector3.zero;
        //rect_Joystick.localPosition = Vector3.zero;
    }

    public float Horizontal()
    {
        // if (inputVector.x != 0)
        //if (playercontroll.isGrounded)

            return inputVector.x;
       // else
           // return 0;
       
       // else
            //return Input.GetAxis("Horizontal");
    }
    public float Vertical()
    {
        //if (inputVector.x != 0)
        //if (playercontroll.isGrounded)

            return inputVector.z;
        //else
           // return 0;
        //else
        //return Input.GetAxis("Vertical");


    }
}
