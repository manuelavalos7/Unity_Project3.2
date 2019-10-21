using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.LWRP;

public class playerMovement : MonoBehaviour
{
    private float speed= 10f;//player speed (for movement cooldown)
    [SerializeField] Tilemap tileObstacles;//tilemap of obstacles
    [SerializeField] Tilemap tileBackground;//tilemap of background
    [SerializeField] Tilemap tileForeground;//tilemap of foreground
    [SerializeField] Tilemap tileWalls;//tilemap of walls
    private GameObject[] enemies;//array of all enemies
    [SerializeField] Sprite openChest;//sprite to display when chest opened
    private GameManager gameManager;//gameManager object to update skulls and supers
    
    private GameObject lastSavePoint;//last visited savepoint object
    private Vector2 lastSavePosition; //integer representation on grid of the saveposition
    private Vector2 startPoint;//where the player starts on the map

    private bool levelDone = false;//to check if level completed
    public Vector2 playerPosition = Vector2.zero;//players position on the grid
    private float moveCooldown = 0;//cooldown timer for movement
    private const float coolDown = 1f;//cooldown reset value
    private bool superInUse = false;
    public bool gamePaused = false;//tells player if game paused
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();//set game manager to avoid finding over and over
        playerPosition = transform.position / 0.5f;//player transform is offset because of the grid
        startPoint = transform.position;//restart point if no savepoint 
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!gamePaused)//can only move if game not paused
        {
            if (moveCooldown <= 0 && !levelDone)//if able to move again, and level not completed
            {
                movePlayer();//get player input and move
                moveCooldown = coolDown;//reset cooldown
            }
            moveCooldown -= Time.deltaTime * speed;//subtract from cooldown to move again later
            if (!superInUse && Input.GetKeyDown(KeyCode.Space) && gameManager.supers > 0)
            {//if space pressed and player stil has supers
                gameManager.usedSuper();//keeps track of supers used
                StartCoroutine(FlashScreen());//super move flashes screen lights on for short time

            }
            else if (!superInUse && Input.GetKeyDown(KeyCode.Space) && gameManager.skulls >= 20)
            {//you can use 20 skulls to use a super
                StartCoroutine(FlashScreen());//use super function
                gameManager.skulls -= 21;//subtract 21, but addSkull adds 1
                gameManager.addSkull();//run this to update text
            }
        }

    }
    private void movePlayer() {
        Vector3 playerChange = Vector3.zero;//no change yet
        playerChange.x = Input.GetAxisRaw("Horizontal");//read A D or right/left arrows
        playerChange.y = Input.GetAxisRaw("Vertical");// read vertical movement
        if (playerChange.x != 0 || playerChange.y != 0)//if up or down  or right/left pressed
        {
            if (playerChange.x != 0)//if right/left pressed
            {
                playerChange.y = 0;//set y to 0, no diagonal movement
                playerChange.x = playerChange.x / Mathf.Abs(playerChange.x);//to move one unit in x
            }
            else if (playerChange.y != 0)//if no x movement, but there is y movement
            {
                playerChange.y = playerChange.y / Mathf.Abs(playerChange.y);//to move one unit in y
            }
            if (!tileMapHasObstacle(transform.position.x + playerChange.x * 0.5f, transform.position.y + playerChange.y * 0.5f))//fucntion checks tilemaps to see if player is trying to go into wall or into obstacle
            {
                transform.position += (playerChange * 0.5f);//move player with an offset (playerchange is grid movement coordinates)
                playerPosition += (Vector2)playerChange;//keep track of positin on grid coordinates
            }
            if (playerChange.x != 0)//if player moves on x axis
            {
                transform.localScale = new Vector3(playerChange.x, transform.localScale.y, transform.localScale.z);//flips sprite if player moved on x axis
            }
            enemies = GameObject.FindGameObjectsWithTag("Enemy");//find all enemies
            foreach (GameObject e in enemies) {// fpr each enemy e
                e.GetComponent<enemy>().moveEnemy();// have each enemy make a move
            }
        }
    }

    public bool tileMapHasObstacle(float x, float y) {
        //checks the tilemap at x,y to see if there is an obstacle
  
        TileBase obstacle = tileObstacles.GetTile(tileObstacles.WorldToCell(new Vector3(x,y,0)));//get  obstacle the tile at x, y
        TileBase wall = tileWalls.GetTile(tileObstacles.WorldToCell(new Vector3(x, y, 0)));//get wall obstacle at x,y
        return (wall != null || obstacle != null);//returns true if there is an obstacle (obstacle is not null)
    }


    private void OnTriggerEnter2D(Collider2D collision)//when collides with a collider that is a trigger
    {
        if (collision.tag == "Collect")//if colliding with a collectible
        {
            Destroy(collision.gameObject);//destroy the collectible
            gameManager.addSkull();//add 1 to skulls in gameManager
        }
        else if (collision.tag == "SavePoint")//if entered a savepoint
        {
            lastSavePoint = collision.gameObject;//set the last savepoint to this savepoint
            lastSavePosition = playerPosition; // save the players position to return when dead
            collision.gameObject.GetComponent<torchScript>().toggleLight();//turn on the light on the torch
        }
        else if (collision.tag == "Win")//if collided with chest
        {
            WinLevel(SceneManager.GetActiveScene().buildIndex, collision.gameObject);//run win level function on current scene (passing the chest gameobject)
        }
        else if (collision.tag == "Enemy") {//if collided with enemy
            if (lastSavePoint != null)//if there is a last savepoint
            {
                playerPosition = lastSavePosition;//set the player position to the last save position
                transform.position = lastSavePoint.transform.position;//set the transform to last savepoint's transform
                GetComponent<AudioSource>().Play();//play sound attached to player (extinguish sound)

            }
            else
            {
                transform.position = startPoint;//if there is no savepoint, go to original startPoint
                playerPosition = transform.position / 0.5f;//set player position with offset from grid
            }
        }
    }

    IEnumerator FlashScreen() {
        superInUse = true;//set flag so super is not used more than once at a time
        float elapsed = 0f;//timer for flash

        float original = GetComponentInChildren<Light2D>().pointLightOuterRadius;//find the original light source radius
        while(elapsed < 1f)//while timer less than 1f
        {
            GetComponentInChildren<Light2D>().pointLightOuterRadius += 25*Time.deltaTime;//increase thej radius of light source
            elapsed += Time.deltaTime;//increase timer to escape while eventually
            yield return null;//do update frame and come back here
        }
        elapsed = 0;//reset timer
        while (elapsed < 1f)//while loop to return radious to normal
        {
            GetComponentInChildren<Light2D>().pointLightOuterRadius -= 25 * Time.deltaTime;//decrease radius of light source
            elapsed += Time.deltaTime;//decrease timer
            yield return null;//do upldate frame and come back here
        }
        GetComponentInChildren<Light2D>().pointLightOuterRadius =original;//reset light source radius to original
        superInUse = false;//reset so super can be used
    }

    private void WinLevel(int index, GameObject chest)//win level fucntion when chest reached
    {
        chest.GetComponent<SpriteRenderer>().sprite = openChest;//set chest to appear open
        levelDone = true;//set flag for finished level
        gameManager.saveState();//save progress before starting next level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);//load the next scene
        
    }

    



}


