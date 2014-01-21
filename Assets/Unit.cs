using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    //xander, why do we need these, just use transform.position. no need to keep track of the info twice
    public int x, y;
    public bool isSelected, isUnderCuror, isReadyToAttack;
    public Transform cursorLoc;
    public UnityEngine.Object moveIndicator;
    public UnityEngine.Object AttackIndicator;
    public Team team;
    public UnitType type;

    public GameBoard game;

    private List<UnityEngine.GameObject> IndicatorList = new List<UnityEngine.GameObject>();
    // Use this for initialization
    void Start()
    {
        cursorLoc = GameObject.Find("cursor").transform;
        moveIndicator = Resources.Load("moveIndicator");
        AttackIndicator = Resources.Load("AttackIndicator");
        game = GameBoard.Instance;

        isSelected = false;
        isReadyToAttack = false;
        isUnderCuror = false;
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
        else if (isUnderCuror)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isSelected = true;
                makeMoveIndicators();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Z) && isSelected)
            {
                DeleteIndicators();
                isSelected = false;
            }
            if (Input.GetKeyDown(KeyCode.Space) && isSelected)
            {
                handleMove();
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


                // add attack indicators 
                foreach (Vector2 loc in GameBoard.Instance.unitLocs)
                {
                    if (Vector2.Distance(pos, loc) < 1.2 && Vector2.Distance(pos, loc) > .3)
                    { // less than sqrt2, but not on the same square
                        UnityEngine.GameObject x = Instantiate(AttackIndicator, loc, Quaternion.identity) as GameObject;
                        IndicatorList.Add(x);
                    }
                }
                UnityEngine.GameObject waitIndicator = Instantiate(moveIndicator, pos, Quaternion.identity) as GameObject;
                IndicatorList.Add(waitIndicator);

                isSelected = false;
                isReadyToAttack = true;
                break;
            }
        }   
    }

    void makeMoveIndicators()
    {
        makeMoveIndicatorsRecursive(4, this.transform.position);
    }

    void makeMoveIndicatorsRecursive(int movedist, Vector3 pos)
    {
        if (movedist < 0)
        {
            return;
        }
        foreach (GameObject tmp in IndicatorList)
        { //computers are fast, so we can get away with this slow algorithms
            if (tmp.transform.position == pos)
            {
                return;
            }
        }
        UnityEngine.GameObject x = Instantiate(moveIndicator, pos, Quaternion.identity) as GameObject;
        IndicatorList.Add(x);
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

        isUnderCuror = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        isUnderCuror = true;
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
                isReadyToAttack = false;
                break;
            }
        }
    }
}
