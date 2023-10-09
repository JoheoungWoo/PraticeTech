using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Dot_Truck : System.Object
{
	public WheelCollider leftWheel;
	public GameObject leftWheelMesh;
	public WheelCollider rightWheel;
	public GameObject rightWheelMesh;
	public bool motor;
	public bool steering;
	public bool reverseTurn;
	public bool isFront;
	public bool isBack;
}

public class Dot_Truck_Controller : MonoBehaviour
{
	public Rigidbody rigid { get; private set; }

	//파워 출력
	public float maxMotorTorque;
	public float maxSteeringAngle;
	public List<Dot_Truck> truck_Infos;

	public bool isBrake;
	WheelHit hit; //속도 측정용

	//속도 설정
	public float minSpeed;
	public float maxSpeed;

	//불 체크
	bool isDraft = false;
	bool isBooster = false;
	public bool isMiddleCheck = false;

	//부스터 
	float boosterMaxTime = 3.0f;
	float nowBoosterDelay = 0.0f;

	//골 스코어
	public int maxLap = 3;
	public int nowLap = 0;

	//임시 변수
	float tempMaxSpeed;

	//차량 등록 확인용
	public MyCar myCar;

	private void Awake()
	{
		rigid = GetComponent<Rigidbody>();
		myCar = GameObject.FindGameObjectWithTag("Car").GetComponent<MyCar>();
		Init(myCar);
	}

	private void Init(MyCar myCar)
    {
		minSpeed = -myCar.MyVehicleProperties.maxSpeed;
		maxSpeed = myCar.MyVehicleProperties.maxSpeed;
		boosterMaxTime = myCar.MyVehicleProperties.boostTime;
		rigid.mass = myCar.MyVehicleProperties.mass;
	}

	//바퀴 회전
	public void VisualizeWheel(Dot_Truck wheelPair)
	{
		Quaternion rot;
		Vector3 pos;
		wheelPair.leftWheel.GetWorldPose(out pos, out rot);
		wheelPair.leftWheelMesh.transform.position = pos;
		wheelPair.leftWheelMesh.transform.rotation = rot;
		wheelPair.rightWheel.GetWorldPose(out pos, out rot);
		wheelPair.rightWheelMesh.transform.position = pos;
		wheelPair.rightWheelMesh.transform.rotation = rot;
	}


