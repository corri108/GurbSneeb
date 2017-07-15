using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private TableCamera cam;
    private PlayerPhase phase = PlayerPhase.Waiting;

    private List<Card> deck = new List<Card>();
    private List<Card> hand = new List<Card>();
    private Card field = null;
    [SerializeField]
    private int playerID = 1;

    [SerializeField]
    private LayerMask cards;
    [SerializeField]
    private LayerMask buttons;
    [SerializeField]
    private LayerMask cardsAndbuttons;
    [SerializeField]
    private Transform deckTransform;
    [SerializeField]
    private TextMesh deckCount;
    [SerializeField]
    private Transform fieldTransform;
    [SerializeField]
    private Transform descriptionBase;
    [SerializeField]
    private Transform decisionBase;
    [SerializeField]
    private TextMesh descriptionText;
    [SerializeField]
    private TextMesh attacksText;
    [SerializeField]
    private TextMesh attackDescriptionsText;
    [SerializeField]
    private Transform zoomedTransform;
    [SerializeField]
    private GameObject decisionGlow, attackGlow, cardGlow;
    [SerializeField]
    private Transform decisionButtons, attackButtons;
    [SerializeField]
    private GameObject backButton;
    [SerializeField]
    private GameObject chooseEnemy;
    [SerializeField]
    private GameObject confirmDecision;

    [SerializeField]
    private AudioClip clickSound;

    int attacksTextInitialSize;
    Vector3 cardDeckOffset = new Vector3(0.1f, 0.1f, 0f);

    public int PlayerID
    {
        get { return playerID; }
        set
        {
            playerID = value;
        }
    }
	// Use this for initialization
	void Start ()
    {
        cam = GameObject.FindObjectOfType<TableCamera>();
        descriptionBase.gameObject.SetActive(false);
        decisionBase.gameObject.SetActive(true);
        attacksTextInitialSize = attacksText.fontSize;

        LoadDeck();
        field.Visible = true;
        field.SetFrontOrder(49);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Table.instance.currentPlayerTurn != this)
            return;

        CardControl();
        DecisionControl();

        if(Input.GetMouseButtonDown(2))
        {
            DestroyFieldCard();
        }
	}

    void LoadDeck()
    {
        int backOrder = 2;
        for(int i=0; i < deckTransform.childCount; ++i)
        {
            deckTransform.GetChild(i).position += cardDeckOffset * i;
            deckTransform.GetChild(i).GetComponent<Card>().SetBackOrder(backOrder);
            deckTransform.GetChild(i).GetComponent<Card>().ownerID = this.PlayerID;
            deck.Add(deckTransform.GetChild(i).GetComponent<Card>());
            backOrder++;
        }

        field = fieldTransform.GetChild(0).GetComponent<Card>();
        field.ownerID = this.PlayerID;

        deckCount.transform.position += cardDeckOffset * (deckTransform.childCount - 1);
        deckCount.text = deck.Count.ToString();
        deckCount.GetComponent<MeshRenderer>().sortingOrder = 50;
        attacksText.fontSize = field.HasLongAttackName() ? field.GetLongAttackLength() : attacksTextInitialSize;
    }

    Card myCurrentCard = null;
    Card enemyCurrentCard = null;
    Card cardToAttack = null;
    Attack selectedAttack = null;
    void CardControl()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = new Vector3(mousePosition.x, mousePosition.y, 2);
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Ray2D pointerRay = new Ray2D(mousePosition, cam.transform.forward);
        RaycastHit2D rh;

        rh = Physics2D.Raycast(pointerRay.origin, pointerRay.direction, 100, cards);

        if(rh.collider != null)
        {
            Card card = rh.collider.gameObject.GetComponent<Card>();

            #region Not Choosing Enemy

            if (phase != PlayerPhase.ChooseEnemy && phase != PlayerPhase.ConfirmAttack && phase != PlayerPhase.Attacking)
            {
                if (card != null && card.Visible && card.ownerID == this.PlayerID)
                {
                    Transform camZoom = zoomedTransform.GetChild(1);
                    cam.Zoom(camZoom.localPosition, camZoom.GetComponent<Camera>().orthographicSize);
                    myCurrentCard = card;
                    descriptionText.text = myCurrentCard.GetDescription();
                    attacksText.text = myCurrentCard.GetAttacksString();
                    attackDescriptionsText.text = myCurrentCard.GetAttackDescriptionsString();
                    descriptionBase.gameObject.SetActive(true);
                    decisionBase.gameObject.SetActive(false);
                }
                else if (card != null && card.Visible)
                {
                    //Debug.Log("Not my card");
                    //belongs to a different player.
                    Player owner = Table.GetPlayerFromID(card.ownerID);

                    enemyCurrentCard = card;
                    Transform camZoom = owner.zoomedTransform.GetChild(1);
                    cam.Zoom(camZoom.localPosition, camZoom.GetComponent<Camera>().orthographicSize);
                    owner.descriptionText.text = enemyCurrentCard.GetDescription();
                    owner.attacksText.text = enemyCurrentCard.GetAttacksString();
                    owner.attackDescriptionsText.text = enemyCurrentCard.GetAttackDescriptionsString();
                    owner.descriptionBase.gameObject.SetActive(true);
                    owner.decisionBase.gameObject.SetActive(false);
                }
            }

            #endregion

            #region We Are Choosing Enemy

            else
            {
                if (card != null && card.Visible && card.ownerID == this.PlayerID)
                {
                    if (Input.GetMouseButton(1))
                    {
                        Transform camZoom = zoomedTransform.GetChild(1);
                        cam.Zoom(camZoom.localPosition, camZoom.GetComponent<Camera>().orthographicSize);
                        myCurrentCard = card;
                        descriptionText.text = myCurrentCard.GetDescription();
                        attacksText.text = myCurrentCard.GetAttacksString();
                        attackDescriptionsText.text = myCurrentCard.GetAttackDescriptionsString();
                        descriptionBase.gameObject.SetActive(true);
                        decisionBase.gameObject.SetActive(false);
                    }
                    else
                    {
                        MyCardZoomout();
                        cam.Zoomout();
                    }
                }
                else if (card != null && card.Visible)
                {
                    if (Input.GetMouseButton(1))
                    {
                        Player owner = Table.GetPlayerFromID(card.ownerID);

                        enemyCurrentCard = card;
                        Transform camZoom = owner.zoomedTransform.GetChild(1);
                        cam.Zoom(camZoom.localPosition, camZoom.GetComponent<Camera>().orthographicSize);
                        owner.descriptionText.text = enemyCurrentCard.GetDescription();
                        owner.attacksText.text = enemyCurrentCard.GetAttacksString();
                        owner.attackDescriptionsText.text = enemyCurrentCard.GetAttackDescriptionsString();
                        owner.descriptionBase.gameObject.SetActive(true);
                        owner.decisionBase.gameObject.SetActive(false);
                    }
                    else
                    {
                        EnemyCardZoomout();
                        cam.Zoomout();
                    }
                }
            }

            #endregion
        }
        else
        {
            cam.Zoomout();

            if (myCurrentCard != null)
            {
                MyCardZoomout();
            }
            else if(enemyCurrentCard != null)
            {
                EnemyCardZoomout();
            }
        }

        if (myCurrentCard != null)
        {
            Transform cardZoom = zoomedTransform.GetChild(0);
            myCurrentCard.Zoom(cardZoom);
            Table.HideAllOthers(this.playerID);
        }

        if (enemyCurrentCard != null)
        {
            Player owner = Table.GetPlayerFromID(enemyCurrentCard.ownerID);
            Transform cardZoom = owner.zoomedTransform.GetChild(0);
            enemyCurrentCard.Zoom(cardZoom);
            Table.HideAllOthers(owner.playerID, this.playerID);
        }
    }

    void MyCardZoomout()
    {
        if (myCurrentCard == null)
            return;
        //player stuff
        descriptionText.text = "";
        attacksText.text = "";
        attackDescriptionsText.text = "";
        descriptionBase.gameObject.SetActive(false);
        decisionBase.gameObject.SetActive(true);
        //mycur card stuff
        myCurrentCard.Zoomout(fieldTransform);
        myCurrentCard = null;
        Table.ShowAllPlayers();
    }

    void EnemyCardZoomout()
    {
        if (enemyCurrentCard == null)
            return;
        //get enemy player
        Player owner = Table.GetPlayerFromID(enemyCurrentCard.ownerID);
        //enemy player stuff
        owner.descriptionText.text = "";
        owner.attacksText.text = "";
        owner.attackDescriptionsText.text = "";
        owner.descriptionBase.gameObject.SetActive(false);
        owner.decisionBase.gameObject.SetActive(true);
        //mycur card stuff
        enemyCurrentCard.Zoomout(owner.fieldTransform);
        enemyCurrentCard = null;
        Table.ShowAllPlayers();
    }

    void DecisionControl()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = new Vector3(mousePosition.x, mousePosition.y, 2);
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Ray2D pointerRay = new Ray2D(mousePosition, cam.transform.forward);
        RaycastHit2D rh = new RaycastHit2D();

        switch (phase)
        {
            #region Entry Phase

            case PlayerPhase.Entry:
                //check for buttons
                rh = Physics2D.Raycast(pointerRay.origin, pointerRay.direction, 100, buttons);

                if (rh.collider != null)
                {
                    decisionGlow.SetActive(true);
                    decisionGlow.transform.position = rh.collider.transform.position;
                    DecisionButton db = rh.collider.GetComponent<DecisionButton>();

                    if (Input.GetMouseButtonDown(0))
                    {
                        if (db.decisionName.Equals("Attack"))
                        {
                            SwitchPhase(PlayerPhase.ChooseAttack);
                        }
                        else if (db.decisionName.Equals("Quit"))
                        {
                            Debug.Log("Quitting...");
                            Application.Quit();
                        }
                    }
                }
                else
                {
                    decisionGlow.SetActive(false);
                }

                break;

            #endregion

            #region Choose Attack Phase

            case PlayerPhase.ChooseAttack:
                //check for buttons
                rh = Physics2D.Raycast(pointerRay.origin, pointerRay.direction, 100, buttons);

                if (rh.collider != null)
                {
                    AttackButton ab = rh.collider.GetComponent<AttackButton>();
                    DecisionButton db = rh.collider.GetComponent<DecisionButton>();

                    if (ab != null)
                    {
                        decisionGlow.SetActive(false);
                        attackGlow.SetActive(true);
                        
                        attackGlow.transform.position = rh.collider.transform.position;

                        if (Input.GetMouseButtonDown(0))
                        {
                            selectedAttack = ab.attack;
                            SwitchPhase(PlayerPhase.ChooseEnemy);
                        }
                    }
                    else if (db != null)//you went back
                    {
                        decisionGlow.SetActive(true);
                        attackGlow.SetActive(false);
                        
                        decisionGlow.transform.position = rh.collider.transform.position;

                        if (Input.GetMouseButtonDown(0))
                        {
                            selectedAttack = null;
                            SwitchPhase(PlayerPhase.Entry);
                        }
                    }
                }
                else
                {
                    decisionGlow.SetActive(false);
                    attackGlow.SetActive(false);
                }

                break;

            #endregion

            #region Choose Enemy Phase

            case PlayerPhase.ChooseEnemy:
                //check for buttons
                rh = Physics2D.Raycast(pointerRay.origin, pointerRay.direction, 100, cardsAndbuttons);

                if (rh.collider != null)
                {
                    Card card = rh.collider.GetComponent<Card>();
                    DecisionButton db = rh.collider.GetComponent<DecisionButton>();

                    if (card != null && card.Visible)
                    {
                        cardGlow.SetActive(true);
                        decisionGlow.SetActive(false);

                        cardGlow.transform.position = rh.collider.transform.position;

                        if (Input.GetMouseButtonDown(0))
                        {
                            cardToAttack = card;
                            SwitchPhase(PlayerPhase.ConfirmAttack);
                        }
                    }
                    else if (db != null)//you went back
                    {
                        decisionGlow.SetActive(true);
                        cardGlow.SetActive(false);

                        decisionGlow.transform.position = rh.collider.transform.position;

                        if (Input.GetMouseButtonDown(0))
                        {
                            selectedAttack = null;
                            SwitchPhase(PlayerPhase.ChooseAttack);
                        }
                    }
                }
                else
                {
                    decisionGlow.SetActive(false);
                    cardGlow.SetActive(false);
                }

                break;

            #endregion

            #region Confirm Phase

            case PlayerPhase.ConfirmAttack:
                //check for buttons
                rh = Physics2D.Raycast(pointerRay.origin, pointerRay.direction, 100, buttons);

                if (rh.collider != null)
                {
                    DecisionButton db = rh.collider.GetComponent<DecisionButton>();
                    
                    if (db != null)//you went back
                    {
                        decisionGlow.SetActive(true);
                        decisionGlow.transform.position = rh.collider.transform.position;

                        if (Input.GetMouseButtonDown(0))
                        {
                            if (db.decisionName.Equals("Yes"))
                            {
                                SwitchPhase(PlayerPhase.Attacking);
                            }
                            else if(db.decisionName.Equals("No"))
                            {
                                cardToAttack = null;
                                SwitchPhase(PlayerPhase.ChooseEnemy);
                            }
                        }
                    }
                }
                else
                {
                    decisionGlow.SetActive(false);
                }

                break;

                #endregion
        }
    }

    void SwitchPhase(PlayerPhase nextPhase)
    {
        if(phase == PlayerPhase.Entry)
        {
            //we either quit or we clicked on attacks
            if(nextPhase == PlayerPhase.ChooseAttack)
            {
                decisionButtons.gameObject.SetActive(false);
                attackButtons.gameObject.SetActive(true);
                decisionGlow.SetActive(false);
                backButton.SetActive(true);

                //setup buttons
                SetupAttackButtons();
            }
        }
        else if(phase == PlayerPhase.ChooseAttack)
        {
            //we either clicked an attack, or we went back to decisions
            if (nextPhase == PlayerPhase.ChooseEnemy)
            {
                attackButtons.gameObject.SetActive(false);
                attackGlow.SetActive(false);
                chooseEnemy.SetActive(true);
            }
            else if(nextPhase == PlayerPhase.Entry)
            {
                decisionButtons.gameObject.SetActive(true);
                attackButtons.gameObject.SetActive(false);
                backButton.SetActive(false);
            }
        }
        else if (phase == PlayerPhase.ChooseEnemy)
        {
            //we either clicked an attack, or we went back to decisions
            if (nextPhase == PlayerPhase.ConfirmAttack)
            {
                attackButtons.gameObject.SetActive(false);
                cardGlow.SetActive(false);
                backButton.SetActive(false);
                confirmDecision.SetActive(true);
                chooseEnemy.SetActive(false);
            }
            else if (nextPhase == PlayerPhase.ChooseAttack)//we went back
            {
                decisionButtons.gameObject.SetActive(false);
                attackButtons.gameObject.SetActive(true);
                chooseEnemy.SetActive(false);
            }
        }
        else if (phase == PlayerPhase.ConfirmAttack)
        {
            //we either clicked an attack, or we went back to decisions
            if (nextPhase == PlayerPhase.Attacking)
            {
                attackButtons.gameObject.SetActive(false);
                cardGlow.SetActive(false);
                backButton.SetActive(false);
                confirmDecision.SetActive(false);
                SendAttack();
            }
            else if (nextPhase == PlayerPhase.ChooseEnemy)//we went back
            {
                decisionButtons.gameObject.SetActive(false);
                attackButtons.gameObject.SetActive(false);
                confirmDecision.SetActive(false);
                backButton.SetActive(true);
                chooseEnemy.SetActive(true);
            }
        }

        AudioSource.PlayClipAtPoint(clickSound, cam.transform.position);

        phase = nextPhase;  
        decisionGlow.SetActive(false);
    }

    void SendAttack()
    {
        AttackObject attackCopy = GameObject.Instantiate(selectedAttack.attackObject, field.transform.position, field.transform.rotation);
        attackCopy.Setup(field, PlayerID, selectedAttack, cardToAttack);
        PopTextManager.CreateText(Vector3.zero + new Vector3(10f, 0f, 0f),
            string.Format("Player {0} used \n{1}\non Player {2}!", PlayerID, selectedAttack.attackName, cardToAttack.ownerID),
            Color.gray, 53, 120, 110, true);

        AudioSource.PlayClipAtPoint(attackCopy.attackSpawn, cam.transform.position);
    }

    public void DestroyFieldCard()
    {
        if(deck.Count > 0)
        {
            GameObject.Destroy(field.gameObject);
            field = deck[deck.Count - 1];
            deck.RemoveAt(deck.Count - 1);
            field.transform.SetParent(fieldTransform);
            field.Visible = true;
            field.transform.localPosition = Vector3.zero;
            field.SetFrontOrder(49);
            deckCount.text = deck.Count.ToString();
            deckCount.transform.position -= cardDeckOffset;
            attacksText.fontSize = field.HasLongAttackName() ? field.GetLongAttackLength() : attacksTextInitialSize;
        }
        else
        {
            Application.Quit();
        }
    }

    void SetupAttackButtons()
    {
        for (int i = 0; i < attackButtons.childCount; ++i)
        {
            attackButtons.GetChild(i).gameObject.SetActive(false);
        }

        List<Attack> fattacks = field.GetAttacks();
        for(int i = 0; i < fattacks.Count; ++i)
        {
            attackButtons.GetChild(i).gameObject.SetActive(true);
            attackButtons.GetChild(i).GetComponent<AttackButton>().SetupButton(fattacks[i]);
        }
    }

    public void ActivatePhase()
    {
        phase = PlayerPhase.Entry;
        backButton.SetActive(false);
        attackButtons.gameObject.SetActive(false);
        chooseEnemy.SetActive(false);
        confirmDecision.SetActive(false);

        decisionButtons.gameObject.SetActive(true);
        selectedAttack = null;
        cardToAttack = null;
    }
}
