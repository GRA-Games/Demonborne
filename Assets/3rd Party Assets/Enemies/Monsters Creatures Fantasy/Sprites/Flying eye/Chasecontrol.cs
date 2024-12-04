using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chasecontrol : MonoBehaviour
{
    public Flying_enemy[] enemyArray;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")){

            foreach (Flying_enemy enemy in enemyArray)
            {
                enemy.chase = true;
            }

        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            foreach (Flying_enemy enemy in enemyArray)
            {
                enemy.chase = false;
            }

        }
    }
        // Start is called before the first frame update
        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
