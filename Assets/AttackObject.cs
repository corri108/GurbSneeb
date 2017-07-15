using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackObject : MonoBehaviour {

    Card ownerCard;
    Card targetCard;
    Attack myAttack;
    [SerializeField]
    private float attackSpeed = 0.075f;
    private Vector3 directionToTravel;
    bool shouldTravel = false;
    int travelCountdown = 20;
    int ownerID = -1;

    [SerializeField]
    private AudioClip attackHit;
    public AudioClip attackSpawn;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        if(shouldTravel)
        {
            this.transform.position += directionToTravel;
        }
        else
        {
            travelCountdown--;
            shouldTravel = travelCountdown == 0 ? true : false;
        }
    }

    public void Setup(Card owner, int myPlayerID, Attack myAttack, Card target)
    {
        this.ownerCard = owner;
        this.targetCard = target;
        this.ownerID = myPlayerID;
        this.myAttack = myAttack;

        Vector3 dir = targetCard.transform.position - ownerCard.transform.position;
        dir.Normalize();
        directionToTravel = dir * attackSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Card c = other.GetComponent<Card>();
        if (c != null && c.ownerID != ownerID)
        {
            //KILLLLLL
            DoAttackDamage();
        }
    }

    void DoAttackDamage()
    {
        targetCard.TakeDamage(myAttack.damage);
        ownerCard.AttackFinished();
        AudioSource.PlayClipAtPoint(attackHit, Camera.main.transform.position);
        GameObject.Destroy(this.gameObject);
    }
}
