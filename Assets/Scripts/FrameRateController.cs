using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class FrameRateController : MonoBehaviour
{
    
    int ticker = 0;
    public int freq = 180;
    public int spikeMS = 1000;
    void Update() {
        if (ticker % freq == 0) {
            Thread.Sleep(spikeMS);
        }
        ticker++;
    }


}
