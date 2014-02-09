using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public abstract class Unit : MonoBehaviour
{
    //xander, why do we need these, just use transform.position. no need to keep track of the info twice
    public int x, y;
    public bool isSelected, lookingAtEnemyIndicators;
    public bool isUnderCursor = false;
    public Transform cursorLoc;
    public UnityEngine.Object moveIndicator;
    public UnityEngine.Object AttackIndicator;
    public TeamColor team;
    public UnitType type;
    public int hp = 10;

    public bool isVehicle = false;

    public Team owner;

    // used to more or less use Dijkstra's algorithm to make movement indicators
    private struct PriQueueElt { public int x, y, moveDistRemaining; public List<Direction> path; }
    private List<PriQueueElt> bfsQueue = new List<PriQueueElt>();

    public List<GameObject> IndicatorList = new List<GameObject>();

	public static Unit currentSelectedUnit;

    public bool isReadyToAttack = false;
    public bool menuing = false;

    public bool hid_hasMovedThisTurn = false;
    public bool hasMovedThisTurn
    {
        set
        {
            hid_hasMovedThisTurn = value;

            gameObject.transform.Find("MovedIndicator").gameObject.SetActive(value);
        }
        get
        {
            return hid_hasMovedThisTurn;
        }
    }

    abstract protected int GetUnitMoveCost(TerrainType terrain);
    abstract protected int GetUnitMoveRange();
    abstract protected int GetAttack(Unit target);

	// This denotes BOTH move animation AND attack animation
    private bool _InAnimation;
    public bool InAnimation
    {
        set
        {
            GameBoard.Instance.someUnitAnimating = value;
            _InAnimation = value;
        }
        get {
            return _InAnimation;
        }
    }

    Vector2 start_pos; // pos we started animation from

    // Use this for initialization
    public void Start()
    {
        InAnimation = false;
        lookingAtEnemyIndicators = false;
        cursorLoc = GameObject.Find("cursor").transform;
        moveIndicator = Resources.Load("moveIndicator");
        AttackIndicator = Resources.Load("AttackIndicator");

        isSelected = false;
        isUnderCursor = false;
        type = UnitType.Infantry;

        x = (int)transform.position.x;
        y = (int)transform.position.y;
        GameBoard.Instance.unitLocs.Add(transform.position);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (InAnimation)
			return;

        if (Input.GetKeyDown(KeyCode.X) && Input.GetKeyDown(KeyCode.Z)) { return; }// prevent dumb bugs if they hit go and stop at the same time

        if (Input.GetKeyDown(KeyCode.X))
        { // cancel whatever we are doing
            CancelActionIfApplicable();
			moveindicatorscript.destroyPath();
            return;
        }
        if (menuing)
        {
            return;
        }
        else if (isReadyToAttack && Input.GetKeyDown(KeyCode.Z))
        {
            HandleAttack();
        }
        else if (isSelected)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                handleMove();
				moveindicatorscript.destroyPath();
            }
        }
        else if (isUnderCursor)
        {
            // if  it is our turn and noone else is selected, we can move
            if (Input.GetKeyDown(KeyCode.Z) && GameBoard.Instance.isAnyoneSelected == false
                && team == GameBoard.Instance.current && !hasMovedThisTurn)
            {
				currentSelectedUnit = this;
                GameBoard.Instance.isAnyoneSelected = true;
                isSelected = true;
                makeMoveIndicators();

                GameBoard.Instance.someUnitActive = true;
            }
            //if its not our turn, we can still look at their movement range, but only make things once
            else if (Input.GetKeyDown(KeyCode.C) && !lookingAtEnemyIndicators
                     && !GameBoard.Instance.isAnyoneSelected && team != GameBoard.Instance.current)
            {
                lookingAtEnemyIndicators = true;
                makeMoveIndicators();
            }
        }
    }

    void CancelActionIfApplicable() {
        if (isReadyToAttack) { // clean up attack inidcators and re-open the menu
            GameBoard.Instance.someUnitAttacking = false;
            isReadyToAttack = false;
            menuing = true;
            DeleteIndicators();
            //disable the cursor because we are in a menu, allso make sure that we are not displaying the attack cursor
            CursorScript.Instance.gameObject.SetActive(false);
            CursorScript.Instance.cursorsr.gameObject.SetActive(true);
            CursorScript.Instance.crosshsr.gameObject.SetActive(false);

            //open menu back up
            CursorScript.Instance.unitMenu.GetComponent<UAMScript>().turnOn(this);
        }


        else if (menuing) // if in menu get out of menu, make unit still selected in his old position
        {
            //remove moved location from unit locations
            foreach (Vector2 loc in GameBoard.Instance.unitLocs)
            {
                if (Vector2.Distance(this.transform.position, loc) < .3f)
                {
                    GameBoard.Instance.unitLocs.Remove(loc);
                    break;
                }
            }
            this.transform.position = start_pos;
            GameBoard.Instance.unitLocs.Add(this.transform.position);
            menuing = false;
            CursorScript.Instance.unitMenu.GetComponent<UAMScript>().fixpos();
            CursorScript.Instance.unitMenu.GetComponent<UAMScript>().gameObject.SetActive(false);
            makeMoveIndicators();

            CursorScript.Instance.gameObject.SetActive(true);
        }
        else if (isSelected) // if we are looking at move indicators for this unit
        {
            isSelected = false;
            DeleteIndicators();
            GameBoard.Instance.isAnyoneSelected = false;
            GameBoard.Instance.someUnitActive = false;
        }
	}

	public const float moveRate = 0.05f;
	public List<Direction> movementPath;
	IEnumerator moveAnimationCoroutine() {
		
		InAnimation = true;
		
		Vector3 dir;
		Vector3 total;
		Vector3 dest;
		foreach (Direction d in movementPath) {
			
			dir = dirToVec(d);
			total = new Vector3();
			dest = transform.position + dir;
			// Move in that direction
			while (total.magnitude < 1f) {
				transform.position += dir*moveRate;
				total += dir*moveRate;
				yield return true;
			}
			
			transform.position = dest;
		}
		
		InAnimation = false;
		GameBoard.Instance.unitLocs.Add(transform.position);

        if (this is APC)
            return false;

		CursorScript.Instance.unitMenu.GetComponent<UAMScript>().turnOn(this);
		menuing = true;
	}

	const float _attackAnimTime = 0.5f;
	public Unit attackTarget;
	public bool isAttacking;
	IEnumerator attackAnimationCoroutine() {

		// Turn off the crosshair
		CursorScript.Instance.crosshsr.gameObject.SetActive(false);

		// This is a property with a setter that takes care of the GameObject attribute
		InAnimation = true;
		isAttacking = true;

		GameObject gunFire = GameObject.Find ("Controller").GetComponent<GameSystemControl> ().gunFire;

		// Attack
		gunFire.transform.position = attackTarget.transform.position;
		gunFire.SetActive (true);

		int dmg = this.GetAttack (attackTarget);
		GameObject dmgInd = attackTarget.transform.Find ("Dmg Display").gameObject;
		dmgInd.GetComponent<TextMesh> ().text = "-" + dmg.ToString ();
		dmgInd.SetActive (true);

		float time = 0f;
		while (time < _attackAnimTime) {
			time += Time.deltaTime;

			yield return true;
		}

		attackTarget.DealDamage(this.GetAttack(attackTarget));
		gunFire.SetActive(false);
		dmgInd.SetActive (false);

		// Check if the target survived
		if (attackTarget.gameObject.activeSelf) {

			// Otherwise, counterattack
			gunFire.transform.position = transform.position;
			gunFire.SetActive (true);
			dmg = attackTarget.GetAttack (this);
			dmgInd = this.transform.Find ("Dmg Display").gameObject;
			dmgInd.GetComponent<TextMesh> ().text = "-" + dmg.ToString ();
			dmgInd.SetActive (true);

			time = 0f;

			while (time < _attackAnimTime) {
				time += Time.deltaTime;
				
				yield return true;
			}
			
			this.DealDamage (attackTarget.GetAttack (this));
			gunFire.SetActive (false);
			dmgInd.SetActive (false);
		}

		InAnimation = false;
		isAttacking = false;

		isSelected = false;
		GameBoard.Instance.isAnyoneSelected = false;
		GameBoard.Instance.someUnitAttacking = false;
		isReadyToAttack = false;
		
		GameBoard.Instance.someUnitActive = false;
		
		hasMovedThisTurn = true;
		owner.unitMoved();

		// Turn on the cursor
		CursorScript.Instance.cursorsr.gameObject.SetActive(true);
	}

    void handleMove()
    {
        //first, make sure the cursor is within the unit's movement rage!
		bool IsCursorOverMoveIndicator = false;
        foreach (GameObject tmp in IndicatorList)
        {
            if (Vector2.Distance(tmp.transform.position, cursorLoc.position) < .3f)
            {
                IsCursorOverMoveIndicator = true;
				movementPath = tmp.GetComponent<moveindicatorscript> ().path;
                break;
            }
        }
        if (!IsCursorOverMoveIndicator)
        {
            return;
        }
		
		DeleteIndicators();

        // if the cursor is currently in motion, dont allow units to move, 
        //this stops the bug where we check if a location is valid to move to, then the cursor moves, then we move the unit to that location
        if (!CursorScript.Instance.canmove)
        {
            return;
        }
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

        CursorScript.Instance.gameObject.SetActive(false);
		pos = new Vector3(CursorScript.Instance.shouldbex, CursorScript.Instance.shouldbey, 0);
        start_pos = this.transform.position;
		StartCoroutine (moveAnimationCoroutine ());
		
		return;

        /*
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

                InMoveAnimation = true;
                destaion_pos = pos;
                start_pos = this.transform.position;
                animationStartTime = Time.time;
                //this.transform.position = pos;
                //add its new location to the list of unit locations
                GameBoard.Instance.unitLocs.Add(pos);
             
                return;
            }
        }
        */
    }

    public void makeAttackIndicators()
    {

        // look to see if any enemy units are within attack range, and make attack indicators as appropriate
        Team opponent = getOtherTeam();
        Vector2 pos = gameObject.transform.position;
        //  foreach (Vector2 loc in GameBoard.Instance.unitLocs)
        foreach (Unit u in opponent.units)
        {
            Vector2 loc = u.gameObject.transform.position;
            if (Vector2.Distance(pos, loc) < 1.2 && Vector2.Distance(pos, loc) > .3)
            { // less than sqrt2, but not on the same square
                Vector3 newind = loc;
                newind.z = -2; // make sure indicators are above everything
                UnityEngine.GameObject x = Instantiate(AttackIndicator, newind, Quaternion.identity) as GameObject;
                x.SetActive(false);
                IndicatorList.Add(x);
            }
        }
    }

    public void prepareToAttack()
    {

        foreach (GameObject g in IndicatorList)
        {
            g.SetActive(true);
        }

        isReadyToAttack = true;
        GameBoard.Instance.someUnitAttacking = true;
    }

    protected void makeMoveIndicators()
    {
        //make the move nowhere indicator first, because recursive won't do it, as there is a unit there allready
        int xpos = (int)Math.Round(this.transform.position.x, 0);
        int ypos = (int)Math.Round(this.transform.position.y, 0);

        //double check that our rounding did what we expected, panic if not
        if (Math.Abs(xpos - this.transform.position.x) + Math.Abs(ypos - this.transform.position.y) > .3f)
        {
            Debug.Log("oh god da fuck is happenin");
        }

        // add go nowhere indicator
        UnityEngine.GameObject x = Instantiate(moveIndicator, this.transform.position, Quaternion.identity) as GameObject;
        IndicatorList.Add(x);

        if (this is APC)
        {
            x.renderer.material.color = new Color(x.renderer.material.color.r, x.renderer.material.color.g, x.renderer.material.color.b, .3f);
            Destroy(x.collider2D);
        }
        else {
            CursorScript.Instance.currentIndicator = x;
        }

        //add 4 adjancent squares to our queue
        PriQueueElt queueElt;
        queueElt.moveDistRemaining = GetUnitMoveRange();
        queueElt.x = xpos + 1;
        queueElt.y = ypos;
        queueElt.path = new List<Direction>();
        queueElt.path.Add(Direction.Right);
        bfsQueue.Add(queueElt);
        queueElt.x = xpos - 1;
        queueElt.y = ypos;
        queueElt.path = new List<Direction>();
        queueElt.path.Add(Direction.Left);
        bfsQueue.Add(queueElt);
        queueElt.x = xpos;
        queueElt.y = ypos + 1;
        queueElt.path = new List<Direction>();
        queueElt.path.Add(Direction.Up);
        bfsQueue.Add(queueElt);
        queueElt.x = xpos;
        queueElt.y = ypos - 1;
        queueElt.path = new List<Direction>();
        queueElt.path.Add(Direction.Down);
        bfsQueue.Add(queueElt);


        callNextBFS(); // start make_move_indicators_recursive
    }

    public GameObject findLeftMoveIndicator() {

        GameObject leftMoveInd = IndicatorList[0];
        int x = 1000;//(int)Math.Round(leftMoveInd.transform.position.x);
        int y = 1000; //(int)Math.Round(leftMoveInd.transform.position.y);

        foreach (UnityEngine.GameObject indicator in IndicatorList)
        {
            if (Math.Abs(indicator.transform.position.y - 1) < .2f || Math.Abs(indicator.transform.position.y - 8) <.2f) {
                //Debug.Log("ain't goin there");
                continue;
            }
            if (indicator.transform.position.x < x)
            {
                x = (int)Math.Round(indicator.transform.position.x);
                y = (int)Math.Round(indicator.transform.position.y);
                leftMoveInd = indicator;
            }
            else if (indicator.transform.position.x == x && indicator.transform.position.y > y)
            {
                x = (int)Math.Round(indicator.transform.position.x);
                y = (int)Math.Round(indicator.transform.position.y);
                leftMoveInd = indicator;
            }
        }
        return leftMoveInd;

    }
    //moves unit as far left as legally allowed, called by AI team
    public void moveLeft()
    {
        //Debug.Log("unit movin left");
        makeMoveIndicators();
        // x and y values for the current furthest left movement indicator

        GameObject leftMoveInd = findLeftMoveIndicator();

        Vector3 newPos;
        newPos.z = this.transform.position.z;
        newPos.x = (int)Math.Round(leftMoveInd.transform.position.x);
        newPos.y = (int)Math.Round(leftMoveInd.transform.position.y);

        if (newPos.x == 0 && !GameBoard.Instance.GameOver)
        {
            GameBoard.Instance.GameOver = true;
            GameObject defeat = GameObject.Find("DefeatText");
            defeat.renderer.enabled = true;
        }
        //remove old location of this unit from our list, and add new one
        foreach (Vector2 loc in GameBoard.Instance.unitLocs)
        {
            if (Vector2.Distance(this.transform.position, loc) < .3f)
            {
                GameBoard.Instance.unitLocs.Remove(loc);
                break;
            }
        }
        GameBoard.Instance.unitLocs.Add(newPos);

        //setup values for animation
        InAnimation = true;
        start_pos = this.transform.position;

        movementPath = findLeftMoveIndicator().GetComponent<moveindicatorscript>().path;
        StartCoroutine(moveAnimationCoroutine());

        DeleteIndicators();
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

    void makeMoveIndicatorsRecursive(int movedist, int xloc, int yloc, List<Direction> path)
    {

        // first check if we are trying to move off the map. if so return;
        TerrainType terrain;
        try
        {
            terrain = GameBoard.Instance.terrains[xloc, yloc];
        }
        catch
        {
            //we are trying to move outside the map, so dont go any further.
            callNextBFS();
            return;
        }

        int movecost = GetUnitMoveCost(terrain);

        // next return if we are out of movement range
        if (movedist - movecost < 0)
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
        if (xloc < 0 || yloc < 0)
        {
            Debug.Log("oh god da fuck is happenin");
        }


        if (!occupied)
        {
            UnityEngine.GameObject x = Instantiate(moveIndicator, pos, Quaternion.identity) as GameObject;
            x.GetComponent<moveindicatorscript>().path = path;
            IndicatorList.Add(x);

            if (this is APC) { 
                x.renderer.material.color = new Color(x.renderer.material.color.r, x.renderer.material.color.g, x.renderer.material.color.b, .3f);
                Destroy(x.collider2D);
            }
        }

        int movementRemaining = movedist - movecost;

        PriQueueElt queueElt;
        queueElt.moveDistRemaining = movementRemaining;
        //add move right to queue
        queueElt.x = xloc + 1;
        queueElt.y = yloc;
        queueElt.path = new List<Direction>(path);
        queueElt.path.Add(Direction.Right);
        bfsQueue.Add(queueElt);
        //add move left to queue
        queueElt.x = xloc - 1;
        queueElt.y = yloc;
        queueElt.path = new List<Direction>(path);
        queueElt.path.Add(Direction.Left);
        bfsQueue.Add(queueElt);
        //add move up to queue
        queueElt.x = xloc;
        queueElt.y = yloc + 1;
        queueElt.path = new List<Direction>(path);
        queueElt.path.Add(Direction.Up);
        bfsQueue.Add(queueElt);
        //add move down to queue
        queueElt.x = xloc;
        queueElt.y = yloc - 1;
        queueElt.path = new List<Direction>(path);
        queueElt.path.Add(Direction.Down);
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
            else if (c1.moveDistRemaining > c2.moveDistRemaining)
            {
                return 0;
            }
            return 1;
        }
    }

    // calls the next element in our ghetto priority queue,  used only to make movement indicators
    private void callNextBFS()
    {
        if (bfsQueue.Count == 0)
        {

            return;
        }

        PriQueueElt queueElt = bfsQueue[0];
        bfsQueue.RemoveAt(0);
        makeMoveIndicatorsRecursive(queueElt.moveDistRemaining, queueElt.x, queueElt.y, queueElt.path);
    }

    // cleans up any movement or attack indicators we have made
    protected void DeleteIndicators()
    {
        foreach (GameObject x in IndicatorList)
        {
            Destroy(x);
        }
        IndicatorList.Clear();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (CursorScript.Instance.unitUnderCursor == this)
        {
            CursorScript.Instance.unitUnderCursor = null;
        }
        isUnderCursor = false;

        // if we are currently looking at an opponetns rage, remove that data as soon as we stop hoving the cursor
        if (lookingAtEnemyIndicators)
        {
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
                //here is where we check if we are attacking and attack if needed, otherwise just end turn
                DeleteIndicators();

                Unit target = CursorScript.Instance.unitUnderCursor;

                if (target.team != GameBoard.Instance.current)
                {
					attackTarget = target;
					StartCoroutine(attackAnimationCoroutine());
                }

                break;
            }
        }
    }

    void DealDamage(int dmg)
    {

        hp -= dmg;

        if (hp <= 0)
            Die();

        TextMesh tm = (TextMesh)transform.Find("HP Display").GetComponent(typeof(TextMesh));
        tm.text = hp.ToString();
    }

    virtual protected void Die()
    {
        GameBoard.Instance.unitLocs.Remove(this.gameObject.transform.position);
        gameObject.SetActive(false);
        owner.unitDestroyed(this);
	}
	
	public static Vector3 dirToVec(Direction dir) {
		switch (dir) {
		case Direction.Up:
			return new Vector3(0,1,0);
		case Direction.Down:
			return new Vector3(0,-1,0);
		case Direction.Right:
			return new Vector3(1,0,0);
		case Direction.Left:
			return new Vector3(-1,0,0);
		}
		
		return new Vector3();
	}
}
