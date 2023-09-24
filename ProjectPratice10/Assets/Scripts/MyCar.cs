using UnityEngine;

public enum driveType
{
    FRONTDRIVE,
    REARDRIVE,
    ALLDRIVE
}

public class MyCar : MonoBehaviour
{
    public driveType drive;

    public WheelCollider[] wheels = new WheelCollider[4];
    private GameObject[] wheelMesh = new GameObject[4];


    public float power = 100f; // ������ ȸ����ų ��
    public float rot = 35f; // ������ ȸ�� ����

    private Rigidbody rigid;

    public float radius = 1;

    InputManager inputManager;

    public float testX = 0;
    public float testY = 0;
    
    

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        inputManager = gameObject.AddComponent<InputManager>();
    }

    void Start()
    {
        wheelMesh = GameObject.FindGameObjectsWithTag("WheelMesh");
        for (int i = 0; i < wheelMesh.Length; i++)
        {
            wheels[i].transform.position = wheelMesh[i].transform.position;
        }
        rigid.centerOfMass = new Vector3(0, -1, 0); //���� �߽� y������ ����
    }

    private void FixedUpdate()
    {
        MoveVehicle();
        SteerVehicle();
        var rotation = gameObject.transform.eulerAngles;
        rotation.z = 0;
        transform.rotation = Quaternion.Euler(rotation);
    }

    void MoveVehicle()
    {
        float motorInput = Input.GetAxis("Vertical");
        float steeringInput = Input.GetAxis("Horizontal");

        // ��� ������ ���� motorTorque ����
        foreach (WheelCollider wheel in wheels)
        {
            // �������濡�� �극��ũ�� ����Ͽ� �ӵ��� ����
            if (motorInput < 0 && wheel.isGrounded)
            {
                wheel.brakeTorque = power;
            }
            else
            {
                wheel.brakeTorque = 0f;
                wheel.motorTorque = motorInput * power;
            }
        }

        //�չ������� steerAngle ����
        
        for(int i = 0; i < 2; i++)
        {
            wheels[i].steerAngle = steeringInput * rot;
        }

    }

    void SteerVehicle()
    {
        // ��Ŀ�� ����
        if (inputManager.horizontal > 0)
        {   // rear tracks size is set to 1.5f          wheel base has been set to 2.55f
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * inputManager.horizontal;
             wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * inputManager.horizontal;
            //transform.Rotate(Vector3.up * inputManager.horizontal);
        }
        else if (inputManager.horizontal < 0)
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * inputManager.horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * inputManager.horizontal;
            //transform.Rotate(Vector3.up * inputManager.horizontal);
        }
    }
    

    private void WheelPosAndAni()
    {
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        for (int i = 0; i < 4; i++)
        {
            wheels[i].GetWorldPose(out wheelPosition, out wheelRotation);
            wheelMesh[i].transform.position = wheelPosition;
            wheelMesh[i].transform.rotation = wheelRotation;
        }
    }
}
