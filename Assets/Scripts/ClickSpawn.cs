using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickSpawn : MonoBehaviour
{
    //Only change 
    public GameObject redBallPrefab;
    public GameObject greenBallPrefab;
    public GameObject blueBallPrefab;

    void Update ()
    {
        //gets the position to spawn the balls
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = new Vector3(mousePosition.x, mousePosition.y, 2);
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        //spawn red ball
        if (Input.GetMouseButtonDown(0))
        {
            GameObject.Instantiate(redBallPrefab, mousePosition, Quaternion.identity);
        }
        //spawn green ball
        else if (Input.GetMouseButtonDown(1))
        {
            GameObject.Instantiate(greenBallPrefab, mousePosition, Quaternion.identity);
        }
        //spawn blue ball
        else if (Input.GetMouseButtonDown(2))
        {
            GameObject.Instantiate(blueBallPrefab, mousePosition, Quaternion.identity);
        }
    }
}
