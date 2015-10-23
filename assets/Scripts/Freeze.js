
var Freeze : GameObject;

function OnTriggerEnter(other : Collider) 
{
	Freeze.GetComponent("CharacterMotor").enabled = false;
	
	print("first base");
}
