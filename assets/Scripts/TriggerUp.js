var target : GameObject;
var eventName1 : String;

function OnTriggerEnter () 
{
iTweenEvent.GetEvent(target, eventName1).Play();
}

