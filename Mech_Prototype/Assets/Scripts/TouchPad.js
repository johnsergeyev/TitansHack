#pragma strict
@script RequireComponent( GUITexture )
var Ccamera:Camera;
var playerObject : Transform;
var touchCount = 0;

//var xRotation :float;
//var yRotation :float;
var originalR : float;
var lookSpeed :int = 20;
var ddd:GameObject;
private var horizontalRotation : float;
private var verticalRotation : float;

var maxLookAxis : float = 80;
var minLookAxis : float = -80;


function Update()
{
   if (Input.touchCount == 1)
   {
      var touch : Touch = Input.GetTouch(0);
      
      if(touch.phase == TouchPhase.Moved)
      {
      
         horizontalRotation = lookSpeed * touch.deltaPosition.x ;
         Ccamera.transform.Rotate(0, -horizontalRotation, 0, Space.World);
 
         verticalRotation = lookSpeed * touch.deltaPosition.y ;
         Ccamera.transform.Rotate(verticalRotation, 0, 0, Space.World);
      }
   }
}