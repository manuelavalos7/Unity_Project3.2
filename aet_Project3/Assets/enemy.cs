using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    private GameObject player;//refrence to the player
    private playerMovement playerMove;//refrence to the player's script
    private Vector2 enemyPosition;//posititon of the enemy (in grid coordinates)

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");//set refrence of player
        playerMove = player.GetComponent<playerMovement>();//set refrence of player's script
        enemyPosition = transform.position / 0.25f;//set enemies grid coordinates based on offset
    }


    public void moveEnemy() {//enemy only moves when player moves
        if (Vector3.Distance(transform.position, player.transform.position) > 3) {// only moves if player is 3 unites away or less
            return;
        }
        Vector3 change = Vector3.zero;//inital move is nothing
        int rand = Random.Range(-1, 2);//rand is either -1 or 0 oe 1
        if (rand < 0)//if -1
        {
            change = bestChange();//do a smart move(calculates best option to move towards player)
        }
        else//do a random move ( to lessen difficulty)
        {
            change.x = Random.Range(-1, 2);//change is the next move the enemy will do
            change.y = Random.Range(-1, 2);//both of these return -1 or 0 or 1
            if (change.x != 0)//if moving on x
            {
                change.y = 0;//do not move on y
                change.x = change.x / Mathf.Abs(change.x);//to move one unit in x
            }
            else if (change.y != 0)//if only moving on y
            {
                change.y = change.y / Mathf.Abs(change.y);//to move one unit in y
            }
        }


        if (!playerMove.tileMapHasObstacle(transform.position.x + change.x * 0.5f, transform.position.y + change.y * 0.5f))//check if this is a valid move
        {
          
            transform.position += (change * 0.5f);//change position with some offset
            enemyPosition += (Vector2)change;//keep track  of grid coordinates
            GetComponent<AudioSource>().Play();
        }
        if (change.x != 0)//if moved on x axis
        {
            transform.localScale = new Vector3(-change.x, transform.localScale.y, transform.localScale.z);//flips sprite if enemy moved on x axis
        }

       
    }

    private Vector3 bestChange()
    {//calulates which direct up, right, down, or left is closer to the player
        Vector3 best = Vector2.zero;//best move is inistially 0
        for (int x = -1; x < 2; x++)//-1, 0, 1
        {
            Vector3 test = new Vector3(x, 0, 0);//test each option for x axis movement
            if (Vector3.Distance(test+transform.position, player.transform.position) < Vector3.Distance(best + transform.position, player.transform.position)) {
                best = test;//if distance to old best is larger than new distance then set the test to the best
            }
            test = new Vector3(0, x, 0);//test each option for y axis movement
            if (Vector3.Distance(test + transform.position, player.transform.position) < Vector3.Distance(best + transform.position, player.transform.position))
            {
                best = test; //if distance to old best is larger than new distance then set the test to the best
            }

        }
        return best;//return the best move that was found
    }
}

