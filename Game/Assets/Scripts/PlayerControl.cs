﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour {


	//public variables
	public float speed; //speed of the player
	public float maxSpeed = 5f; //maximum speed of the player
	public float jumpForce; //contorls how high the player will jump
	public float bulletSpeed = 4f;
	public GameObject[] projectiles; //array of what the player can shoot

	//private variables
	GameControl gc;
	Rigidbody2D rb; //rigidbody of the player
	float horiz = 0f; //the value of the horizontal axis

	bool jump = false; //whether the player is jumping or not
	int numJumps= 0; //how many jumps the player is allowed to have
	int proj_num = 0; //decides which projectile to shoot from the array
	float currentHealth;

//	bool left = false; //whether the player is facing left or not
	Vector3 startPosition; //stores the starting position of the ball

	//===================================================================================================================================
	//Unity Functions
	// Use this for initialization
	void Start () {
		gc = GameObject.Find ("GameController").GetComponent<GameControl> ();
		rb = GetComponent<Rigidbody2D> (); //get reference to the rigidbody of the player
		startPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		//-------------------------------------------------------------------------------------------------------------------------------
		//Stuff to do with shooting
		//if left mouse button is pressed - call Fire function to fire bullet
		if (Input.GetMouseButtonDown (0)) {
			Shoot ();
		}
		//goes through the bullets in the array every time the right mouse button is pressed
		if (Input.GetMouseButtonDown (1)) {
			proj_num++;
			if(proj_num >= projectiles.Length) //if the proj_numb is more than or equal to the length of the projectiles array
			{
				proj_num = 0;  //then reset the proj_num to the beginning of the array
			}

			gc.changeBullet(projectiles.Length, proj_num);
		}

		//---------------------------------------------------------------------------------------------------------------------------------
		//Stuff to do with moving
		horiz = Input.GetAxis ("Horizontal"); //get value of horizontal access

		//press jump key
		//if space is pressed set jump to true so you can jump
		if (Input.GetKeyDown (KeyCode.Space)) {
			jump = true;
		}
	}

	//used fixed update for changes to physics
	void FixedUpdate()
	{
		//jump
		if (jump == true && numJumps == 0) {
			rb.AddForce(new Vector2(0f, jumpForce)); //if jump is true then add a force in the y direction to the player
			numJumps++;
			jump = false; //set jump to false so that not continuously adding the upward force
		}

		//general moving
		rb.AddForce (Vector2.right * horiz * speed); //add a force to the player in the x direction
		if (Mathf.Abs (rb.velocity.x) > maxSpeed)  //if the player's speed is more than the maximum speed:
		{
			rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y); //set the speed back to the maximum
		}
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		//if the player hits the killzone then bring it back to the beginning position
		if (col.gameObject.tag == "Killzone") {
			transform.position = startPosition;
			gc.changeHealth(10);
		}

		//once the player lands numJumps is reset
		if (col.gameObject.tag == "block") {
			numJumps = 0;
		}

		if (col.gameObject.tag == "Switch") {
			gc.turnOn();
		}

		if(col.gameObject.tag == "Doors")
		{
			gc.reachDoors();
		}

		if(col.gameObject.tag == "Enemy")
		{
			rb.AddForce(new Vector2(-(rb.velocity.x * 900f), 0f));
			gc.changeHealth(10);
		}
	}

	//========================================================================================================================================
	//custom functions
	//private function by default
	void Shoot()
	{
		Vector3 target = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Vector3 direction = target - transform.position;
		direction.Normalize ();
		Vector3 playerPos = new Vector3 (transform.position.x + (direction.x), transform.position.y + (direction.y), transform.position.z);
		GameObject bullet = (GameObject)Instantiate (projectiles [proj_num], playerPos, Quaternion.identity);
		bullet.GetComponent<Rigidbody2D> ().velocity = direction * bulletSpeed;
	}
	
}
