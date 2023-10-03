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


    public float power = 100f; // 바퀴를 회전시킬 힘
    public float rot = 35f; // 바퀴의 회전 각도

    private Rigidbody rigid;

    public float radius = 1;

    public float testX = 0;
    public float testY = 0;

    int minSpeed = 0;
    int maxSpeed = 10;
    int nowSpeed = 5;




    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Start()
    {
        wheelMesh = GameObject.FindGameObjectsWithTag("WheelMesh");
        for (int i = 0; i < wheelMesh.Length; i++)
        {
            wheels[i].transform.position = wheelMesh[i].transform.position;
        }
        rigid.centerOfMass = new Vector3(0, -0.1f, 0); //무게 중심 y축으로 설정
    }

    private void FixedUpdate()
    {
        WheelControl();
        //WheelPosAndAni();
        DriftControl();

        
        var rotation = gameObject.transform.eulerAngles;
        rotation.z = 0;
        transform.rotation = Quaternion.Euler(rotation);
        
    }

    void WheelControl()
    {
        float motorInput = Input.GetAxis("Vertical");
        float steeringInput = Input.GetAxis("Horizontal");

        for (int i = 0; i < 2; i++)
        {
            if(steeringInput != 0)
            {
                var rotValue = rot * steeringInput;
                wheels[i].steerAngle = rotValue;
                wheels[i+1].motorTorque -= wheels[i + 1].motorTorque * steeringInput * Mathf.Clamp(rigid.velocity.magnitude,0,10) * 5;
                WheelPosAndAni();
            }
            else
            {
                wheels[i].steerAngle = 0;
                wheels[i + 1].brakeTorque = 0;
            }
        }

        for(int i = 2; i < 4; i++)
        {
            var speed = power * nowSpeed * Mathf.Clamp(rigid.velocity.magnitude, 0,maxSpeed);
            Debug.Log(Mathf.Clamp(rigid.velocity.magnitude, 0, maxSpeed));
            Debug.Log(wheels[i].motorTorque);
            wheels[i].motorTorque = Mathf.Clamp(speed, 0, 20000
                );

            rigid.velocity = new Vector3(Mathf.Clamp(rigid.velocity.x, -10, 10),
                Mathf.Clamp(rigid.velocity.y, -10, 10),
                Mathf.Clamp(rigid.velocity.z, -10, 10));
            Debug.Log($"{rigid.velocity.x} / {rigid.velocity.y} / {rigid.velocity.z}");
            WheelPosAndAni();
        }

            /*
            // 앞바퀴에만 steerAngle 설정
            for (int i = 0; i < 2; i++)
            {
                if (true)
                {
                    // 현재 속도에 따라 스티어링 각도를 동적으로 조절
                    float currentSpeed = rigid.velocity.magnitude;

                    wheels[i].steerAngle = rot * steeringInput;
                    //transform.Translate(new Vector3(0, 0, 0.005f) * steeringInput);
                    transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + steeringInput * rot * rigid.velocity.magnitude * 0.001f, 0);

                }
                else
                {
                    //wheels[i].steerAngle = 0;
                }*/



      
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

    private void DriftControl()
    {
        /*
        if (drift)
        {
            Debug.Log("누른 상태");
            if (shiftPut <= 0.05f)
                shiftPut += 0.005f;


            wheels[0].motorTorque = 0;
            wheels[1].motorTorque = 0;
            wheels[2].brakeTorque = 100000;
            wheels[3].brakeTorque = 100000;
            wheels[2].motorTorque = 0;
            wheels[3].motorTorque = 0;

            //transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + rigid.velocity.magnitude * steeringInput * 0.1f, 0);
            //transform.Translate(-transform.right * rigid.velocity.magnitude * steeringInput * 0.0005f);
            //transform.Translate(-transform.forward * steeringInput * 0.08f);
        }
        else
        {
            shiftPut = 0.01f;
        }
        */
    }
}
