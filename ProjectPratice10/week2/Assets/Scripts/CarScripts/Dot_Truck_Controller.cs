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


	public float maxMotorTorque;
	public float maxSteeringAngle;
	public List<Dot_Truck> truck_Infos;

	public bool isBrake;

	WheelHit hit; //�ӵ� ����

	public float minSpeed = -50;
	public float maxSpeed = 50;

	public float myAcceleration = 2f;

	bool isDraft = false;

	private void Awake()
	{
		rigid = GetComponent<Rigidbody>();
	}

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
		rigid.velocity = new Vector3(Mathf.Clamp(rigid.velocity.x, minSpeed, maxSpeed),
		Mathf.Clamp(rigid.velocity.y, minSpeed, maxSpeed),
		Mathf.Clamp(rigid.velocity.z, minSpeed, maxSpeed));

		rigid.centerOfMass = new Vector3(0, -0.15f, 0); //���� �߽� y������ ����

		float motor = maxMotorTorque * Input.GetAxis("Vertical");
		float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
		float brakeTorque = Mathf.Abs(Input.GetAxis("Jump"));




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
						Debug.Log($"GetGrundhHit��Left : {hit.forwardSlip}");
					}
					if (truck_Info.rightWheel.GetGroundHit(out hit))
					{
						Debug.Log($"GetGrundhHit��Right : {hit.forwardSlip}");
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

					//�� ����
					frictionLeft.stiffness = 1f;
					truck_Info.leftWheel.forwardFriction = frictionLeft;

					frictionRight.stiffness = 1f;
					truck_Info.leftWheel.forwardFriction = frictionRight;

					//�� ����
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
                #region ����
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
                #region �ӵ��� ����
                
                float targetSpeed = maxSpeed; // ��ǥ �ӵ�
				float acceleration = myAcceleration; // ���ӵ�

				float speedDifference = targetSpeed - rigid.velocity.magnitude * 3.6f;
				float requiredForce = rigid.mass;

				//������ ���� ũ��� �����Ƿ� �ƹ� ������ ��������
				float wheelRadius = truck_Info.leftWheel.radius * maxMotorTorque;

				float torque = requiredForce * wheelRadius * motor;

				truck_Info.leftWheel.motorTorque = torque;
				truck_Info.rightWheel.motorTorque = torque;



				// ������ �ӵ��� ��ǥ �ӵ��� ���Ͽ� ��ũ�� ����
				if (speedDifference > 0)
				{
					// ���� �ӵ��� ��ǥ �ӵ����� ������ ����
					truck_Info.leftWheel.motorTorque = torque;
					truck_Info.rightWheel.motorTorque = torque;
				}
				else if (speedDifference < 0)
				{
					// ���� �ӵ��� ��ǥ �ӵ����� ������ ����
					truck_Info.leftWheel.motorTorque = -torque;
					truck_Info.rightWheel.motorTorque = -torque;
				}
				else
				{
					// ���� �ӵ��� ��ǥ �ӵ��� �����ϸ� ��ũ�� 0���� �����Ͽ� ����
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

		// ������ ���� ���̸� ����
		RaycastHit hit;
		// ������ ����
		if (Physics.Raycast(transform.position, -transform.up, out hit, 0.2f) || isDraft)
		{
			rigid.angularDrag = 1.5f;
		} //���� ���� 
		//�� ������
		else
        {
			rigid.angularDrag = 1000;
		}

	}


}
