#pragma strict

var NewTexture : Texture;
var DestroyCollider : GameObject;

function Update () 
{
	if (gameObject.GetComponent.<Collider>().enabled == false)
	{
		gameObject.GetComponent.<Renderer>().material.mainTexture = NewTexture;
		DestroyCollider.GetComponent.<Collider>().enabled = false;
	}
}