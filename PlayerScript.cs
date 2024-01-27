using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody rg;
    public float verticalMovement;
    public float horizontalMovement;
    [SerializeField] float speed;
    [SerializeField] float jumpForce;

    [SerializeField] UnityEngine.Vector3 movementDirection;

    public Transform playerCameraDirection;
    public Camera playerCamera;
    Quaternion OG_Player_Rotation;
    [SerializeField] float Sensitivity;

    private WebSocketWorker WSW;
    private GameObject WebSocket;



    // Start is called before the first frame update
    void Start()
    {
        rg = GetComponent<Rigidbody>();
        rg.freezeRotation = true;
        WebSocket = GameObject.Find("WebSocket");
        WSW = WebSocket.GetComponent<WebSocketWorker>();
    }

    // Update is called once per frame
    void Update()
    {
        verticalMovement = Input.GetAxis("Vertical");
        horizontalMovement = Input.GetAxis("Horizontal");


        movementDirection = transform.forward * verticalMovement + transform.right * horizontalMovement;
        if (Input.GetKeyDown(KeyCode.Space) && transform.position.y <= 5.57f)
        {
            rg.AddForce(UnityEngine.Vector3.up * jumpForce, ForceMode.Impulse);
        }
        else
        {
            rg.AddForce(movementDirection * speed, ForceMode.Force);
        }
        OG_Player_Rotation.x += (Input.GetAxis("Mouse Y") * 1 * -1) * Sensitivity;
        OG_Player_Rotation.y += Input.GetAxis("Mouse X") * Sensitivity;

        OG_Player_Rotation.x = Mathf.Clamp(OG_Player_Rotation.x, -45, 75);


        playerCameraDirection.rotation = Quaternion.Euler(OG_Player_Rotation.x, OG_Player_Rotation.y, OG_Player_Rotation.z);

        transform.rotation = Quaternion.Euler(0f, OG_Player_Rotation.y, 0);

        
        
        WSW.SendPlayerData(transform);
        

    }
}
