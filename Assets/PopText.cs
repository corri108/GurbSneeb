using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopText : MonoBehaviour {

    int aliveTime = 60;
    int shrinkTime = 20;
    bool shrink = false;
    Vector3 initScale;
    void Awake()
    {
        initScale = this.transform.localScale;
    }
    void FixedUpdate()
    {
        aliveTime--;

        if(shrink && aliveTime < shrinkTime)
        {
            this.transform.localScale = Vector3.Lerp(this.transform.localScale, initScale * 0.2f, 0.1f);
        }

        if(aliveTime == 0)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    public void Setup(int timeAlive, bool shrink)
    {
        this.aliveTime = timeAlive;
        this.shrink = shrink;
    }
}
