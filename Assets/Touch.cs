 using UnityEngine;
 using System.Collections;
 
 public class Touch : MonoBehaviour {
  
    [SerializeField] Transform target;
    float speed = 6f;
    Vector2 targetPos;
    public bool touched = false;
    public bool moused = false;
 
	private void Start()
	{
		targetPos = transform.position;
	}
 
	void Update ()
	{
        // Touched
        if (Input.touches.Length > 0) {
            if (Input.touches[0].phase == TouchPhase.Began) {
                targetPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.touches[0].position);
                if (GetComponent<Collider2D>() == Physics2D.OverlapPoint(targetPos))
                    touched = true;
            }
            if (touched) {
                targetPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.touches[0].position);
                target.position = new Vector3(targetPos.x, targetPos.y, 2);
            }
            if (Input.touches[0].phase == TouchPhase.Ended)
                touched = false;

            // Mouse
            
        }
        if (Input.GetMouseButtonDown(0)) {
            targetPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (GetComponent<Collider2D>() == Physics2D.OverlapPoint(targetPos))
                moused = true;
        }
        if (moused) {
            targetPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.position = new Vector3(targetPos.x, targetPos.y, 2);
        }
        if (Input.GetMouseButtonUp(0))
            moused = false;
    }

    public bool Touched(){
        return moused || touched;
    }
 }