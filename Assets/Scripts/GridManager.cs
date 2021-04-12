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
    public float moveDelay;
    public float transitionSpeed;
    private GameObject selectedAlly;
    private GameObject lastSelected;
    public List<GameObject> party;
    private List<GameObject> partyInstance;
    private Stack<GameObject> movementOrbs;
    private int tilesMoved;
    private bool moving;
    //ui elements for character movement
    [Header("UI Elements for character movement")]
    public GameObject selectedAllyCursor;
    private GameObject selectedAllyCursorInstance;
    public GameObject arrowIndicator;
    private Stack<GameObject> arrowIndicators;
    public TextMeshPro stepsLeft;
    private int numberWaiting;

    [Header("Messy Implementation for Orbs falling")]
    public GameObject emptyOrb;

    [Header("Enemy Attack UI")]
    private GameObject enemyIndicator;
    private List<GameObject> enemyAttacks;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Game starting");
        //instantiation
        numberWaiting = 0;
        moving = false;
        movementOrbs = new Stack<GameObject>();
        partyInstance = new List<GameObject>();
        selectedAlly = null;
        enemyIndicator = null;
        CreateGrid();
        characterSelected = false;
        //EnemyPhase();
        gamePhase = GamePhase.Player;
        enemyAttacks = new List<GameObject>();

        //pooling objects
        selectedAllyCursorInstance = (GameObject)Instantiate(selectedAllyCursor);
        selectedAllyCursorInstance.SetActive(false);
        arrowIndicators = new Stack<GameObject>();


    }
    //Order of a move
    //build move -> execute move -> check match -> fillholes -> wait

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("game" + gamePhase);
        //if there is a touch
        if (!moving)
        {
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
                if (gamePhase == GamePhase.Player && movementOrbs.Count <= 1)
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
                            if (move_Orbs != lastSelected)
                            {
                                //only build move when the tile ur finger is on is a different one
                                if (movementOrbs.Peek() != null)
                                {
                                    if (isNeighbour(move_Orbs, movementOrbs.Peek()))
                                    {
                                        buildMove(move_Orbs);
                                    }
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

                    StartCoroutine(ExecuteMove());
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
                partyInstance.Add(newTile);
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
        Debug.Log("Grid created");
    }
    //may implement later if we want to be able to choose where to place party members
    /*public void PlaceParty(GameObject replace)
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

    }*/

    public IEnumerator SwapOrbs(GameObject orb, GameObject orbSwap)
    {
        Orb o = orb.GetComponent<Orb>();
        Orb oSwap = orbSwap.GetComponent<Orb>();

        Vector2Int position = o.getPosition();
        //Orb temp = o;
        Vector2Int swapPosition = oSwap.getPosition();

        grid[position.x, position.y] = orbSwap;
        grid[swapPosition.x, swapPosition.y] = orb;
        Debug.Log("Swapping orbs | before swap | orb 1 " + orb.name + "position: " + o.getPosition() + " orb 2 " + orbSwap.name + "position: " + oSwap.getPosition());

        oSwap.SetPosition(position.x, position.y);
        o.SetPosition(swapPosition.x, swapPosition.y);

        Vector3 tempPosition = orb.transform.position;
        //orb.transform.position = orbSwap.transform.position;
        StartCoroutine(MoveObject(orb, orb.transform.position, orbSwap.transform.position, moveDelay));
        StartCoroutine(MoveObject(orbSwap, orbSwap.transform.position, tempPosition, moveDelay));

        //orbSwap.transform.position = tempPosition;
        Debug.Log("Swapping orbs | after swap | orb 1 " + orb.name + "position: " + o.getPosition() + " orb 2 " + orbSwap.name + "position: " + oSwap.getPosition());
        //CheckMatch();
        yield return null;



    }

    IEnumerator MoveObject(GameObject obj, Vector3 source, Vector3 target, float overTime)
    {
        float startTime = Time.time;
        while (Time.time < startTime + overTime)
        {
            obj.transform.position = Vector3.Lerp(source, target, (Time.time - startTime) / overTime);
            yield return null;
        }
        obj.transform.position = target;
    }
    private bool CheckMatch()
    {
        bool matchFound = false;
        Debug.Log("Checking match");
        HashSet<GameObject> matchedTiles = new HashSet<GameObject>();
        for(int i = 0; i < row; i++)
        {
            for(int j = 0; j < col; j++)
            {

                Orb o = grid[i, j].GetComponent<Orb>();
                Orb.OrbType type = o.orbType;
                if(o.orbType == Orb.OrbType.Ally)
                {
                    continue;
                }
                List<GameObject> horizontalMatches = FindColumnMatchForTile(i, j, type);
                if(horizontalMatches.Count >= 2)
                {
                    matchedTiles.UnionWith(horizontalMatches);
                    //Debug.Log("Adding into match: " + grid[i, j].name + " at " + grid[i, j].transform.position + "at " + i + "," + j);
                    matchedTiles.Add(grid[i, j]);
                    matchFound = true;
                }
                List<GameObject> verticalMatches = FindRowMatchForTile(i, j, type);
                if(verticalMatches.Count >= 2)
                {
                    matchedTiles.UnionWith(verticalMatches);
                    //Debug.Log("Adding into match: " + grid[i, j].name + " at " + grid[i, j].transform.position + "at " + i + "," + j);
                    matchedTiles.Add(grid[i, j]);
                    matchFound = true;
                }

            }
        }
        
        foreach(GameObject g in matchedTiles)
        {
            Orb o = g.GetComponent<Orb>();
            
            //Debug.Log("Destroying object: " + g.name + " at " + g.transform.position);
            Vector2Int pos = o.getPosition();

            //create empty orb with position that is same as grid that was destroyed
            grid[pos.x, pos.y] = emptyOrb;
            grid[pos.x, pos.y].GetComponent<Orb>().SetPosition(pos.x, pos.y);

            //different behaviors based on what was matched
            switch (o.orbType)
            {
                case Orb.OrbType.Attack:
                    Attack();
                    break;
                case Orb.OrbType.Enemy:
                    enemyAttacks.Remove(g);
                    break;
                case Orb.OrbType.Heal:
                    Heal();
                    break;
                case Orb.OrbType.Talk:
                    Talk();
                    break;

            }
                
            Destroy(g);
        }
        return matchFound;
    }
    private IEnumerator Attack()
    {
        Debug.Log("Attack initiated");
        yield return null;
    }
    private IEnumerator Heal()
    {
        Debug.Log("Heal initiated");
        yield return null;
    }
    private IEnumerator Talk()
    {
        Debug.Log("Talk initiated");
        yield return null;
    }


    List<GameObject> FindColumnMatchForTile(int x, int y, Orb.OrbType type)
    {
        List<GameObject> matches = new List<GameObject>();
        for(int i = x + 1; i < col; i++)
        {
            GameObject nextColumn = grid[i, y];
            //Debug.Log("Horizontal match working: " + nextColumn.name + " orbtype: " + nextColumn.GetComponent<Orb>().orbType + "position: " + i + ", " + y + "match count: " + matches.Count);

            if (nextColumn.GetComponent<Orb>().orbType != type)
            {
                break;
            }
            matches.Add(nextColumn);
            //Debug.Log("Next horizontal matching: " + nextColumn.name + " orbtype: " + nextColumn.GetComponent<Orb>().orbType + "position: " + i + ", " + y + "match count: " + matches.Count);

        }
        return matches;
    }

    List<GameObject> FindRowMatchForTile(int x, int y, Orb.OrbType type)
    {
        List<GameObject> matches = new List<GameObject>();
        for (int i = y + 1; i < row; i++)
        {
            GameObject nextRow = grid[x, i];
            //Debug.Log("Vertical match working: " + nextRow.name + " orbType: " + nextRow.GetComponent<Orb>().orbType + "position: " + x + ", " + i + "match count: " + matches.Count);

            if (nextRow.GetComponent<Orb>().orbType != type)
            {
                break;
            }

            matches.Add(nextRow);
            //Debug.Log("Next vertical matching: " + nextRow.name + " orbType: " + nextRow.GetComponent<Orb>().orbType + "position: " + x + ", " + i + "match count: " + matches.Count);

        }
        return matches;
    }


    private IEnumerator FillHoles()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                Debug.Log("grid space: " + i + "," + j + ": " + grid[i, j]);
            }
        }
        Debug.Log("Filling holes");
        for(int i = 0; i < row; i++)
        {
            for(int j = 0; j < col; j++)
            {
                if (grid[i,j] == emptyOrb)
                {
                    Debug.Log("Empty space detected: " + i + ", " + j);
                    yield return Skyfall(i, j);
                    break;
                    

                }
            }
        }
        //StartCoroutine(FillRemaining());

        //FillRemaining();
    }
    private IEnumerator Skyfall(int x, int yStart)
    {

        int nullCount = 0;
        List<GameObject> orbs = new List<GameObject>();
        for (int y = yStart; y < col - 1; y++)
        {
            //yield return new WaitForSeconds(moveDelay);
            //Debug.Log("Object skyfalling: " + grid[i, j + 1].name + "position: " + grid[i, j + 1].transform.position);
            //Debug.Log("Why does ally not work: " + falling.name);
            //GameObject empty = grid[i, j];
            //TODO BUGGED

            if (grid[x, y] == emptyOrb)
            {
                Debug.Log("Null count incremented at " + x + "," + y + " with object " + grid[x, y].name);
                nullCount++;
            }
            //orbs.Add(grid[x, y]);
        }
        //PLEASE HELP HOLY SHIT
        for (int i = 0; i < nullCount; i++)
        {
            for (int y = yStart; y < col - 1; y++)
            {
                GameObject falling = grid[x, y + 1];
                Vector3 dropDown = falling.transform.position - new Vector3(0, tileSize, 0);
                grid[x, y + 1] = emptyOrb;
                falling.GetComponent<Orb>().SetPosition(x, y);
                StartCoroutine(MoveObject(falling, falling.transform.position, dropDown, moveDelay));
                grid[x, y] = falling;
                yield return new WaitForSeconds(moveDelay);

            }
            /*for (int j = 0; j < orbs.Count - 1; j++)
            {
                //initial declarations
                GameObject tempSwap = orbs[j + 1];
                Orb tempO = orbs[j].GetComponent<Orb>();
                Orb swapO = orbs[j + 1].GetComponent<Orb>();
                Vector2Int tempOPos = tempO.getPosition();
                Vector2Int swapOPos = swapO.getPosition();

                //shallow copy - orbs[j] is now same as orbs[j + 1] but wants to be orb[j+1] at position of orbs[j];
                //move the upper object downwards
                Vector3 dropDown = orbs[j + 1].transform.position - new Vector3(0, tileSize, 0);
                Debug.Log("Preparing to move object orbs[" + j+1 + "] : "  + orbs[j + 1].name + " to " + dropDown);
                StartCoroutine(MoveObject(orbs[j + 1], orbs[j + 1].transform.position, dropDown, 0.2f));
                //have the index in the array orbs contain the info about the its orbs[j+1]
                orbs[j] = orbs[j + 1];
                //set the new position
                orbs[j].GetComponent<Orb>().SetPosition(tempO.getPosition().x, tempO.getPosition().y);
                //orbs[j].transform.position = Vector3.Lerp(transform.position, temp.transform.position, Time.deltaTime * transitionSpeed);
                //update the actual grid
                grid[tempOPos.x, tempOPos.y] = orbs[j];

                //orbs[j + 1].SetActive(false);
                //set the swapping orb to empty cuz the original orb was empty
                orbs[j + 1] = emptyOrb;
                //set the empty orbs transform to the old transform or the now empty orb which is x,y+1
                orbs[j + 1].transform.position = tempSwap.transform.position;
                //update the actual grid
                grid[swapOPos.x, swapOPos.y] = orbs[j + 1];
            }
            yield return new WaitForSeconds(moveDelay);

        }*/

            /*GameObject falling = grid[i, j + 1];
            grid[i, j + 1] = emptyOrb;
            falling.transform.position = new Vector2(i * tileSize + xOffset, j * tileSize + yOffset);
            falling.GetComponent<Orb>().SetPosition(i, j);
            grid[i, j] = falling;
            Debug.Log("grid[i,j]: " + grid[i, j].name);
            */

            //grid[i, j + 1] = emptyOrb;
        }

    }

    private IEnumerator FillRemaining()
    {
        yield return FillHoles();
        List<GameObject> orbTemp = new List<GameObject>();
        orbTemp.AddRange(orbList);

        for(int x = 0; x < row; x++)
        {
            for (int y = 0; y < col; y++)
            {
                if (grid[x, y] == emptyOrb)
                {
                    yield return new WaitForSeconds(moveDelay);
                    //choose a random orb from orblist 
                    GameObject random = orbTemp[Random.Range(0, orbTemp.Count)];
                    Vector3 position = new Vector3(x * tileSize + xOffset, y * tileSize + yOffset);
                    GameObject newTile = (GameObject)Instantiate(random, position, Quaternion.identity);
                    Orb orb = newTile.GetComponent<Orb>();
                    orb.SetPosition(x, y);
                    //remove the orb from the orbtemp list to ensure variety between spawned orbs
                    orbTemp.Remove(random);
                    //refill if no orbs left
                    if(orbTemp.Count == 0)
                    {
                        orbTemp.AddRange(orbList);
                    }
                    //new position
                    grid[x, y] = newTile;

                }
            }
            
        }

    }

    private void TapToSelect(Vector3 position)
    {
        //Ray ray = Camera.main.ScreenPointToRay(position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, Vector2.zero, Mathf.Infinity);
        
        if (hit)
        {
            Debug.Log("Hit object: " + hit.transform.gameObject.name + "Grid[i,j]: " + hit.transform.gameObject.GetComponent<Orb>().getPosition() + " Actual Transform.position: " + hit.transform.position + "Orb type: " + hit.transform.gameObject.GetComponent<Orb>().orbType);

            if (hit.transform.gameObject.CompareTag("Ally"))
            {
                
                selectedAlly = hit.transform.gameObject;
                Ally ally = selectedAlly.GetComponent<Ally>();
                Debug.Log("Ally selected: ally movestate - " + ally.moveState);
                if (!(ally.moveState == Ally.moveStates.Wait))
                {
                    selectedAllyCursorInstance.SetActive(true);
                    if (!characterSelected)
                    {
                        Debug.Log("Selected ally after no characters were selected.");
                        ally.moveState = Ally.moveStates.Selected;
                        characterSelected = true;
                        //selection ui element on
                        selectedAllyCursorInstance.transform.position = new Vector3(ally.getPosition().x + xOffset, ally.getPosition().y + yOffset);
                        tilesMoved = ally.getMovement();
                        stepsLeft.transform.position = ally.transform.position + new Vector3(0, 0.5f, 0);
                        stepsLeft.transform.SetParent(hit.transform);
                        stepsLeft.SetText(ally.getMovement().ToString());
                        movementOrbs.Push(selectedAlly);

                    }
                    else if (hit.transform.gameObject != selectedAlly)
                    {
                        Debug.Log("Selected new ally");
                        Ally oldSelection = selectedAlly.GetComponent<Ally>();
                        Ally newSelection = hit.transform.gameObject.GetComponent<Ally>();
                        selectedAlly = hit.transform.gameObject;
                        oldSelection.moveState = Ally.moveStates.Unselected;
                        newSelection.moveState = Ally.moveStates.Selected;
                        //moving ui                     
                        selectedAllyCursorInstance.transform.position = new Vector3(newSelection.getPosition().x + xOffset, newSelection.getPosition().y + yOffset);
                        tilesMoved = newSelection.getMovement();
                        stepsLeft.transform.position = ally.transform.position + new Vector3(0, 0.5f, 0);
                        stepsLeft.transform.SetParent(hit.transform);
                        stepsLeft.SetText(newSelection.getMovement().ToString());
                        movementOrbs.Pop();
                        movementOrbs.Push(selectedAlly);

                    }
                    else
                    {
                        //selectedAlly = null;
                        movementOrbs.Pop();
                        Debug.Log("Deselect");
                        characterSelected = false;
                        stepsLeft.SetText("");
                        selectedAllyCursorInstance.SetActive(false);
                    }
                    lastSelected = selectedAlly;
                }

            }
            else if (hit.transform.gameObject.CompareTag("Enemy"))
            {
                //if enemy was last selected
                if(hit.transform.gameObject == lastSelected)
                {
                    Debug.Log("Selecting enemy that was already selected, not showing enemy attack range.");
                    enemyIndicator.SetActive(false);
                    lastSelected = null;
                }
                else
                {
                    Debug.Log("Selected enemy - showing enemy attack range");
                    enemyIndicator = ObjectPooler.Instance.SpawnFromPool("EnemyIndicator", hit.transform.position, Quaternion.identity);
                    lastSelected = hit.transform.gameObject;
                }
                

            }
        }
    }
    private void buildMove(GameObject move_Orbs)
    {
        //CHANGE THIS 
        //should add positions to a list and then go thru that list of positions to change rather than going thru the actual gameobjects
        //Orb o = move_Orbs.GetComponent<Orb>();

        //if its not already a move
        if (!movementOrbs.Contains(move_Orbs) && tilesMoved > 0)
        {
            //to calculate what the rotation of the arrow should be
            GameObject lastMoveOrb = movementOrbs.Peek();
            Vector3 difference = (move_Orbs.transform.position - lastMoveOrb.transform.position).normalized;
            Debug.Log("difference: " + difference);
            Vector3 rotation = new Vector3(0, 0, 0);
            if(difference.x == -1)
            {
                rotation = new Vector3(0, 0, 90);
            }
            else if (difference.y == -1)
            {
                rotation = new Vector3(0, 0, 180);
            }
            else if(difference.x == 1)
            {
                rotation = new Vector3(0, 0, 270);
            }
            
            
            Debug.Log("Build move adding: " + move_Orbs.name + "at " + move_Orbs.transform.position);
            GameObject ai = ObjectPooler.Instance.SpawnFromPool("MoveIndicator", move_Orbs.transform.position, Quaternion.Euler(rotation));

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

        /*else if(move_Orbs == selectedAlly && tilesMoved > 0)
        {
            if(movementOrbs.Count < 3)
            {
                Debug.Log("Build move removing: " + move_Orbs.name + "at " + move_Orbs.transform.position);
                movementOrbs.Pop();
                tilesMoved += 1;
                arrowIndicators.Pop().SetActive(false);
                lastSelected = move_Orbs;
            }
            else
            {
                //this is to fix the bug where you cannot move overlapping the selected character which u should be able to accomplish
                Debug.Log("Build move adding: " + move_Orbs.name + "at " + move_Orbs.transform.position);
                GameObject ai = ObjectPooler.Instance.SpawnFromPool("MoveIndicator", move_Orbs.transform.position, Quaternion.identity);
                movementOrbs.Push(move_Orbs);
                tilesMoved -= 1;
                arrowIndicators.Push(ai);
                lastSelected = move_Orbs;
            }
        }*/
        
        stepsLeft.SetText(tilesMoved.ToString());

        Debug.Log("tiles moved: " + tilesMoved);

    }

    private IEnumerator EnemyPhase()
    {
        Debug.Log("Enemy phase started");
        gamePhase = GamePhase.Enemy;

        int count = 0;
        while(count < 3)
        {
            int randomX = Random.Range(0, 4);
            int randomY = Random.Range(0, 4);
            if (!grid[randomX, randomY].CompareTag("Ally") && !grid[randomX, randomY].CompareTag("Enemy"))
            {
                count++;
                Debug.Log("Grid " + randomX + ", " + randomY + " became enemy orb");
                grid[randomX, randomY].GetComponent<Orb>().BecomeEnemy();
                GameObject enemy = grid[randomX, randomY];
                enemyAttacks.Add(enemy);
                yield return new WaitForSeconds(moveDelay);
            }
        }
        foreach(GameObject g in enemyAttacks)
        {
            Debug.Log("Enemy Attack initiating");
            yield return EnemyAttack(g);

        }
        StartPlayerPhase();
        
        
    }
    private IEnumerator EnemyAttack(GameObject enemy)
    {
        Orb o = enemy.GetComponent<Orb>();
        Vector2Int pos = o.getPosition();
        //check neighbors
        if(pos.x-1 >= 0)
        {
            if (grid[pos.x - 1, pos.y].CompareTag("Ally"))
            {
                grid[pos.x - 1, pos.y].GetComponent<Ally>().TakeDamage();
                yield return characterFlashRed(grid[pos.x - 1, pos.y]);
            }
        }
        if(pos.x+1 <= col)
        {
            if (grid[pos.x + 1, pos.y].CompareTag("Ally"))
            {
                grid[pos.x + 1, pos.y].GetComponent<Ally>().TakeDamage();
                yield return characterFlashRed(grid[pos.x + 1, pos.y]);

            }
        }
        
        if(pos.y-1 >= 0)
        {
            if (grid[pos.x, pos.y - 1].CompareTag("Ally"))
            {
                grid[pos.x, pos.y - 1].GetComponent<Ally>().TakeDamage();
                yield return characterFlashRed(grid[pos.x, pos.y - 1]);

            }
        }
        
        if(pos.y+1 <= row)
        {
            if (grid[pos.x, pos.y + 1].CompareTag("Ally"))
            {
                grid[pos.x, pos.y + 1].GetComponent<Ally>().TakeDamage();
                yield return characterFlashRed(grid[pos.x, pos.y + 1]);

            }
        }
        yield return new WaitForSeconds(moveDelay);
    }

    private IEnumerator characterFlashRed(GameObject g)
    {
        SpriteRenderer sprite = g.GetComponent<SpriteRenderer>();
        float flashingFor = 0;
        float flashSpeed = 0.1f;
        float flashTime = 0.3f;
        var flashColor = Color.red;
        var newColor = flashColor;
        var originalColor = Color.white;
        while (flashingFor < flashTime)
        {
            sprite.color = newColor;
            flashingFor += Time.deltaTime;
            yield return new WaitForSeconds(flashSpeed);
            flashingFor += flashSpeed;
            if (newColor == flashColor)
            {
                newColor = originalColor;
            }
            else
            {
                newColor = flashColor;
            }
        }
        sprite.color = originalColor;


    }
    private void StartPlayerPhase()
    {
        numberWaiting = 0;
        Debug.Log("Player phase started");
        gamePhase = GamePhase.Player;
        foreach(GameObject g in partyInstance)
        {
            g.GetComponent<Ally>().Ready();
        }
        return;
    }
    private IEnumerator ExecuteMove()
    {
        Debug.Log("Executing move of size " + movementOrbs.Count);
        if(movementOrbs.Count < 2)
        {
            ResetValues();
            yield break;
        }
        else if(movementOrbs.Count == 2 && movementOrbs.Peek() == selectedAlly)
        {
            ResetValues();
            yield break;
        }
        //movementOrbs.
        ReverseStack();
        //IEnumerator<GameObject> enumerator = movementOrbs.GetEnumerator();
        moving = true;
        while (movementOrbs.Count > 0)
        {
            Debug.Log("Swap initiating with: " + movementOrbs.Peek().name + " at " + movementOrbs.Peek().transform.position);
            StartCoroutine(SwapOrbs(selectedAlly, movementOrbs.Pop()));
            yield return new WaitForSeconds(moveDelay);

        }
        
        //CheckMatch();
        //reset all conditions
        foreach (GameObject g in arrowIndicators)
        {
            g.SetActive(false);
        }
        Ally a = selectedAlly.GetComponent<Ally>();
        a.moveState = Ally.moveStates.Wait;
        numberWaiting++;
        a.Wait();
        

        if (CheckMatch())
        {
            yield return FillRemaining();
        }

        if (numberWaiting >= party.Count)
        {
            Debug.Log("Enemy phase, all allies waiting");
            gamePhase = GamePhase.Enemy;
            StartCoroutine(EnemyPhase());
        }
        //FillHoles();
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
        moving = false;
        lastSelected = null;
        selectedAllyCursorInstance.SetActive(false);
        selectedAlly = null;
        characterSelected = false;
        movementOrbs.Clear();
        stepsLeft.SetText("");
        
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
    

    
}
