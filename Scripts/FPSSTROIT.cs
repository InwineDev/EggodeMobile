using UnityEngine;

public class FPSSTROIT : MonoBehaviour
{
    public float speed = 10.0f;
    public float jumpForce = 10.0f;
    public float gravity = -9.81f;
    public float rotationSpeed = 100.0f;
    public float scaleSpeed = 1.0f;
    public float moveSpeed = 10.0f;
    public GameObject objectToCreate;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Get the input from the keyboard and mouse.
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float rotationInput = Input.GetAxis("Mouse X");
        float scaleInput = Input.GetAxis("Mouse ScrollWheel");

        // Move the player in the horizontal and vertical directions.
        rb.AddForce(new Vector3(horizontalInput * speed, 0, verticalInput * speed));

        // Make the player jump when the spacebar is pressed.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector3(0, jumpForce, 0));
        }

        // Apply gravity to the player.
        rb.AddForce(new Vector3(0, gravity, 0));

        // Rotate the object around the Y axis when the left mouse button is held down.
        if (Input.GetMouseButton(0))
        {
            transform.Rotate(new Vector3(0, rotationInput * rotationSpeed * Time.deltaTime, 0));
        }

        // Scale the object up or down when the scroll wheel is used.
        transform.localScale += new Vector3(scaleInput * scaleSpeed, scaleInput * scaleSpeed, scaleInput * scaleSpeed);

        // Move the object in the forward direction when the W key is pressed.
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }

        // Move the object in the backward direction when the S key is pressed.
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }

        // Move the object to the left when the A key is pressed.
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }

        // Move the object to the right when the D key is pressed.
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }

        // Create a new object when the left mouse button is clicked.
        if (Input.GetMouseButtonDown(0))
        {
            GameObject newObject = Instantiate(objectToCreate, transform.position, transform.rotation);
        }
    }
}