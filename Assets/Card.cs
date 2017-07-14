using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {

    //assignable
    [SerializeField]
    private string cardName = "SNEEB CARD";
    [SerializeField]
    private int HP = 50;
    [SerializeField]
    private Sprite picture;
    [SerializeField]
    private int cardLevel = 1;
    [SerializeField]
    private string description;
    [SerializeField]
    private List<Attack> attacks = new List<Attack>();
    [SerializeField]
    private int longAttackNameLength = -1;

    //hiddens
    [HideInInspector]
    public bool active = false;
    [HideInInspector]
    public int ownerID = -1;

    //privates
    private SpriteRenderer stats;
    private SpriteRenderer front;
    private SpriteRenderer back;
    private SpriteRenderer outline;
    private bool visible = false;
    private bool isZooming = false;
    private Transform goTo;
    private float lerpSpeed = 0.1f;

    public bool Visible
    {
        get { return visible; }
        set
        {
            visible = value;

            if(visible)
            {
                front.enabled = true;
                back.enabled = false;
                stats.gameObject.SetActive(true);
            }
            else
            {
                front.enabled = false;
                back.enabled = true;
                stats.gameObject.SetActive(false);
            }
        }
    }
	// Use this for initialization
	void Awake ()
    {
        cardName = cardName.ToUpper();
        front = this.transform.FindChild("Front").GetComponent<SpriteRenderer>();
        back = this.transform.FindChild("Back").GetComponent<SpriteRenderer>();
        outline = this.transform.FindChild("Outline").GetComponent<SpriteRenderer>();
        stats = this.transform.FindChild("Stats").GetComponent<SpriteRenderer>();

        AdjustLengthFromName();
        stats.transform.GetChild(0).GetComponent<TextMesh>().text = cardName;
        stats.transform.GetChild(1).GetComponent<TextMesh>().text = HP.ToString();
        stats.transform.GetChild(0).GetComponent<TextMesh>().GetComponent<MeshRenderer>().sortingOrder = 100;
        stats.transform.GetChild(1).GetComponent<TextMesh>().GetComponent<MeshRenderer>().sortingOrder = 100;

        front.sprite = picture;

        Visible = false;
        goTo = this.transform;
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(isZooming)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, goTo.position, lerpSpeed);
            this.transform.localScale = Vector3.Lerp(this.transform.localScale, goTo.localScale, lerpSpeed);
        }
        else
        {
            this.transform.position = Vector3.Lerp(this.transform.position, goTo.position, lerpSpeed);
            this.transform.localScale = Vector3.Lerp(this.transform.localScale, goTo.localScale, lerpSpeed);
        }
	}

    void AdjustLengthFromName()
    {
        if (cardName.Length > 17)
        {
            if (cardName[12] != ' ')
                cardName = cardName.Insert(12, "-\n");
            else
            {
                cardName = cardName.Insert(13, "\n");
            }

        }
        else if (cardName.Length > 13)
        {
            stats.transform.GetChild(0).GetComponent<TextMesh>().fontSize -= 10;
        }
    }

    public void Zoom(Transform zoomTo)
    {
        if(!isZooming)
        {
            ShiftLayers(true);
        }

        isZooming = true;
        this.goTo = zoomTo;
    }

    public void Zoomout(Transform backTo)
    {
        if (isZooming)
        {
            ShiftLayers(false);
        }

        isZooming = false;
        this.goTo = backTo;
    }

    void ShiftLayers(bool shiftUp)
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder += shiftUp ? 2 : -2;
        }

        //stats.transform.GetChild(0).GetComponent<TextMesh>().GetComponent<MeshRenderer>().sortingOrder = 100;
        //stats.transform.GetChild(1).GetComponent<TextMesh>().GetComponent<MeshRenderer>().sortingOrder = 100;
    }

    public void SetBackOrder(int order)
    {
        back.sortingOrder = order;
        outline.sortingOrder = order;
        stats.sortingOrder = order + 1;
    }

    public void SetFrontOrder(int order)
    {
        front.sortingOrder = order;
        outline.sortingOrder = order;
        stats.sortingOrder = order + 1;
    }

    public string GetDescription()
    {
        string desc = description;
        int loop = description.Length / 24;

        for (int i = 0; i < loop; ++i)
        {
            desc = desc.Insert(24 * (i + 1), "\n");
        }

        return desc;
    }

    public string GetAttacksString()
    {
        string attackString = "";

        foreach(var a in attacks)
        {
            attackString += a.GetAttackName().ToUpper() + "\n";
        }

        return attackString;
    }

    public string GetAttackDescriptionsString()
    {
        string attackString = "";

        foreach (var a in attacks)
        {
            attackString += a.GetAttackDescription() + "\n\n";
        }

        return attackString;
    }

    public bool HasLongAttackName()
    {
        return longAttackNameLength != -1;
    }

    public int GetLongAttackLength()
    {
        return longAttackNameLength;
    }
}
