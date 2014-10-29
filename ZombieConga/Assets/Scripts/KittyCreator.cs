using UnityEngine;
using System.Collections;

public class KittyCreator : MonoBehaviour {
	#region vars
	//instance spawn time thresholds
	public float minSpawnTime = 0.75f;
	public float maxSpawnTime = 2f;

	//ref to cat prefab
	public GameObject catPrefab;
	#endregion

	#region on start
	void Start () {
		//init first call
		Invoke("SpawnCat", minSpawnTime);
	}
	#endregion
	
	#region on frame
	void Update () {

	}
	#endregion

	#region functions
	//function for creating cat instance
	void SpawnCat() {
		//get camera ref and its position ref
		Camera camera = Camera.main;
		Vector3 cameraPos = camera.transform.position;
		//half horizontal view size
		float xMax = camera.aspect * camera.orthographicSize;
		//calculate spawn area's max horizontal value
		//'camera.orthographicSize * 2.0f' is full area value, 1.75f gives safe bounds
		float xRange = camera.aspect * camera.orthographicSize * 1.75f;
		//half vertical view size
		float yMax = camera.orthographicSize - 0.5f;

		//calculate new cat pos in current visible camera view
		Vector3 catPos = new Vector3(
			//random x pos from negative to max x value
			cameraPos.x + Random.Range(xMax - xRange, xMax),
			//camera not moving in y direction,
			//no need to consider current camera y pos
			Random.Range(-yMax, yMax),
			//copy z pos from existing instance
			//so all cats have same depth
			catPrefab.transform.position.z);

		//create prefab instance
		//set rotation to 'identity rotation', corresponds to "no rotation"
		Instantiate(catPrefab, catPos, Quaternion.identity);

		//queue subsequent call
		Invoke("SpawnCat", Random.Range(minSpawnTime, maxSpawnTime));
	}
	#endregion
}
