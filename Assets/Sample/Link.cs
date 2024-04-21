using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PegC.Util;

public class Link : MonoBehaviour
{
	public Transform target;

	public RopeLine2D ropeLine;


	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		ropeLine.ChangePosition(transform.position,target.position);
	}
}
