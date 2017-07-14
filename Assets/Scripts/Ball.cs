using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private ScoreText scoreText;
    public int ballPoint = 1;

    void Start()
    {
        scoreText = GameObject.FindObjectOfType<ScoreText>();
    }

    // Update is called once per frame
    void Update ()
    {
		if(transform.position.y < -17)
        {
            GameObject.Destroy(this.gameObject);
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Bucket>())
        {
            GameObject.Destroy(this.gameObject);
            scoreText.Score += ballPoint;
            scoreText.GetComponent<TextMesh>().text = "Score: " + scoreText.Score;
        }
    }
}
