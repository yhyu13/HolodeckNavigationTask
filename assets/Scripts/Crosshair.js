private var CrosshairTexture : Texture2D;
 
var NormalTexture : Texture2D;
var ShowCrosshair : boolean = true;

function Start()
{      
        Screen.lockCursor = true;
        CrosshairTexture = NormalTexture;
}
                      
function OnGUI()
{
        if(ShowCrosshair == true)
        {
        GUI.Label(Rect((Screen.width - CrosshairTexture.width) /2, (Screen.height -
                        CrosshairTexture.height) /2, CrosshairTexture.width, CrosshairTexture.height), CrosshairTexture);
        }
 
}