using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private TableCamera cam;

    private List<Card> deck = new List<Card>();
    private List<Card> hand = new List<Card>();
    private Card field = null;
    [SerializeField]
    private int playerID = 1;

    [SerializeField]
    private LayerMask cards;
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

        if(Input.GetMouseButtonDown(0))
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
            else if(card != null && card.Visible)
            {
                Debug.Log("Not my card");
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
            //else if(currentHighlight != null)
            //{
            //    cam.Zoomout();
            //    currentHighlight.Zoomout(fieldTransform);
            //    currentHighlight = null;
            //    descriptionText.text = "";
            //    attacksText.text = "";
            //    attackDescriptionsText.text = "";
            //    descriptionBase.gameObject.SetActive(false);
            //    decisionBase.gameObject.SetActive(true);
            //}
        }
        else
        {
            cam.Zoomout();

            if (myCurrentCard != null)
            {
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
            else if(enemyCurrentCard != null)
            {
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

    void DestroyFieldCard()
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
}
