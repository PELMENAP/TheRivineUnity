using UnityEngine;

public class CM : MonoBehaviour
{
    [SerializeField] private Transform cameratrans;
    [SerializeField] private float Velocity, MinDistance;
    [SerializeField] private Camera mainCam;
    public static bool cameraForMap;
    private Vector3 offset, playerOffset;
    private Transform playerTransform;
    private bool changeCam = false;

    private void Start()
    {
        cameraForMap = false;
        playerTransform = PlayerController.playerTrans;
        offset = cameratrans.position - playerTransform.position;
        cameratrans.position = playerTransform.position + new Vector3(0, 0, -1);
    }

    private void LateUpdate()
    {
        if (changeCam || Input.GetKeyDown("p")) {
            if(cameraForMap){
                playerTransform.gameObject.SetActive(true);
                cameratrans.position = playerOffset;
                cameratrans.position += new Vector3(0, 0, -1); 
                mainCam.orthographicSize = 20;
            }
            else{
                playerTransform.gameObject.SetActive(false);
                cameratrans.position += new Vector3(0, 0, 99);
                playerOffset = playerTransform.position;
                mainCam.orthographicSize = 100;
            }
            cameraForMap = !cameraForMap;
            changeCam = false;
        }
        if(cameraForMap){
            UpdateForMap();
        }
        else{
            UpdateForGame();
        }
    }

    public void Changed(){
        changeCam = true;
    }

    private void UpdateForGame() {
        var targetPos = playerTransform.position + offset;
        if (playerTransform == null || Vector3.Distance(cameratrans.position, targetPos) < MinDistance)
        {
            return;
        }
        cameratrans.Translate(cameratrans.InverseTransformPoint(Vector3.Lerp(cameratrans.position, targetPos, Velocity * Time.fixedDeltaTime)));
    }

    private void UpdateForMap(){
        if(Input.GetKeyDown("=")){
            mainCam.orthographicSize -= 20;
        }
        else if(Input.GetKeyDown("-")){
            mainCam.orthographicSize += 20;
        }
        else if (Input.mouseScrollDelta.y != 0){
            mainCam.orthographicSize += Input.mouseScrollDelta.y * 20 * mainCam.orthographicSize / 300;
            this.transform.Translate(new Vector3((Input.mousePosition.x - Screen.width/2) * 0.5f, (Input.mousePosition.y - Screen.height/2) * 0.5f, 0) * (Input.mouseScrollDelta.y > 0 ? -1 : 1) * mainCam.orthographicSize / 200);
        }
        if(mainCam.orthographicSize > 1000){
            mainCam.orthographicSize = 1000;
        }
        else if(mainCam.orthographicSize < 10){
            mainCam.orthographicSize = 10;
        }
    }
}

