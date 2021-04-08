using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;


public class GridManager : MonoBehaviour
{
    //prevent non-singleton constructor use

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
    private Stack<GameObject> movementOrbs;
    private int tilesMoved;
    //ui elements for character movement
    [Header("UI Elements for character movement")]
    public GameObject selectedAllyCursor;
    private GameObject selectedAllyCursorInstance;
    public GameObject arrowIndicator;
    private Stack<GameObject> arrowIndicators;
    public TextMeshProUGUI stepsLeft;



    // Start is called before the first frame update
    void Start()
    {
        //instantiation
        movementOrbs = new Stack<GameObject>();
        selectedAlly = null;
        CreateGrid();
        characterSelected = false;
        gamePhase = GamePhase.Player;

        //pooling objects
        selectedAllyCursorInstance = (GameObject)Instantiate(selectedAllyCursor);
        selectedAllyCursorInstance.SetActive(false);
        arrowIndicators = new Stack<GameObject>();


    }
   

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("game" + gamePhase);
        //if there is a touch
        if (Input.GetMouseButtonDown(0))
        {
            //place ur party TODO DOESNT WORK
            /*if(gamePhase == GamePhase.Start)
            {
                
                //Touch touch = Input.GetTouch(0);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                //raycast isnt hitting anything
                //TODO
                Debug.Log("alliesplaced: " + alliesPlaced + "party Count " + party.Count);
                if (Physics.Raycast(ray, out hit) && alliesPlaced <= party.Count)
                {
                    Debug.Log("touch registered: " + gamePhase);
                    GameObject replacedOrb = hit.transform.gameObject;
                    PlaceParty(replacedOrb);
                    alliesPlaced++;
                }
                

                gamePhase = GamePhase.Player;
                
            }*/
            //move ur party
            if (gamePhase == GamePhase.Player)
            {
                //Debug.Log("Player phase");
                //the first tap on screen
                //Touch touch = Input.GetTouch(0);

                TapToSelect(Input.mousePosition);



            }

        }
        
