using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour {

    [SerializeField]
    public List<Player> seats = new List<Player>();

    [HideInInspector]
    public Player currentPlayerTurn = null;

    public static Table instance = null;
	// Use this for initialization
	void Awake ()
    {
		if(instance == null)
        {
            instance = this;
            GameObject.DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this)
        {
            GameObject.Destroy(this.gameObject);
        }
	}

    private void Start()
    {
        currentPlayerTurn = seats[0];
        currentPlayerTurn.ActivatePhase();
    }

    // Update is called once per frame
    void Update () {
		
	}

    public static Player GetPlayerFromID(int id)
    {
        foreach(var p in instance.seats)
        {
            if (p.PlayerID == id)
                return p;
        }

        return null;
    }

    public static void HideAllOthers(int otherThanID)
    {
        foreach (var p in instance.seats)
        {
            if (p.PlayerID == otherThanID) { }
            //dn
            else
            {
                p.gameObject.SetActive(false);
            }
        }

        instance.transform.GetChild(0).gameObject.SetActive(false);
    }

    public static void HideAllOthers(int otherThanID, int otherThanID2)
    {
        foreach (var p in instance.seats)
        {
            if (p.PlayerID == otherThanID || p.PlayerID == otherThanID2) { }
            //dn
            else
            {
                p.gameObject.SetActive(false);
            }
        }

        instance.transform.GetChild(0).gameObject.SetActive(false);
    }

    public static void ShowAllPlayers()
    {
        foreach (var p in instance.seats)
        {
            p.gameObject.SetActive(true);
        }

        instance.transform.GetChild(0).gameObject.SetActive(true);
    }
}
