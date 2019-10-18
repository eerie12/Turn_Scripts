using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraType
{
    ObjectFront,
    Reset
}

[System.Serializable]
public class Dialogue 
{
    [Header("カメラにターゲティングされる対象")]
    public CameraType cameraType;
    public Transform tf_Target;


    [HideInInspector]
    public string name;

    [HideInInspector]
    public string[] contexts;

    
    [HideInInspector]
    public string[] VoiceName;

}

[System.Serializable]
public class EventTiming
{
    public int[] eventNum;
    public int[] check;
    //public int[] eventConditions;
    //public bool conditionFlag;
    //public int eventEndNum;
}


[System.Serializable]
public class DialogueEvent
{
    public string name;
    public EventTiming eventTiming;
    public Vector2 line;
    public Dialogue[] dialogues;

    [Space]
    public Vector2 lineB;
    public Dialogue[] dialoguesB;

    [Space]
    public Vector2 lineC;
    public Dialogue[] dialoguesC;

    [Space]
    public Vector2 lineD;
    public Dialogue[] dialoguesD;

    [Space]
    public Vector2 lineE;
    public Dialogue[] dialoguesE;
}
