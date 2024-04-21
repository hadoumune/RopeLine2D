using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PegC.Util{

[RequireComponent(typeof(LineRenderer))]
public class RopeLine2D : MonoBehaviour
{
	static int RopeSegmentCount = 16;
	LineRenderer _lineRenderer;
	LineRenderer lineRenderer => _lineRenderer??=GetComponent<LineRenderer>();

	Vector2 controlBegin = Vector2.zero;
	Vector2 controlEnd = Vector2.zero;
	Vector2 controlPoint = Vector2.zero;
	Vector2 targetControlPoint = Vector2.zero;
	bool stableRope = false;

	[SerializeField]
	float _lineWidth = 0.2f;
	float lineWidth = 0.2f;

	[SerializeField]
	bool _useRopeBehaviour = false;
	bool useRopeBehaviour = false;

	[SerializeField]
	bool _isVisible = true;
	bool isVisible = true;

	[SerializeField]
	float _ropeTension = 0.5f;
	float ropeTension = 0.5f;

	public void SetVisible(bool visible){
		lineRenderer.enabled = visible;
		isVisible = visible;
	}

	public void SetWidth(float width){
		lineRenderer.startWidth = width;
		lineRenderer.endWidth = width;
		lineWidth = width;
	}

	public void SetTension(float tension){
		ropeTension = tension;
		calcControlPoint(controlBegin,controlEnd);
	}

	public void SetRopBehaviour(bool useRope){
		useRopeBehaviour = useRope;
		if ( useRopeBehaviour ){
			lineRenderer.positionCount = RopeSegmentCount;
		}
		else{
			lineRenderer.positionCount = 2;
		}
		calcControlPoint(controlBegin,controlEnd);
	}

	Vector2 calcBezier(Vector2 begin, Vector2 end,Vector2 mid,float t){
		var _2mid = 2.0f*mid;
		return (begin -_2mid + end)*t*t+(-2.0f*begin+_2mid)*t+begin;
	}

	// 垂直に近づくほど垂れないようにする.
	void calcControlPoint(Vector2 begin, Vector2 end){
		var midPoint = (begin + end)*0.5f;
		targetControlPoint = midPoint + (-1.0f*ropeTension * Mathf.Abs(Vector2.Dot(Vector2.right,begin-end)) * Vector2.up);
		stableRope = false;
		recalcRope();
	}

	void recalcRope(){
		for( int idx = 0 ; idx < lineRenderer.positionCount ; idx++ ){
			float t = (float)idx/(float)(lineRenderer.positionCount-1);
			var v = calcBezier(controlBegin,controlEnd,controlPoint,t);
			lineRenderer.SetPosition(idx,v);
		}
	}

	public void ChangePosition(Vector2 start, Vector2 end){
		controlBegin = start;
		controlEnd = end;
		calcControlPoint(controlBegin,controlEnd);
	}

	public void ChangePositionEnd(Vector2 end){
		controlEnd = end;
		calcControlPoint(controlBegin,controlEnd);
	}
	public void ChangePositionBegin(Vector2 start){
		controlBegin = start;
		calcControlPoint(controlBegin,controlEnd);
	}

	// Start is called before the first frame update
	void Awake(){
	}
	void Start()
	{
		lineRenderer.useWorldSpace = true;
		SetRopBehaviour(_useRopeBehaviour);
		SetVisible(_isVisible);
		SetWidth(_lineWidth);
		SetTension(_ropeTension);
	}

	// Update is called once per frame
	void LateUpdate()
	{
		if ( _isVisible != isVisible ){
			SetVisible(_isVisible);
		}

		if ( _useRopeBehaviour != useRopeBehaviour){
			SetRopBehaviour(_useRopeBehaviour);
		}

		if ( _ropeTension != ropeTension ){
			SetTension(_ropeTension);
		}

		if ( _lineWidth != lineWidth ){
			SetWidth(_lineWidth);
		}

		//if ( !useRopeBehaviour ) return;
		var dist = targetControlPoint - controlPoint;
		if ( dist.sqrMagnitude < 0.0001f ){
			controlPoint = targetControlPoint;
			stableRope = false;
		}

		if ( !stableRope ){
			controlPoint = controlPoint+dist*0.01f;
			recalcRope();
		}

	}
}
}