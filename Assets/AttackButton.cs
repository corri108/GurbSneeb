using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButton : MonoBehaviour {

    [HideInInspector]
    public int attackID = -1;
    [HideInInspector]
    public Attack attack;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetupButton(Attack a)
    {
        this.attack = a;
        this.transform.GetChild(1).GetComponent<TextMesh>().text = a.attackName;
    }
}
