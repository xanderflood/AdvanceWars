using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class UnitMovement : MonoBehaviour
{
		public bool isSelected;
		public bool isUnderCuror;
		public Transform cursorLoc;
		public GameObject moveIndicator;


		private List<UnityEngine.GameObject> moveIndList =  new List<UnityEngine.GameObject>();
		// Use this for initialization
		void Start ()
		{
			isSelected = false;
		} 
		// Update is called once per frame
		void Update ()
		{
				if (isUnderCuror) {
						if (Input.GetKeyDown (KeyCode.Space)) {
								isSelected = true;
								makeMoveIndicators();
						}
				} else {
						if (Input.GetKeyDown (KeyCode.Space)) {
								if (isSelected) {
										// awful code that checks if the curser is over one of the range indacators we made, 
										foreach (GameObject tmp in moveIndList) { 
											// equality of vectors didnt work for whatever reason, so use the silly double equality check 
											if(Vector3.Distance(tmp.transform.position,cursorLoc.position)<.3f){
											//if (Math.Abs(tmp.transform.position.x - cursorLoc.position.x) < 0.3f &&
						   						// Math.Abs(tmp.transform.position.y - cursorLoc.position.y) < 0.3f) {
												DeleteMoveIndicators();
												isSelected = false;
												Vector3 pos = this.transform.position;
												pos.x=  cursorLoc.position.x;
												pos.y = cursorLoc.position.y;
												this.transform.position = pos;
												break;
											}
										}
								}		
						}
				}
		}

		void makeMoveIndicators(){	
			makeMoveIndicatorsRecursive(4, this.transform.position);
		}

		void makeMoveIndicatorsRecursive(int movedist, Vector3 pos){
			if (movedist < 0) {
				return;		
			}
			foreach (GameObject tmp in moveIndList) { //computers are fast, so we can get away with this slow algorithms
				if (tmp.transform.position == pos) {
					return;
				}
			}
			UnityEngine.GameObject x = Instantiate(moveIndicator, pos, Quaternion.identity) as GameObject;
			moveIndList.Add (x);
			Vector3 posCpy = pos;

			//move right
			pos.x = pos.x + 1;
			makeMoveIndicatorsRecursive (movedist - 1, pos);   //todo: change -1 to -terrain move cost
			//move left
			pos = posCpy;
			pos.x = pos.x - 1;
			makeMoveIndicatorsRecursive (movedist - 1, pos);  
			// move up
			pos = posCpy;
			pos.y = pos.y + 1;
			makeMoveIndicatorsRecursive (movedist - 1, pos);  
			// move down
			pos = posCpy;
			pos.y = pos.y - 1;
			makeMoveIndicatorsRecursive (movedist - 1, pos);  
		}

		void DeleteMoveIndicators(){
			foreach (GameObject x in moveIndList) {
				Destroy(x);	
			}
			moveIndList.Clear ();
		}

		void OnTriggerExit2D (Collider2D other)
		{

				isUnderCuror = false;
		}

		void OnTriggerEnter2D (Collider2D other)
		{
				isUnderCuror = true;
		}
}
