using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    //xander, why do we need these, just use transform.position. no need to keep track of the info twice
    public int x, y;
    public bool isSelected, isUnderCursor, isReadyToAttack;
    public Transform cursorLoc;
    public UnityEngine.Object moveIndicator;
    public UnityEngine.Object AttackIndicator;
    public Team team;
    public UnitType type;
	public int hp = 10;
    bool lookingAtEnemyIndicators;

    private List<UnityEngine.GameObject> IndicatorList = new List<UnityEngine.GameObject>();
    // Use this for initialization
    public void Start()
    {
         lookingAtEnemyIndicators = false;
        cursorLoc = GameObject.Find("cursor").transform;
        moveIndicator = Resources.Load("moveIndicator");
        AttackIndicator = Resources.Load("AttackIndicator");

        isSelected = false;
        isReadyToAttack = false;
        isUnderCursor = false;
        team = Team.None;
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
            //looking at movment range
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // if  it is our turn and noone else is selected, we can move
                if (GameBoard.Instance.isAnyoneSelected == false && team == GameBoard.Instance.current)
                {
                    GameBoard.Instance.isAnyoneSelected = true;
                    isSelected = true;
                    makeMoveIndicators();
                }
                //if its not our turn, we can still look at their movement range, but only make things once
                else if(!lookingAtEnemyIndicators)
                {
                    lookingAtEnemyIndicators = true;
                    makeMoveIndicators();
                }
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
                pos.x = cursorLoc.position.x;
                pos.y = cursorLoc.position.y;
                this.transform.position = pos;
                //add it's new location to the list of unit locations
                GameBoard.Instance.unitLocs.Add(pos);


                //xander:: if there is noone to attack, do we still want to skip this step and just finish the move?
                // add attack indicators 
                foreach (Vector2 loc in GameBoard.Instance.unitLocs)
                {
                    if (Vector2.Distance(pos, loc) < 1.2 && Vector2.Distance(pos, loc) > .3)
                    { // less than sqrt2, but not on the same square
                        Debug.Log("wtf");
                        Vector3 newind = loc;
                        newind.z = -2; // make sure indicators are above everything
                        UnityEngine.GameObject x = Instantiate(AttackIndicator, newind, Quaternion.identity) as GameObject;
                        IndicatorList.Add(x);
                    }
                }
                UnityEngine.GameObject waitIndicator = Instantiate(moveIndicator, pos, Quaternion.identity) as GameObject;
                IndicatorList.Add(waitIndicator);

                isSelected = false;
                GameBoard.Instance.isAnyoneSelected = false;
                isReadyToAttack = true;
                return;
            }
        }   
    }

    void makeMoveIndicators()
    {
        //make the move nowhere indicator first, because recursive won't do it, as there is a unit there allready
        UnityEngine.GameObject x = Instantiate(moveIndicator, this.transform.position, Quaternion.identity) as GameObject;
        IndicatorList.Add(x);
        makeMoveIndicatorsRecursive(4, this.transform.position);
    }

    void makeMoveIndicatorsRecursive(int movedist, Vector3 pos)
    {
        if (movedist < 0)
        {
            return;
        }
        bool occupied = false;
        foreach (GameObject tmp in IndicatorList)
        { //computers are fast, so we can get away with this slow algorithms
            if (tmp.transform.position == pos)
            {
                occupied = true; //cant just return here because we are doing DFS, and sometimes we might have made a square at the end of the move range where we want to go, but we can get there faster, so still need to search out
            }
        }

        // check to see if the tile we are looking at is allready occupied by a unit, if so dont add a move indicator
        // todo: make it so you cant move though foes;

        foreach (Vector2 loc in GameBoard.Instance.unitLocs)
        {
            if (Vector2.Distance(pos, loc) < .2f)
            {
                occupied = true;
                break;
            }
        }

        if (!occupied)
        {
            UnityEngine.GameObject x = Instantiate(moveIndicator, pos, Quaternion.identity) as GameObject;
            IndicatorList.Add(x);
        }



        Vector3 posCpy = pos;
        //move right
        pos.x = pos.x + 1;
        makeMoveIndicatorsRecursive(movedist - 1, pos);   //todo: change -1 to -terrain move cost
        //move left
        pos = posCpy;
        pos.x = pos.x - 1;
        makeMoveIndicatorsRecursive(movedist - 1, pos);
        // move up
        pos = posCpy;
        pos.y = pos.y + 1;
        makeMoveIndicatorsRecursive(movedist - 1, pos);
        // move down
        pos = posCpy;
        pos.y = pos.y - 1;
        makeMoveIndicatorsRecursive(movedist - 1, pos);
    }

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
				
				isReadyToAttack = false;
				break;
			}
		}
	}
	
	void DealDamage(int dmg) {
		
		hp -= dmg;
		
		if (hp <= 0)
			gameObject.SetActive (false);
		
		TextMesh tm = (TextMesh)transform.Find ("HP Display").GetComponent(typeof(TextMesh));
		tm.text = hp.ToString();
	}
}
