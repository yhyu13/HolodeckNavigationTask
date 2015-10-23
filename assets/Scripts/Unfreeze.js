var Unfreeze : GameObject;

function Update () 
{
	if (Input.GetKeyDown ("x"))
	{
		Unfreeze.GetComponent("BoxCollider").enabled=false;
	}
}