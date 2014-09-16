#pragma strict

// line start and finish targets' transform objects info
var post1 : Transform;
var post2 : Transform;

// on object start
function Start () {
    // Set line renderer and material
    var lineRenderer : LineRenderer = gameObject.AddComponent(LineRenderer);
	var lineMat : Material = new Material (Shader.Find ("Particles/Multiply"));
    
    // set position and assign material
    lineRenderer.SetPosition(0, post1.position);
    lineRenderer.SetPosition(1, post2.position);
    lineRenderer.material = lineMat;
    lineRenderer.SetColors(new Color(255, 0, 0, 0.5), new Color(0, 255, 0, 0.5));
}