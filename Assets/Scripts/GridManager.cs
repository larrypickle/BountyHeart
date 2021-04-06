using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridManager : Singleton<GridManager>
{
    //prevent non-singleton constructor use
    protected GridManager()
    {

    }
    //grid generation
    [Header("Grid Generation")]
    [HideInInspector]
    public GameObject[,] grid;
    public int row, col;
    private Vector2Int gridDimension;
    public float tileSize;
    public float xOffset, yOffset;
    public List<GameObject> orbList;

    //game phases
    [HideInInspector]
    public enum GamePhase
    {
        Start,
        Player,
        Enemy,
    }
    private GamePhase gamePhase;
    //game start placement
    private int alliesPlaced;
    //character movement
    [Header("Character Movement")]
    private bool characterSelected;
    private GameObject selectedAlly;
    private GameObject lastSelected;
    public List<GameObject> party;
    private Queue<GameObject> movementOrbs;
    private int tilesMoved;
    //ui elements for character movement
    [Header("UI Elements for character movement")]
    public GameObject selectedAllyCursor;
    public GameObject arrowIndicator;
    public TextMeshProUGUI stepsLeft;
    


    // Start is called before the first frame update
    void Start()
    {
        
        CreateGrid();
        characterSelected = false;

    }

    // Update is called once per frame
    void Update()
    {
        //if there is a touch
        if (Input.touchCount > 0)
        {
            
            //place ur party
            if(gamePhase == GamePhase.Start)
            {
                Touch touch = Input.GetTouch(0);
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
                while(alliesPlaced < party.Count)
                {
                    if (touch.phase == TouchPhase.Began && Physics.Raycast(ray, out hit))
                    {
                        Debug.Log("touch registered: " + gamePhase);
                        GameObject replacedOrb = hit.transform.gameObject;
                        PlaceParty(replacedOrb);
                    }
                }

                gamePhase = GamePhase.Player;
                
            }
            //move ur party
            else if(gamePhase == GamePhase.Player)
            {
                //the first tap on screen
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    TapToSelect(touch);
                }
                if (touch.phase == TouchPhase.Moved)
                {
                    Ally ally = selectedAlly.GetComponent<Ally>();
                    tilesMoved = ally.getMovement();
                    if (ally.moveState == Ally.moveStates.Selected)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(touch.position);
                        RaycastHit hit;
                        //store all the tiles 
                        while (tilesMoved > 0)
                        {
                            if (Physics.Raycast(ray, out hit))
                            {
                                GameObject move_Orbs = hit.transform.gameObject;
                                //only build move when the tile ur finger is on is a different one
                                if (move_Orbs != lastSelected)
                                {
                                    buildMove(move_Orbs);
                                }
                            }

                        }
                    }
                }
                if (touch.phase == TouchPhase.Ended)
                {
                    ExecuteMove();
                    
                }
            }
           
        }
    }
    //[row][col] = [y][x]
    public void CreateGrid()
    {
        grid = new GameObject[row, col];

        //how to prevent repeating tiles
        GameObject[] previousLeft = new GameObject[col];
        GameObject previousBelow = null;


        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                //used in order to make sure there are no combos when the board is being generated
                List<GameObject> possibleOrbs = new List<GameObject>();
                possibleOrbs.AddRange(orbList);
                possibleOrbs.Remove(previousLeft[j]);
                possibleOrbs.Remove(previousBelow);


                GameObject removedObject = possibleOrbs[Random.Range(0, possibleOrbs.Count)];
                Vector3 position = new Vector3(i * tileSize + xOffset, j * tileSize + yOffset);
                GameObject newTile = (GameObject)Instantiate(removedObject, position, Quaternion.identity);
                Orb orb = newTile.GetComponent<Orb>();
                orb.SetPosition(i, j);

                //new position
                grid[i, j] = newTile;
                previousLeft[j] = removedObject;
                previousBelow = removedObject;
            }
        }
    }

    public void PlaceParty(GameObject replace)
    {
        Debug.Log("party placed: " + gamePhase);

        Orb replaceOrb = replace.GetComponent<Orb>();
        Vector2Int position = replaceOrb.getPosition();
        //turn the replaced orb into ally

        grid[position.x, position.y] = party[alliesPlaced];
        party[alliesPlaced].GetComponent<Orb>().SetPosition(position.x, position.y);
        //spawn the ally
        Instantiate(party[alliesPlaced], new Vector3(position.x * tileSize + xOffset, position.y * tileSize + yOffset), Quaternion.identity);
        //unspawn the orb on that square
        replace.SetActive(false);

        alliesPlaced++;
    }

    public void SwapOrbs(GameObject orb, GameObject orbSwap)
    {
        Orb o = orb.GetComponent<Orb>();
        Orb oSwap = orbSwap.GetComponent<Orb>();

        Vector2Int position = o.getPosition();
        //Orb temp = o;
        Vector2Int swapPosition = oSwap.getPosition();

        grid[position.x, position.y] = orb;
        grid[swapPosition.x, swapPosition.y] = orbSwap;
        oSwap.SetPosition(position.x, position.y);
        o.SetPosition(swapPosition.x, swapPosition.y);

        CheckMatch(oSwap);


    }

    public void CheckSides(Orb o)
    {
        Vector2Int position = o.getPosition();
        //grid[position.x, position.y]
    }

    private void CheckMatch(Orb orb)
    {

    }

    private void TapToSelect(Touch touch)
    {
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.CompareTag("Ally"))
            {
                if (!characterSelected)
                {
                    selectedAlly = hit.transform.gameObject;
                    Ally ally = hit.transform.gameObject.GetComponent<Ally>();
                    ally.moveState = Ally.moveStates.Selected;
                    characterSelected = true;

                }
                else
                {
                    Ally oldSelection = selectedAlly.GetComponent<Ally>();
                    Ally newSelection = hit.transform.gameObject.GetComponent<Ally>();
                    oldSelection.moveState = Ally.moveStates.Unselected;
                    newSelection.moveState = Ally.moveStates.Selected;
                }
            }
        }
    }
    private void buildMove(GameObject move_Orbs)
    {
        Orb o = move_Orbs.GetComponent<Orb>();

        //if its already a move
        if (!movementOrbs.Contains(move_Orbs))
        {
            movementOrbs.Enqueue(move_Orbs);
            tilesMoved -= 1;
        }
        else
        {
            movementOrbs.Dequeue();
            tilesMoved += 1;
        }
        stepsLeft.SetText(tilesMoved.ToString());
    }

    private void ExecuteMove()
    {
        IEnumerator<GameObject> enumerator = movementOrbs.GetEnumerator();
        while (enumerator.MoveNext())
        {
            SwapOrbs(selectedAlly, enumerator.Current);
        }
        for(int i = 0; i < movementOrbs.Count; i++)
        {
            movementOrbs.Dequeue();
            selectedAlly.GetComponent<Ally>().moveState = Ally.moveStates.Wait;
        }
        if (AllWaiting())
        {
            gamePhase = GamePhase.Enemy;
        }
    }
    
    //TODO
    private bool AllWaiting()
    {
        return false;
    }

    private void DisplayMaxSteps(Ally ally)
    {
        stepsLeft.SetText(ally.getMovement().ToString());
    }
    
}
