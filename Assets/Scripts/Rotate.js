var rotateSpeedX : float;
var rotateSpeedY : float;
var rotateSpeedZ : float;



function Update () {

transform.Rotate(rotateSpeedX*Time.deltaTime,rotateSpeedY*Time.deltaTime,rotateSpeedZ*Time.deltaTime);

}