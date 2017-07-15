using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopTextManager : MonoBehaviour {

    [SerializeField]
    private PopText poptextPrefab;
    public static PopTextManager instance = null;

    private void Awake()
    {
        instance = this;
    }

    public static PopText CreateText(Vector3 position, string text, Color c, int sortingOrder, int aliveTime, int fontSize, bool shrink)
    {
        PopText popInstance = GameObject.Instantiate(instance.poptextPrefab, position, Quaternion.identity);
        popInstance.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        popInstance.GetComponent<TextMesh>().fontSize = fontSize;
        popInstance.GetComponent<TextMesh>().text = text;
        popInstance.Setup(aliveTime, shrink);

        return popInstance;
    }
}
