using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CinemachineBossFocus : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public CinemachineCamera playerVcam;
    public CinemachineCamera bossVcam;

    [Header("Timing")]
    public float blendTime = 2f;      // match CinemachineBrain default blend duration
    public float focusDuration = 2f;  // how long to stay looking at boss AFTER blend completes

    [Header("Priorities")]
    public int bossPriority = 30;     // higher than player default
    int playerPriority;
    bool isFocusing = false;

    void Start()
    {
        if (playerVcam != null) playerPriority = playerVcam.Priority;
        // ensure boss priority starts lower than player so player is default.
        if (bossVcam != null) bossVcam.Priority = playerPriority - 1;
    }

    // Call this when boss becomes active (e.g., after boss.SetActive(true))
    public void FocusOnBossOnce()
    {
        if (!isFocusing) StartCoroutine(FocusRoutine());
    }

    IEnumerator FocusRoutine()
    {
        if (playerVcam == null || bossVcam == null) yield break;
        isFocusing = true;

        // Raise boss vcam priority → Cinemachine will start blending to it
        bossVcam.Priority = bossPriority;

        // Wait for blend to finish
        yield return new WaitForSeconds(blendTime + 0.05f);

        // Stay focused on boss for desired time
        yield return new WaitForSeconds(focusDuration);

        // Lower boss priority so player vcam returns
        bossVcam.Priority = playerPriority - 1;

        // optional: wait for blend back to finish before allowing another focus
        yield return new WaitForSeconds(blendTime + 0.05f);

        isFocusing = false;
    }
}
