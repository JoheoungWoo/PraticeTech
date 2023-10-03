using UnityEngine;
using TMPro;
using System.Diagnostics;
using TimeSpan = System.TimeSpan;
using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour
{
    public TMP_Text speedInMetersPerHourTMP;
    public TMP_Text timeTMP;

    public Dot_Truck_Controller myCar;

    Stopwatch stopWatch = new Stopwatch();
    TimeSpan timeSpan;

    bool isZero100 = false;

    private void Awake()
    {
        stopWatch.Start();
    }

    private void Update()
    {
        //�ð� ����
        timeSpan = TimeSpan.FromMilliseconds(stopWatch.ElapsedMilliseconds);


        if (Mathf.Round(myCar.rigid.velocity.magnitude * 3.6f) >= 70 && !isZero100)
        {
            Debug.Log($"�ɸ� �ð� : {timeSpan} mass: {myCar.rigid.mass} power: {myCar.myAcceleration}");
            isZero100 = true;
        }

        //TMP�� ����
        speedInMetersPerHourTMP.text = $"{Mathf.Round(myCar.rigid.velocity.magnitude * 3.6f)}km/h";
        timeTMP.text = string.Format("{0:D2}:{1:D2}:{2:D2}",timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }


}
