using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;
    [SerializeField] string csv_FileName;

    Dictionary<int, Dialogue> dialogueDic = new Dictionary<int, Dialogue>();

    public static bool isFinish = false;

    private void Awake()
    {
      if(instance = null)
        {
            instance = this;
            DialogueParser theParser = GetComponent<DialogueParser>();
            Dialogue[] dialouges = theParser.Parse(csv_FileName);
            for(int i = 0; i<dialouges.Length; i++)
            {
                dialogueDic.Add(i + 1,dialouges[i]);
            }
            isFinish = true;
        }  
    }


}
