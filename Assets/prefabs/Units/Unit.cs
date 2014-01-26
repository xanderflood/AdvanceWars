using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class Unit : MonoBehaviour
{
    //xander, why do we need these, just use transform.position. no need to keep track of the info twice
    public int x, y;
    public bool isSelected, isUnderCursor, lookingAtEnemyIndicators;
    public Transform cursorLoc;
    public UnityEngine.Object moveIndicator;
    public UnityEngine.Object AttackIndicator;
    public TeamColor team;
    public UnitType type;
	public int hp = 10;
	
	public Team owner;
	
	// used to more or less use Dijkstra's algorithm to make movement indicators
	private struct PriQueueElt { public int x, y, moveDistRemaining;}
	private List<PriQueueElt> bfsQueue = new List<PriQueueElt>();
	
	private List<UnityEngine.GameObject> IndicatorList = new List<UnityEngine.GameObject>();

	public bool isReadyToAttack = false;

	public bool hid_hasMovedThisTurn = false;
	public bool hasMovedThisTurn {
		set {
			hid_hasMovedThisTurn = value;

			gameObject.transform.Find("MovedIndicator").gameObject.SetActive(value);
		}
		get {
			return hid_hasMovedThisTurn;
		}
	}

    // Use this for initialization
    public void Start()
    {
        lookingAtEnemyIndicators = false;
        cursorLoc = GameObject.Find("cursor").transform;
        moveIndicator = Resources.Load("moveIndicator");
        AttackIndicator = Resources.Load("AttackIndicator");

        isSelected = false;
        isUnderCursor = false;
        team = TeamColor.None;
        type = UnitType.Infantry;

        x = (int)transform.position.x;
        y = (int)transform.position.y;
        GameBoard.Instance.unitLocs.Add(transform.position);
	}

    // Update is called once per frame
    void Update()
    {
        if (isReadyToAttack && Input.GetKeyDown(KeyCode.Space))
        {
            HandleAttack();
        }
        else if(isSelected)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                DeleteIndicators();
                GameBoard.Instance.isAnyoneSelected = false;
                isSelected = false;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                handleMove();
            }
        }
        else if (isUnderCursor)
        {
            // if  it is our turn and noone else is selected, we can move
            if (Input.GetKeyDown(KeyCode.Space) && GameBoard.Instance.isAnyoneSelected == false
			    && team == GameBoard.Instance.current && !hasMovedThisTurn)
            {
                GameBoard.Instance.isAnyoneSelected = true;
                isSelected = true;
                makeMoveIndicators();

				GameBoard.Instance.someUnitActive = true;
            }
            //if its not our turn, we can still look at their movement range, but only make things once
            else if (Input.GetKeyDown(KeyCode.C) && !lookingAtEnemyIndicators && !GameBoard.Instance.isAnyoneSelected  && team != GameBoard.Instance.current)
            {
                lookingAtEnemyIndicators = true;
                makeMoveIndicators();
            }
        }
    }

    void handleMove() {
        // awful code that checks if the curser is over one of the range indacators we made, 
        foreach (GameObject tmp in IndicatorList)
        {
            if (Vector2.Distance(tmp.transform.position, cursorLoc.position) < .3f)
            {
                //if (Math.Abs(tmp.transform.position.x - cursorLoc.position.x) < 0.3f &&
                //Math.Abs(tmp.transform.position.y - cursorLoc.position.y) < 0.3f) {
                DeleteIndicators();
                Vector3 pos = this.transform.position;
                //we are moving the unit, so remove it's old location from the list
                foreach (Vector2 loc in GameBoard.Instance.unitLocs)
                {
                    if (Vector2.Distance(pos, loc) < .3f)
                    {
                        GameBoard.Instance.unitLocs.Remove(loc);
                        break;
                    }
                }

				pos = new Vector3(CursorScript.Instance.shouldbex, CursorScript.Instance.shouldbey, 0);
				this.transform.position = pos;
                //add it's new location to the list of unit locations
                GameBoard.Instance.unitLocs.Add(pos);

                // look to see if any enemy units are within attack range, and make attack indicators as appropriate
                Team opponent = getOtherTeam();
                //  foreach (Vector2 loc in GameBoard.Instance.unitLocs)
                foreach (Unit u in opponent.units)
                {
                    Vector2 loc = u.gameObject.transform.position;
                    if (Vector2.Distance(pos, loc) < 1.2 && Vector2.Distance(pos, loc) > .3)
                    { // less than sqrt2, but not on the same square
                        Vector3 newind = loc;
                        newind.z = -2; // make sure indicators are above everything
                        UnityEngine.GameObject x = Instantiate(AttackIndicator, newind, Quaternion.identity) as GameObject;
                        IndicatorList.Add(x);
                    }
                }
                UnityEngine.GameObject waitIndicator = Instantiate(moveIndicator, pos, Quaternion.identity) as GameObject;
                IndicatorList.Add(waitIndicator);
				isReadyToAttack = true;
				GameBoard.Instance.someUnitAttacking = true;
				return;
            }
		}
    }

    void makeMoveIndicators()
    {
        //make the move nowhere indicator first, because recursive won't do it, as there is a unit there allready
        int xpos = (int)Math.Round(this.transform.position.x, 0);
        int ypos = (int)Math.Round(this.transform.position.y, 0);

        //double check that our rounding did what we expected, panic if not
        if (Math.Abs(xpos - this.transform.position.x) + Math.Abs(ypos - this.transform.position.y) > .3f)
        {
            Debug.Log("oh god da fuck is happenin");
            int zero = 0;
            int error = 1 / zero;
        }

        // add go nowhere indicator
        UnityEngine.GameObject x = Instantiate(moveIndicator, this.transform.position, Quaternion.identity) as GameObject;
        IndicatorList.Add(x);

        //add 4 adjancent squares to our queue
        PriQueueElt queueElt;
        queueElt.moveDistRemaining = 3;
        queueElt.x = xpos + 1;
        queueElt.y = ypos;
        bfsQueue.Add(queueElt);
        queueElt.x = xpos - 1;
        queueElt.y = ypos;
        bfsQueue.Add(queueElt);
        queueElt.x = xpos;
        queueElt.y = ypos + 1;
        bfsQueue.Add(queueElt);
        queueElt.x = xpos;
        queueElt.y = ypos - 1;
        bfsQueue.Add(queueElt);


        callNextBFS(); // start make_move_indicators_recursive
    }

    Team getOtherTeam()
    {
        Team opponent;
        if (this.owner == GameBoard.Instance.redTeam)
        {
            opponent = GameBoard.Instance.blueTeam;
        }
        else
        {
            opponent = GameBoard.Instance.redTeam;
        }
        return opponent;
    }

    void makeMoveIndicatorsRecursive(int movedist, int xloc, int yloc)
    {
        
        // first check if we are trying to move off the map. if so return;
        int movecost;
        try
        {
            movecost = (int)GameBoard.Instance.terrains[xloc, yloc];
        }
        catch
        {
            //we are trying to move outside the map, so dont go any further.
            callNextBFS();
            return;
        }
        //Debug.Log(movecost);
        // fix the movecots to thir correct values. abstract this away eventually, todo
        if (movecost == 2)
        {
            movecost = 1;
        }
        else if (movecost == 3)
        {
            movecost = 2;
        }


        // next return if we are out of movement range
        if (movedist  - movecost < 0)
        {
            callNextBFS();
            return;
        }

        Vector3 pos = new Vector3(xloc, yloc, -.2f);

        bool occupied = false;
        foreach (GameObject tmp in IndicatorList)
        { 
            if (Vector2.Distance(tmp.transform.position, pos) < .3f)
            {
                callNextBFS();
                return;              
            }
        }
         // check to see if there is a unit of the other team in the way, in that case dont move though them
        Team opponent = getOtherTeam();
        foreach (Unit u in opponent.units)
        {
            Vector2 loc = u.gameObject.transform.position;
            if (Vector2.Distance(pos, loc) < .2f)
            {
                callNextBFS();
                return;              

            }
        }

        // check to see if the tile we are looking at is allready occupied by a unit, if so dont add a move indicator
        foreach (Vector2 loc in GameBoard.Instance.unitLocs)
        {
            if (Vector2.Distance(pos, loc) < .2f)
            {
                occupied = true; // we want to be able to walk though allied units          
                break;
            }
        }

        
        // this is the most ghetto assert ever. I'm sorry. fix it later
        if ( xloc < 0 || yloc <0)
        {
            Debug.Log("oh god da fuck is happenin");
            int zero = 0;
            int error = 1 / zero;
        }

        
        if (!occupied)
        {
            UnityEngine.GameObject x = Instantiate(moveIndicator, pos, Quaternion.identity) as GameObject;
            IndicatorList.Add(x);
        }

        int movementRemaining = movedist - movecost;

        PriQueueElt queueElt;
        queueElt.moveDistRemaining = movementRemaining;
        //add move right to queue
        queueElt.x = xloc +1;
        queueElt.y = yloc;
        bfsQueue.Add(queueElt);
        //add move left to queue
        queueElt.x = xloc - 1;
        queueElt.y = yloc;
        bfsQueue.Add(queueElt);
        //add move up to queue
        queueElt.x = xloc;
        queueElt.y = yloc+1;
        bfsQueue.Add(queueElt);
        //add move down to queue
        queueElt.x = xloc;
        queueElt.y = yloc -1 ;
        bfsQueue.Add(queueElt);

        bfsQueue.Sort(new queueSort());
        callNextBFS();

    }

    // used to sort the list, BECAUSE C SHARP DOES NOT HAVE FUCKING PRIORITY QUEUES. 
    private class queueSort : IComparer<PriQueueElt>
    {
        public int Compare(PriQueueElt c1, PriQueueElt c2)
        {
            if (c1.moveDistRemaining > c2.moveDistRemaining)
            {
                return -1;
            }
            else if (c1.moveDistRemaining > c2.moveDistRemaining) {
                return 0;
            }
            return 1;
        }
    }

    // calls the next element in our ghetto priority queue,  used only to make movement indicators
    private void callNextBFS(){
        if(bfsQueue.Count == 0){

            return;
        }

        PriQueueElt queueElt = bfsQueue[0];
        bfsQueue.RemoveAt(0);
        makeMoveIndicatorsRecursive(queueElt.moveDistRemaining, queueElt.x, queueElt.y);    
    }

    // cleans up any movement or attack indicators we have made
    void DeleteIndicators()
    {
        foreach (GameObject x in IndicatorList)
        {
            Destroy(x);
        }
        IndicatorList.Clear();
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if (CursorScript.Instance.unitUnderCursor == this) {
			CursorScript.Instance.unitUnderCursor = null;
		}
		isUnderCursor = false;

        // if we are currently looking at an opponetns rage, remove that data as soon as we stop hoving the cursor
        if (lookingAtEnemyIndicators) {
            DeleteIndicators();
            lookingAtEnemyIndicators = false;
        }
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		CursorScript.Instance.unitUnderCursor = this;
		isUnderCursor = true;
	}
	
	void HandleAttack()
	{
		//make sure that we are close to one of the indicators
		foreach (GameObject tmp in IndicatorList)
		{
			if (Vector2.Distance(tmp.transform.position, cursorLoc.position) < .3f)
			{
				//here is where we check if we are attacking and attack if needed, otherwise juse end turn
				DeleteIndicators();
				
				Unit target = CursorScript.Instance.unitUnderCursor;
				
				if (target.team != GameBoard.Instance.current) {
					target.DealDamage(5);
					this.DealDamage(2);
				}
                isSelected = false;
                GameBoard.Instance.isAnyoneSelected = false;
				GameBoard.Instance.someUnitAttacking = false;
				isReadyToAttack = false;
				
				GameBoard.Instance.someUnitActive = false;

				hasMovedThisTurn = true;
				//Debug.Log(owner);
				owner.unitMoved();

				CursorScript.Instance.cursorsr.gameObject.SetActive(true);
				CursorScript.Instance.crosshsr.gameObject.SetActive(false);
				break;
			}
		}
	}
	
	void DealDamage(int dmg) {
		
		hp -= dmg;
		
		if (hp <= 0)
			Die ();
		
		TextMesh tm = (TextMesh)transform.Find ("HP Display").GetComponent(typeof(TextMesh));
		tm.text = hp.ToString();
	}
	
	void Die() {
		gameObject.SetActive(false);
		owner.unitDestroyed(this);
	}
}
