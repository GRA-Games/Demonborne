using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flying_enemy : MonoBehaviour
{
    
    public float speed;
    public bool chase=false;
    private GameObject player;
    public Transform startingPoint;
 
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) {
            return;
        }
        if (chase == true)
        {
            Chase();
        }
        else
            returnStartpoint();
                
        flip();

    }
    public void Chase()
    {
        transform.position=Vector2.MoveTowards(transform.position,player.transform.position,speed*Time.deltaTime);
        if(Vector2.Distance(transform.position,player.transform.position)<= 0.5f)
        { 
        
        }
    }
    private void returnStartpoint()
    {
        transform.position = Vector2.MoveTowards(transform.position, startingPoint.position, speed * Time.deltaTime);

    }

    private void flip() {
        if (transform.position.x > player.transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }


    }

}
