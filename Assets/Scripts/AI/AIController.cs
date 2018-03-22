using UnityEngine;

/// <summary>
/// Movement controller for AI. Uses machine learning model to predict next best move.
/// </summary>
public class AIController : MonoBehaviour {

	// Time between every movement check
	const float MOVE_RATE = 3f / 3f;

	public float moveSpeed = 3f;
	PlayerController con;

	// Feature related variables
	public float sinceMotion = 0;
	public float sinceDirection = 0;
	public bool inMotion = false;

	// The AI's input vector
	Vector2 direction = Vector2.zero;

	void Start () {
		con = GetComponent<PlayerController>();
	}

	void Update() {
		sinceMotion += Time.deltaTime;
		sinceDirection += Time.deltaTime;

		if (inMotion && (direction == Vector2.zero)) {
			sinceMotion = 0;
		} else if (!inMotion && (direction != Vector2.zero)) {
			sinceMotion = 0;
		}

		inMotion = (direction != Vector2.zero);

		// Move the AI in the predicted direction
		Vector3 velocity = Vector3.zero;
		velocity.x = direction.x * moveSpeed;
		velocity.y = direction.y * moveSpeed;
		con.Move(velocity * Time.deltaTime);
	}
}