        if (characterSelected)
        {
            if (gamePhase == GamePhase.Player)
            {
                Ally ally = selectedAlly.GetComponent<Ally>();
                if (ally.moveState == Ally.moveStates.Selected)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, Vector2.zero, Mathf.Infinity);
                    //store all the tiles 
                    if (hit)
                    {
                        GameObject move_Orbs = hit.transform.gameObject;
                        if(move_Orbs != lastSelected)
                        {
                            //only build move when the tile ur finger is on is a different one
                            if (isNeighbour(move_Orbs, lastSelected))
                            {
                                buildMove(move_Orbs);
                            }
                        }
                        
                    }
                    
                }
            }

        }
        if (Input.GetMouseButtonUp(0) && movementOrbs.Count > 1)
        {
            if (gamePhase == GamePhase.Player)
            {
                Debug.Log("Move executed");

                ExecuteMove();

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
        List<GameObject> partyCopy = new List<GameObject>();
        partyCopy.AddRange(party);

        //place party members in first row
        for (int i = 0; i < col; i++)
        {
            //hard coded in for now
            //spawning allies
            Vector3 position = new Vector3(i * tileSize + xOffset, 0 * tileSize + yOffset);

            if (i <= 3 && i >= 1)
            {
                GameObject partyMember = partyCopy[Random.Range(0, partyCopy.Count)];
                GameObject newTile = (GameObject)Instantiate(partyMember, position, Quaternion.identity);
                Orb orb = newTile.GetComponent<Orb>();
                partyCopy.Remove(partyMember);
                //Debug.Log("removed party member: " + partyCopy.Count);
                orb.SetPosition(i, 0);
                grid[i, 0] = newTile;

            }
            else
            {
                GameObject randomTile = orbList[Random.Range(0, orbList.Count)];
                GameObject newTile = (GameObject)Instantiate(randomTile, position, Quaternion.identity);
                Orb orb = newTile.GetComponent<Orb>();
                orb.SetPosition(i, 0);
                grid[i, 0] = newTile;

            }


        }

        for (int i = 0; i < row; i++)
        {
            for (int j = 1; j < col; j++)
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

        Vector3 tempPosition = orb.transform.position;
        orb.transform.position = orbSwap.transform.position;
        orbSwap.transform.position = tempPosition;

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

    private void TapToSelect(Vector3 position)
    {
        //Ray ray = Camera.main.ScreenPointToRay(position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, Vector2.zero, Mathf.Infinity);
        
        if (hit)
        {
            Debug.Log("Hit object: " + hit.transform.gameObject.name);

            if (hit.transform.gameObject.CompareTag("Ally"))
            {
                selectedAllyCursorInstance.SetActive(true);

                if (!characterSelected)
                {
                    selectedAlly = hit.transform.gameObject;
                    Ally ally = selectedAlly.GetComponent<Ally>();
                    ally.moveState = Ally.moveStates.Selected;
                    characterSelected = true;
                    //selection ui element on
                    selectedAllyCursorInstance.transform.position = new Vector3(ally.getPosition().x + xOffset, ally.getPosition().y + yOffset);
                    tilesMoved = ally.getMovement();
                    stepsLeft.SetText(ally.getMovement().ToString());
                    
                }
                else if(hit.transform.gameObject != selectedAlly)
                {
                    Ally oldSelection = selectedAlly.GetComponent<Ally>();
                    Ally newSelection = hit.transform.gameObject.GetComponent<Ally>();
                    selectedAlly = hit.transform.gameObject;
                    oldSelection.moveState = Ally.moveStates.Unselected;
                    newSelection.moveState = Ally.moveStates.Selected;
                    //moving ui                     
                    selectedAllyCursorInstance.transform.position = new Vector3(newSelection.getPosition().x + xOffset, newSelection.getPosition().y + yOffset);
                    tilesMoved = newSelection.getMovement();
                    stepsLeft.SetText(newSelection.getMovement().ToString());

                }
                else
                {
                    Debug.Log("deselect");
                    // = 
                }
                lastSelected = selectedAlly;
                movementOrbs.Push(selectedAlly);

            }
        }
    }
    private void buildMove(GameObject move_Orbs)
    {
        //Orb o = move_Orbs.GetComponent<Orb>();

        //if its not already a move
        if (!movementOrbs.Contains(move_Orbs) && tilesMoved > 0)
        {
            Debug.Log("Build move adding: " + move_Orbs.name + "at " + move_Orbs.transform.position);
            GameObject ai = ObjectPooler.Instance.SpawnFromPool("MoveIndicator", move_Orbs.transform.position, Quaternion.identity);
            movementOrbs.Push(move_Orbs);
            tilesMoved -= 1;
            arrowIndicators.Push(ai);
            lastSelected = move_Orbs;

        }
        else if(movementOrbs.Contains(move_Orbs))
        {
            Debug.Log("Build move removing: " + move_Orbs.name + "at " + move_Orbs.transform.position);
            movementOrbs.Pop();
            tilesMoved += 1;
            arrowIndicators.Pop().SetActive(false);
            lastSelected = move_Orbs;
        }
        stepsLeft.SetText(tilesMoved.ToString());

        Debug.Log("tiles moved: " + tilesMoved);

    }
    

    private void ExecuteMove()
    {
        Debug.Log("Executing move of size " + movementOrbs.Count);
        if(movementOrbs.Count <= 2)
        {
            ResetValues();
            return;
        }
        //movementOrbs.
        ReverseStack();
        IEnumerator<GameObject> enumerator = movementOrbs.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Debug.Log("next orb" + enumerator.Current);
            SwapOrbs(selectedAlly, enumerator.Current);
        }
        for(int i = 0; i < movementOrbs.Count; i++)
        {
            movementOrbs.Pop();
            selectedAlly.GetComponent<Ally>().moveState = Ally.moveStates.Wait;
        }
        
        if (AllWaiting())
        {
            gamePhase = GamePhase.Enemy;
        }

        //reset all conditions
        selectedAlly.GetComponent<Ally>().moveState = Ally.moveStates.Wait;
        ResetValues();
        
    }
    private void ReverseStack()
    {
        Stack<GameObject> rev = new Stack<GameObject>();
        while (movementOrbs.Count != 0)
        {
            rev.Push(movementOrbs.Pop());
        }
        movementOrbs = rev;
}
    private void ResetValues()
    {
        lastSelected = null;
        selectedAllyCursorInstance.SetActive(false);
        selectedAlly = null;
        characterSelected = false;
        movementOrbs.Clear();
        foreach(GameObject g in arrowIndicators)
        {
            g.SetActive(false);
        }
        arrowIndicators.Clear();
    }
    private bool isNeighbour(GameObject orb, GameObject neighbour)
    {
        if(neighbour == null)
        {
            return true;
        }
        Orb o = orb.GetComponent<Orb>();
        Orb oNext = neighbour.GetComponent<Orb>();
        if(Mathf.Abs(o.getPosition().x - oNext.getPosition().x) == 1 && o.getPosition().y == oNext.getPosition().y)
        {
            return true;
        }

        else if(Mathf.Abs(o.getPosition().y - oNext.getPosition().y) == 1 && o.getPosition().x == oNext.getPosition().x)
        {
            return true;
        }

        else
        {
            return false;

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
