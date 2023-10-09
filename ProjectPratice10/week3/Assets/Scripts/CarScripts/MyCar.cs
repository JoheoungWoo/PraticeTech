using UnityEngine;

public enum driveType
{
    SpeedType,
    BalanceType,
    WeightType,
}

public class VehicleProperties
{
    public float mass; //가속도 -> mass로 설정
    public float maxSpeed; //최고속도 -> float로 설정
    public float boostTime; //부스트 시간
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