	public void Update()
	{
		float motor = maxMotorTorque * Input.GetAxis("Vertical");
		float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
		float brakeTorque = Mathf.Abs(Input.GetAxis("Jump"));
		bool booster = Input.GetButton("Drift");



		if (booster && !isBooster)
        {
			isBooster = true;
        }

		if(isBooster)
        {
			tempMaxSpeed = maxSpeed + 20f;
			if (tempMaxSpeed > rigid.velocity.magnitude * 3.6f)
				rigid.AddForce(transform.forward * 0.5f, ForceMode.Acceleration);

			rigid.velocity = new Vector3(Mathf.Clamp(rigid.velocity.x, minSpeed, tempMaxSpeed),
			Mathf.Clamp(rigid.velocity.y, minSpeed, tempMaxSpeed),
			Mathf.Clamp(rigid.velocity.z, minSpeed, tempMaxSpeed));
		} else
        {
			rigid.velocity = new Vector3(Mathf.Clamp(rigid.velocity.x, minSpeed, maxSpeed),
			Mathf.Clamp(rigid.velocity.y, minSpeed, maxSpeed),
			Mathf.Clamp(rigid.velocity.z, minSpeed, maxSpeed));
		}

		rigid.centerOfMass = new Vector3(0, -0.15f, 0); //무게 중심 y축으로 설정

		if (nowBoosterDelay <= boosterMaxTime && isBooster)
		{
			nowBoosterDelay += Time.deltaTime;
		} else if (nowBoosterDelay > boosterMaxTime && isBooster)
        {
			isBooster = false;
			nowBoosterDelay = 0.0f;
        }

		if (brakeTorque > 0.001)
		{
			rigid.angularDrag = 1.5f;
			isDraft = true;
		}
		else
		{
			isDraft = false;
		}

		foreach (Dot_Truck truck_Info in truck_Infos)
		{
			if (isDraft)
			{
				WheelFrictionCurve frictionLeft = truck_Info.leftWheel.forwardFriction;
				WheelFrictionCurve frictionRight = truck_Info.rightWheel.forwardFriction;

				WheelFrictionCurve frictionBackLeft = truck_Info.leftWheel.sidewaysFriction;
				WheelFrictionCurve frictionBackRight = truck_Info.rightWheel.sidewaysFriction;

				if (truck_Info.isFront)
				{
					truck_Info.motor = true;
					brakeTorque = 0;

					frictionBackLeft.stiffness = 5f;
					truck_Info.leftWheel.sidewaysFriction = frictionBackLeft;

					frictionBackRight.stiffness = 5f;
					truck_Info.rightWheel.sidewaysFriction = frictionBackRight;
				}
				else if (truck_Info.isBack)
				{
					brakeTorque = maxMotorTorque;
					motor = 0;

					frictionLeft.stiffness = 0;
					truck_Info.leftWheel.forwardFriction = frictionLeft;

					frictionRight.stiffness = 0;
					truck_Info.leftWheel.forwardFriction = frictionRight;

					frictionBackLeft.stiffness = 4f;
					truck_Info.leftWheel.sidewaysFriction = frictionBackLeft;

					frictionBackRight.stiffness = 4f;
					truck_Info.rightWheel.sidewaysFriction = frictionBackRight;

					truck_Info.motor = false;


					if (truck_Info.leftWheel.GetGroundHit(out hit))
					{
						Debug.Log($"GetGrundhHit값Left : {hit.forwardSlip}");
					}
					if (truck_Info.rightWheel.GetGroundHit(out hit))
					{
						Debug.Log($"GetGrundhHit값Right : {hit.forwardSlip}");
					}
				}
			}
			else
			{
				WheelFrictionCurve frictionLeft = truck_Info.leftWheel.forwardFriction;
				WheelFrictionCurve frictionRight = truck_Info.rightWheel.forwardFriction;

				WheelFrictionCurve frictionBackLeft = truck_Info.leftWheel.sidewaysFriction;
				WheelFrictionCurve frictionBackRight = truck_Info.rightWheel.sidewaysFriction;

				if (truck_Info.isFront)
				{
					truck_Info.motor = false;

					frictionBackLeft.stiffness = 1f;
					truck_Info.leftWheel.sidewaysFriction = frictionBackLeft;

					frictionBackRight.stiffness = 1f;
					truck_Info.rightWheel.sidewaysFriction = frictionBackRight;
				}
				else if (truck_Info.isBack)
				{
					truck_Info.motor = true;
					brakeTorque = 0;

					//앞 바퀴
					frictionLeft.stiffness = 1f;
					truck_Info.leftWheel.forwardFriction = frictionLeft;

					frictionRight.stiffness = 1f;
					truck_Info.leftWheel.forwardFriction = frictionRight;

					//뒷 바퀴
					frictionBackLeft.stiffness = 1.5f;
					truck_Info.leftWheel.sidewaysFriction = frictionBackLeft;

					frictionBackRight.stiffness = 1.5f;
					truck_Info.rightWheel.sidewaysFriction = frictionBackRight;
				}
			}
			if (truck_Info.steering == true)
			{
				var value = ((truck_Info.reverseTurn) ? -1 : 1) * steering;

				if (isDraft)
					value *= 1.3f;

				truck_Info.leftWheel.steerAngle = truck_Info.rightWheel.steerAngle = value;
			}

			if (truck_Info.motor == true)
			{
                #region 접기
                /*
				float tempRpmLeft =  truck_Info.leftWheel.rpm;
				float tempRpmRight = truck_Info.rightWheel.rpm;

				Debug.Log(tempRpmLeft + "plz!@#$%" + tempRpmRight);

				truck_Info.leftWheel.motorTorque = motor;
				truck_Info.rightWheel.motorTorque = motor;

           
				//if (Mathf.Abs(tempRpmLeft) >= maxRpm || Mathf.Abs(tempRpmRight) >= maxRpm)
                //{
				//	truck_Info.leftWheel.motorTorque = 0;
				//	truck_Info.rightWheel.motorTorque = 0;
				//}
				//else
                //{
				//
				//}
				*/
                #endregion
                #region 속도락 버전
                
                float targetSpeed = maxSpeed; // 목표 속도

				float speedDifference = targetSpeed - rigid.velocity.magnitude * 3.6f;
				float requiredForce = rigid.mass;

				//어차피 바퀴 크기는 같으므로 아무 바퀴나 가져오기
				float wheelRadius = truck_Info.leftWheel.radius * maxMotorTorque;

				float torque = requiredForce * wheelRadius * maxMotorTorque;

				truck_Info.leftWheel.motorTorque = torque;
				truck_Info.rightWheel.motorTorque = torque;



				// 차량의 속도와 목표 속도를 비교하여 토크를 적용
				if (speedDifference > 0)
				{
					// 현재 속도가 목표 속도보다 낮으면 가속
					truck_Info.leftWheel.motorTorque = torque;
					truck_Info.rightWheel.motorTorque = torque;
				}
				else if (speedDifference < 0)
				{
					// 현재 속도가 목표 속도보다 높으면 감속
					truck_Info.leftWheel.motorTorque = -torque;
					truck_Info.rightWheel.motorTorque = -torque;
				}
				else
				{
					// 현재 속도가 목표 속도와 동일하면 토크를 0으로 설정하여 유지
					truck_Info.leftWheel.motorTorque = 0;
					truck_Info.rightWheel.motorTorque = 0;
				}
                #endregion
            }
        }
	}

    private void FixedUpdate()
    {
		var rotation = gameObject.transform.eulerAngles;
		rotation.z = 0;
		transform.rotation = Quaternion.Euler(rotation);

		RaycastHit hit;
		// 평상시의 상태
		if (Physics.Raycast(transform.position, -transform.up, out hit, 0.2f) || isDraft)
		{
			rigid.angularDrag = 1.5f;
		} //버그 방지 
		//붕 떴을때
		else
        {
			rigid.angularDrag = 1000;
		}

	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("StartLine"))
        {
			if(isMiddleCheck)
            {
				if(nowLap < maxLap)
                {
					nowLap += 1;
					isMiddleCheck = false;
				}
			}

        }
		if (other.CompareTag("MiddleLine"))
		{
			isMiddleCheck = true;
		}
	}

}
