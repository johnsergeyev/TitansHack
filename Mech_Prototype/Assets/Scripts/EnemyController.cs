using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {
	public GameObject target; 
	private NavMeshAgent agent;
	private Animator animator;

	private Vector3 lastPos;
	private Vector3 actualSpeed = Vector3.zero; 

	void Start () {
		agent = GetComponent<NavMeshAgent> ();

		animator = GetComponent<Animator> ();
		lastPos = transform.position;

		StartCoroutine ("CalcSpeed");
	}

	void Update () {
		animator.SetFloat ("speed", actualSpeed.z);
		animator.SetFloat ("direction", actualSpeed.x);

		agent.SetDestination (target.transform.position);
	}

	IEnumerator CalcSpeed() {
		while (Application.isPlaying) {
			lastPos = transform.position;

			yield return new WaitForEndOfFrame ();

			actualSpeed = (transform.position - lastPos) / Time.deltaTime;
			actualSpeed = transform.InverseTransformDirection (actualSpeed);
			actualSpeed /= agent.speed;
		}
	}
}
