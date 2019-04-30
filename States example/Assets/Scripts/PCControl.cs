using UnityEngine;
using System.Collections;

public class PCControl : MonoBehaviour
{
	public float y;
	public const float MIN_X= -60;
	public const float MAX_X= 60;
	public const float MIN_Z= -60;
	public const float MAX_Z= 60;
    public GameObject[] enemys;

    //Set/Get/Decrement/Add to Health functions
    public float health;
    public int MaxHealth;
    public float GetHealth() { return health; }
    public void SetHealth(int inHealth) { health = inHealth; }
    public void DecHealth(int amount) { Mathf.Max(0, health - amount); }
    public void AddHealth(int amount) { Mathf.Min(100, health + amount); }

    //if alive then alow movement and attack and stuff
    private bool alive;

    private float speed = 20;
	// Use this for initialization
	void Start ()
    {
		y = transform.position.y;
        alive = true;
    }

    // Update is called once per frame
    void Update()
    {

        //rounds up to 0 for tidyness
        if (health <= 0)
        {
            health = 0;
            alive = false;
        }

        //if alive then alow movement and attack and stuff
        if (alive)
        { 
            KeyboardMovement();
            CheckBounds();

            //Health regen
            if (health < MaxHealth)
            {
                health += 20 * Time.deltaTime;
                if (health > MaxHealth)
                    health = MaxHealth;
            }
        }
    }
	
	private void KeyboardMovement()
	{
		
		float dx = Input.GetAxis("Vertical") * speed * Time.deltaTime * -1.0f;
		float dz = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
		transform.Translate (new Vector3(dx, y, dz));

        //short range attack
        if (Input.GetKeyUp("space"))
        {
            Debug.Log("Attack!");
            foreach (GameObject enemy in enemys)
            if (Mathf.Pow(Mathf.Pow(transform.position.x - enemy.transform.position.x, 2) + Mathf.Pow(transform.position.y - enemy.transform.position.y, 2), 0.5f) <= 5)
            {
                    enemy.GetComponent<AIController>().health -= 25;
                    Debug.Log("Hit");
            }

        }

    }
	
	private void CheckBounds()
	{
		float x = transform.position.x;
		float z = transform.position.z;
		x = Mathf.Clamp (x,MIN_X,MAX_X);
		z = Mathf.Clamp (z,MIN_Z,MAX_Z);
		
		transform.position = new Vector3(x,y,z);
		//print("position " + x + y + z);
	}
}
