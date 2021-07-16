using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLog : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("123456789.Substring(0,4):"+"123456789".Substring(0,4));
		Debug.Log("123456789.Substring(0,4).Length:"+"123456789".Substring(0,4).Length);
    }


}
