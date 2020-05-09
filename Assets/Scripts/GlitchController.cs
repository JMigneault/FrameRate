using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.Rendering.PostProcessing;

public class GlitchController : MonoBehaviour
{

    public int freezeWidthFr;
    public int freezeMeanFr;

    public int preFreezeIntervalFr;

    public int freezeWidthMs;
    public int freezeMeanMs;

    public int nMiniSpikes;

    public int miniGlitchLength;

    public PostProcessVolume pp;

    private int frame = 0;

    private int[] spikes;
    private int nextFreeze;

    public float maxGrain = 0.5f;
    public float maxVignette = 0.5f;
    public float maxDistortion = 20;

    private bool doingFreeze = false;

    public TextController textController;
    

    void Start() {
        Time.maximumDeltaTime = 10f;
        Application.targetFrameRate = 16;
        freezeWidthFr = Mathf.RoundToInt(freezeWidthFr * (Application.targetFrameRate / 60f));
        freezeMeanFr = Mathf.RoundToInt(freezeMeanFr * (Application.targetFrameRate / 60f));
        preFreezeIntervalFr = Mathf.RoundToInt(preFreezeIntervalFr * (Application.targetFrameRate / 60f));

        spikes = new int[nMiniSpikes];
        GenerateSequence();

        Bloom bloom;
        pp.profile.TryGetSettings(out bloom);
        bloom.active = false;
        LensDistortion lensDistortion;
        pp.profile.TryGetSettings(out lensDistortion);
        lensDistortion.active = false;

    }

    void GenerateSequence() {
        nextFreeze = Random.Range(freezeMeanFr - freezeWidthFr / 2, freezeMeanFr + freezeWidthFr / 2) - preFreezeIntervalFr;
        for (int i = 0; i < spikes.Length; i++) {
            spikes[i] = Random.Range(nextFreeze / 4, nextFreeze);
        }
    }

    void Update() {
        if (doingFreeze) {return; }
        AdjustGlitchEffects();
        if (frame % nextFreeze == 0) {
            frame = 1;
            GenerateSequence();
            int lengthMs = Random.Range(freezeMeanMs - freezeWidthMs / 2, freezeMeanMs + freezeWidthMs / 2);
            StartCoroutine(DoGlitch(lengthMs));
            return;
        }
        frame++;
        for (int i = 0; i < spikes.Length; i++) {
            if (spikes[i] % frame == 0) {
                StartCoroutine(DoMiniGlitch());
                return;
            }
        }
    }

    public IEnumerator End() {
        textController.Switch();
        doingFreeze = true;
        EnableGlitchEffects();
        int framesWaited = 0;
        for (; framesWaited < preFreezeIntervalFr; framesWaited++) {
            yield return 0;
        }
        Debug.Log("FREEZING");
        Thread.Sleep(5000);
        Application.Quit();
    }

    IEnumerator DoMiniGlitch() {
        EnableMiniGlitchEffects();
        yield return preFreezeIntervalFr;
        Thread.Sleep(miniGlitchLength);
        DisableMiniGlitchEffects();
    }

    IEnumerator DoGlitch(int ms) {
        textController.Switch();
        doingFreeze = true;
        Debug.LogFormat("Sleeping for {0} ms", ms);
        EnableGlitchEffects();
        int framesWaited = 0;
        for (; framesWaited < preFreezeIntervalFr; framesWaited++) {
            yield return 0;
        }
        Debug.Log("FREEZING");
        Thread.Sleep(ms);
        DisableGlitchEffect();
        doingFreeze = false;
        textController.Switch();
    }

    void AdjustGlitchEffects() {
        float scale = (float) frame / (float) nextFreeze;
        Grain grain;
        pp.profile.TryGetSettings(out grain);
        grain.intensity.value = scale * maxGrain;
        Vignette vignette;
        pp.profile.TryGetSettings(out vignette);
        vignette.intensity.value = scale * maxVignette;
        LensDistortion lensDistortion;
        pp.profile.TryGetSettings(out lensDistortion);
        lensDistortion.intensity.value = scale * maxDistortion;
    }

    void EnableMiniGlitchEffects() {
    }

    void DisableMiniGlitchEffects() {
    }

    void EnableGlitchEffects() {
        Bloom bloom;
        pp.profile.TryGetSettings(out bloom);
        bloom.active = true;
        LensDistortion lensDistortion;
        pp.profile.TryGetSettings(out lensDistortion);
        lensDistortion.active = true;
    }

    void DisableGlitchEffect() {
        Bloom bloom;
        pp.profile.TryGetSettings(out bloom);
        bloom.active = false;
        LensDistortion lensDistortion;
        pp.profile.TryGetSettings(out lensDistortion);
        lensDistortion.active = false;
    }

    

}
