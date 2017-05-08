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
    public float cameraFOVMin = 10f;
    public float cameraFOVMax = 100f;
    // Options
    public bool smoothZoom = true;

    // The movement amount when zooming.
    public float zoomStep = 30f;
    public float zoomSpeed = 5f;
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
        // Debug.Log(zoomResult);
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

            // If the camera is orthographic...
            Camera cam = GetComponent<Camera>();
            if (cam.orthographic)
            {
                // ... change the orthographic size based on the change in distance between the touches.
                cam.orthographicSize += deltaMagnitudeDiff * zoomSpeed;

                // Make sure the orthographic size never drops below zero.
                cam.orthographicSize = Mathf.Max(cam.orthographicSize, 0.1f);
            }
            else
            {
                // Otherwise change the field of view based on the change in distance between the touches.
                cam.fieldOfView += deltaMagnitudeDiff * zoomSpeed;

                // Clamp the field of view to make sure it's between 0 and 180.
                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, cameraFOVMin, cameraFOVMax);
            }
        }
    }

    void zoomScroll(){
        // Record our mouse input.  If we zoom add this to our height and distance.
        float mouseInput = Input.GetAxis("Mouse ScrollWheel");
        heightWanted -= zoomStep * mouseInput;
        distanceWanted -= zoomStep * mouseInput;

        // Make sure they meet our min/max values.
        heightWanted = Mathf.Clamp(heightWanted, min, max);
        distanceWanted = Mathf.Clamp(distanceWanted, min, max);

        if (smoothZoom){
            // Linearly interpolate to achieve smooth camera effect
            height = Mathf.Lerp(height, heightWanted, Time.deltaTime * zoomSpeed);
            distance = Mathf.Lerp(distance, distanceWanted, Time.deltaTime * zoomSpeed);
        } else {
            // Rough Zoom effect
            height = heightWanted;
            distance = distanceWanted;
        }

        // Post our result.
        zoomResult = new Vector3(0f, height, -distance);
        
        // Set the camera position reference.
        transform.position = target.position + zoomResult;
        // Face the desired position.
        transform.LookAt(target);
    }

    void zoomer(){
        //Check if we are running either in the Unity editor or in a standalone build.
        #if UNITY_STANDALONE || UNITY_WEBPLAYER
            zoomScroll();
        //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
        #elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
            zoomTouch();
        #endif
        // End of mobile platform dependent compilation section started above with #if
    }
}