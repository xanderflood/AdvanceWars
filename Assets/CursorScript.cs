using UnityEngine;
using System.Collections;
// code stolen from http://answers.unity3d.com/questions/9885/basic-movement-in-a-grid.html
public class CursorScript : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	bool canmove = true; //indicate if a keyboard key can move a piece
	Vector3 targetPosition; //temporary value for moving (used in coroutines)
	public float speed = .1F;
	public int gridSize=1;
	
	void Update()
	{
		/*
		// normal movement
		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");
		
		Vector2 vel = rigidbody2D.velocity;
		vel.x = h*speed;
		vel.y = v * speed;

		rigidbody2D.velocity = vel;
		*/
		//grid movement
	
		if (Input.GetKey(KeyCode.UpArrow) == true && canmove == true)
		{
			canmove = false;
			StartCoroutine (MoveInGrid((int)transform.position.x, (int)transform.position.y+gridSize, (int)transform.position.z));
		}
		if (Input.GetKey(KeyCode.RightArrow) == true && canmove == true)
		{
			canmove = false;
			StartCoroutine(MoveInGrid((int)transform.position.x+gridSize, (int)transform.position.y, (int)transform.position.z));
		}
		if (Input.GetKey(KeyCode.LeftArrow) == true && canmove == true)
		{
			canmove = false;
			StartCoroutine(MoveInGrid((int)transform.position.x-gridSize, (int)transform.position.y, (int)transform.position.z));
		}
		if (Input.GetKey(KeyCode.DownArrow) == true && canmove == true)
		{
			canmove = false;
			StartCoroutine(MoveInGrid((int)transform.position.x, (int)transform.position.y-gridSize, (int)transform.position.z));
		}
	}
	
	
	IEnumerator MoveInGrid(int x,int y,int z)
	{
		while (transform.position.x != x || transform.position.y != y || transform.position.z != z)
		{
			//moving x forward
			if (transform.position.x < x)
			{
				//moving the point by speed 
				targetPosition.x = speed * Time.deltaTime;
				//check if the point goes more than it should go and if yes clamp it back
				if (targetPosition.x + transform.position.x > x)
				{
					targetPosition.x = x - transform.position.x;
				}
			}
			//moving x backward
			else if (transform.position.x > x)
			{
				//moving the point by speed 
				targetPosition.x = -speed * Time.deltaTime;
				//check if the point goes more than it should go and if yes clamp it back
				if (targetPosition.x + transform.position.x < x)
				{
					targetPosition.x = -(transform.position.x - x);
				}
			}
			else //x is unchanged so should be 0 in translate function
			{
				targetPosition.x = 0;
			}
			//moving y forward
			if (transform.position.y < y)
			{
				//moving the point by speed 
				targetPosition.y = speed * Time.deltaTime;
				//check if the point goes more than it should go and if yes clamp it back
				if (targetPosition.y + transform.position.y > y)
				{
					targetPosition.y = y - transform.position.y;
				}
			}
			//moving y backward
			else if (transform.position.y > y)
			{
				//moving the point by speed 
				targetPosition.y = -speed * Time.deltaTime;
				//check if the point goes more than it should go and if yes clamp it back
				if (targetPosition.y + transform.position.y < y)
				{
					targetPosition.y = -(transform.position.y - y);
				}
			}
			else //y is unchanged so it should be zero
			{
				targetPosition.y = 0;
			}
			//moving z forward
			if (transform.position.z < z)
			{
				//moving the point by speed 
				targetPosition.z = speed * Time.deltaTime;
				//check if the point goes more than it should go and if yes clamp it back
				if (targetPosition.z + transform.position.z > z)
				{
					targetPosition.z = z - transform.position.z;
				}
			}
			//moving z backward
			else if (transform.position.z > z)
			{
				//moving the point by speed 
				targetPosition.z = -speed * Time.deltaTime;
				//check if the point goes more than it should go and if yes clamp it back
				if (targetPosition.z + transform.position.z < z)
				{
					targetPosition.z = -(transform.position.z - z);
				}
			}
			else //z is unchanged so should be zero in translate function
			{
				targetPosition.z = 0;
			}
			transform.Translate(targetPosition);
			yield return 0;
		}
		//the work is ended now congratulation
		canmove = true;
	}
	
}