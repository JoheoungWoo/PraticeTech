using UnityEngine;
using TMPro;
using System.Diagnostics;
using TimeSpan = System.TimeSpan;
using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour
{
    public TMP_Text speedInMetersPerHourTMP;
    public TMP_Text timeTMP;
    public TMP_Text lapTMP;
    public TMP_Text rankTMP;

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
        SpeedUI();
        LapUI();
        RankUI();
        TimeUI();
    }

    private void LapUI()
    {
        lapTMP.text = $"Lap {myCar.nowLap} / {myCar.maxLap}";
    }

    private void RankUI()
    {
        rankTMP.text = "1";
    }

    private void TimeUI()
    {
        //시간 연산
        timeSpan = TimeSpan.FromMilliseconds(stopWatch.ElapsedMilliseconds);
        timeTMP.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }



    private void SpeedUI()
    {
        #region 제로백 테스트
        /*
        if (Mathf.Round(myCar.rigid.velocity.magnitude * 3.6f) >= myCar.maxSpeed && !isZero100)
        {
            Debug.Log($"걸린 시간 : {timeSpan} mass: {myCar.rigid.mass} speed: {myCar.maxSpeed}");
            isZero100 = true;
        }*/
        #endregion

        //TMP에 적용
        speedInMetersPerHourTMP.text = $"{Mathf.Round(myCar.rigid.velocity.magnitude * 3.6f)}km/h";
        
    }

}
