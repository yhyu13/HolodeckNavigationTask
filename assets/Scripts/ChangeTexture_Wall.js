#pragma strict

var NewTexture : Texture;

var Back : GameObject;
var Bottom : GameObject;
var Front : GameObject;
var Left : GameObject;
var Right : GameObject;
var Top : GameObject;

var Blockade : GameObject;

function Update () 
{
	if (gameObject.GetComponent.<Collider>().enabled == false)
	{
		Back.GetComponent.<Renderer>().material.mainTexture = NewTexture;
		Bottom.GetComponent.<Renderer>().material.mainTexture = NewTexture;
		Front.GetComponent.<Renderer>().material.mainTexture = NewTexture;
		Left.GetComponent.<Renderer>().material.mainTexture = NewTexture;
		Right.GetComponent.<Renderer>().material.mainTexture = NewTexture;
		Top.GetComponent.<Renderer>().material.mainTexture = NewTexture;
		
		Back.GetComponent.<Collider>().enabled = false;
		Bottom.GetComponent.<Collider>().enabled = false;
		Left.GetComponent.<Collider>().enabled = false;
		Right.GetComponent.<Collider>().enabled = false;
		Top.GetComponent.<Collider>().enabled = false;		
								
		Blockade.GetComponent.<Collider>().enabled = false;
	}
}