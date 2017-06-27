using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour {

    public LayerMask hitMask;
    public bool gunDrawn = false;

    const float collisionBuffer = .1f;
    public int horizontalRayCount = 3;
    public int verticalRayCount = 3;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    BoxCollider2D col;
    RaycastOrigins raycastOrigins;
    SpriteRenderer childSprite;

    // Applies movement after checking for collisions
    public void Move(Vector3 velocity) {
        if (velocity.Equals(Vector3.zero)) return;

        // Update raycasts only when we need to move character
        UpdateRaycastOrigins();

        // Check for collisions only if we are moving in the respective direction
        if (velocity.x != 0) {
            HorizontalCollisions(ref velocity);
        }
        if (velocity.y != 0) {
            VerticalCollisions(ref velocity);
        }

        transform.Translate(velocity);
    }

    // Applies a rotation from the previous location point
    public void Rotate(Vector3 velocity) {
        if (velocity.Equals(Vector3.zero)) return;
        // Rotate player
        float angle = Mathf.Atan2(velocity.x, velocity.y) * Mathf.Rad2Deg;
        GetComponentInChildren<SpriteRenderer>().transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.back);
    }

    // Draw (or holster) gun
    public void DrawGun() {
        gunDrawn = !gunDrawn;
    }

    void HorizontalCollisions(ref Vector3 velocity) {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + collisionBuffer;

        for (int i = 0; i < horizontalRayCount; i++) {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * horizontalRaySpacing * i;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, hitMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit) {
                velocity.x = (hit.distance - collisionBuffer) * directionX;
                rayLength = hit.distance;
            }
        }
    }

    void VerticalCollisions(ref Vector3 velocity) {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + collisionBuffer;

        for (int i = 0; i < verticalRayCount; i++) {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, hitMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit) {
                velocity.y = (hit.distance - collisionBuffer) * directionY;
                rayLength = hit.distance;
            }
        }
    }

    void Start() {
        col = GetComponent<BoxCollider2D>();
        childSprite = GetComponentInChildren<SpriteRenderer>();

        // Calculate the spacing of each ray
        Bounds bounds = col.bounds;
        bounds.Expand(collisionBuffer * -2);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    void UpdateRaycastOrigins() {
        Bounds bounds = col.bounds;
        bounds.Expand(collisionBuffer * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing() {
        Bounds bounds = col.bounds;
        bounds.Expand(collisionBuffer * -2);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    struct RaycastOrigins {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
