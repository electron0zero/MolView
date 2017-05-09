using UnityEngine;
 
public class FollowTrackingCamera : MonoBehaviour
{
    // Camera target to look at.
    public Transform target;

    // Exposed vars for the camera position from the target.
    public float height = 20f;
    public float distance = 20f;

    // Camera limits.
    public float min = 10f;
    public float max = 60f;

    // camera FOV for pinch to zoom (Zoom on Touch devices)
    // private float cameraFOVMin = 10f;
    // private float cameraFOVMax = 100f;
    // Options
    // public bool smoothZoom = true;

    // The movement amount when zooming.
    private float zoomStep = 30f;
    // private float zoomSpeed = 5f;
    private float heightWanted;
    private float distanceWanted;

    // Result vectors.
    private Vector3 zoomResult;
    private Vector3 targetAdjustedPosition;

    void Start(){
        // Initialize default zoom values.
        heightWanted = height;
        distanceWanted = distance;

        // Setup our default camera.  We set the zoom result to be our default position.
        zoomResult = new Vector3(0f, height, -distance);

    }

    void LateUpdate(){
        // Check target.
        if( !target ){
            Debug.LogError("This camera has no target, you need to assign a target in the inspector.");
            return;
        }

        zoomer();
    }

    void zoomTouch(){
        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            zoomToDistance(-(deltaMagnitudeDiff/100f));

        }
    }

    void zoomScroll(){
        // Record our mouse input.  If we zoom add this to our height and distance.
        float mouseInput = Input.GetAxis("Mouse ScrollWheel");
        zoomToDistance(mouseInput);
    }

    void zoomToDistance(float distance){
        // Debug.Log("distance to zoom:" + distance);
            heightWanted -= zoomStep * distance;
            distanceWanted -= zoomStep * distance;

            // Make sure they meet our min/max values.
            heightWanted = Mathf.Clamp(heightWanted, min, max);
            distanceWanted = Mathf.Clamp(distanceWanted, min, max);

            // if (smoothZoom){
            //     // Linearly interpolate to achieve smooth camera effect
            //     height = Mathf.Lerp(height, heightWanted, Time.deltaTime * zoomSpeed);
            //     distance = Mathf.Lerp(distance, distanceWanted, Time.deltaTime * zoomSpeed);
            // } else {
                // Rough Zoom effect
                height = heightWanted;
                distance = distanceWanted;
            // }

            // Post our result.
            zoomResult = new Vector3(0f, height, -distance);
            
            // Set the camera position reference.
            transform.position = target.position + zoomResult;
            // Face the desired position.
            transform.LookAt(target);
    }

    void zoomer(){
        //Check if we are running on iOS, Android, Windows Phone 8 or iPhone
        #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
            zoomTouch();
        //anywhere else use mouse controls
        #else
            zoomScroll();
        #endif
        // End of mobile platform dependent compilation section started above with #if
    }
}