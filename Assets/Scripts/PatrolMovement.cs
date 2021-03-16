using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolMovement : MonoBehaviour
{
	[Tooltip("Player speed between waypoints")]
	[SerializeField] float speed = 1f;

	GameObject[] waypoints;
	int targetWaypoint;
	Vector3 targetWaypointPosition;
	int lastWayPoint;
	Color newColor;


	void Start()
	{
		waypoints = GetWaypoints();
		targetWaypoint = 0;
		targetWaypointPosition = waypoints[targetWaypoint].transform.position;
		lastWayPoint = waypoints.Length - 1;
	}

	void Update()
	{
		Move();
		TransitionBetweenWaypoints();
	}

	GameObject[] GetWaypoints()
	{
		List<GameObject> waypoints = new List<GameObject>();

		var allSceneObjects = GameObject.FindObjectsOfType<GameObject>();
		foreach (var gObject in allSceneObjects)
		{
			if (gObject.tag == "Untagged")
				waypoints.Add(gObject);
		}
		return waypoints.ToArray();
	}

	public void Move()
	{
		float stepDistance = speed * Time.deltaTime;

		targetWaypointPosition = waypoints[targetWaypoint].transform.position;
		transform.position = Vector3.MoveTowards(transform.position, targetWaypointPosition, stepDistance);
		transform.LookAt(waypoints[targetWaypoint].transform);
	}

	void TransitionBetweenWaypoints()
	{
		if (DistanceBetwen(transform.position, targetWaypointPosition) <= Mathf.Epsilon)
		{
			targetWaypoint = GetNextWaypoint();
			ChangeMaterialColor();
		}
	}

	public float DistanceBetwen(Vector3 pos1, Vector3 pos2)
	{
		Vector3 result = pos1 - pos2;

		return Mathf.Abs(result.magnitude);
	}

	public int GetNextWaypoint()
	{
		int nextWaypoint = targetWaypoint;

		nextWaypoint = targetWaypoint + 1 <  waypoints.Length ? ++nextWaypoint : 0;
		return nextWaypoint;
	}

	void ChangeMaterialColor()
	{
		newColor = Random.ColorHSV();
		GetComponent<Renderer>().material.SetColor("_Color", newColor);
		ChangeColorInChilds();
	}

	void ChangeColorInChilds()
	{
		Renderer[] childRenderers = GetComponentsInChildren<Renderer>();

		foreach (var renderer in childRenderers)
		{
			renderer.material.SetColor("_Color", newColor);
		}
	}
	
	private void OnDrawGizmos()
	{
		//GameObject prevWaypoint = null;
		Gizmos.color = Color.red;

		DrawGizmosAreaAndLines();
	}

	void DrawGizmosAreaAndLines()
	{
		GameObject prevWaypoint = null;

		foreach (var waypoint in waypoints)
		{
			Gizmos.DrawWireSphere(waypoint.transform.position, 1);
			if (prevWaypoint != null) {
				Gizmos.DrawLine(prevWaypoint.transform.position, waypoint.transform.position);
			}
			prevWaypoint = waypoint;
		}
		Gizmos.DrawLine(waypoints[0].transform.position, waypoints[lastWayPoint].transform.position);

	}
}
