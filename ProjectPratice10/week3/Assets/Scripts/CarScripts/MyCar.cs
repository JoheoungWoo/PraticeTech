using UnityEngine;

public enum driveType
{
    SpeedType,
    BalanceType,
    WeightType,
}

public class VehicleProperties
{
    public float mass; //���ӵ� -> mass�� ����
    public float maxSpeed; //�ְ�ӵ� -> float�� ����
    public float boostTime; //�ν�Ʈ �ð�
    public driveType drive;

    public VehicleProperties(float mass, float maxSpeed,float boostTime,driveType drive)
    {
        this.mass = mass;
        this.maxSpeed = maxSpeed;
        this.boostTime = boostTime;
        this.drive = drive;
    }

}
public class MyCar : MonoBehaviour
{
    public driveType drive;
    public float mass;
    public float maxSpeed;
    public float boostTime;

    private VehicleProperties myVehicleProperties;
    public VehicleProperties MyVehicleProperties => myVehicleProperties;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        myVehicleProperties = new VehicleProperties(mass,maxSpeed,boostTime, drive);
    }
}
