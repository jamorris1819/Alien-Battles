using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    public Transform target;

	void Start () {
	
	}
	
	void Update () {
        Vector3 targetPos;
        Debug.Log(Vector3.Distance(target.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        if (Vector3.Distance(target.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)) < 2f)
            targetPos = (target.position + Camera.main.ScreenToWorldPoint(Input.mousePosition)) / 2;
        else
            targetPos = target.position;
        targetPos.z = -10f;
        transform.position = Vector3.Lerp(transform.position, targetPos, 0.2f);
	}

    public void Kickback(Vector3 direction)
    {
        transform.position += direction;
    }

    public void Shake(float amount)
    {
        float x = Random.Range(-amount, amount);
        float y = Random.Range(-amount, amount);
        transform.position += new Vector3(x, y);
    }
}
