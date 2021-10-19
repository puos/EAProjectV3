using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EAFPSCounter : Singleton<EAFPSCounter>
{
    public bool show = false;

    private const int targetFPS =

#if (UNITY_ANDROID || UNITY_IOS)
        60;
#else
        75;
#endif

    public float updateInterval = 0.5f;

    private float accum = 0; // FPS accumulated over the interval
    private float frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval
    private float curFps = 0.0f;
    private string sFPS = "";
  
    private Color color = Color.white; // The color of the GUI, depending of the FPS ( R < 10, Y < 30 , G >= 30 )
    private GUIStyle style; // The style the text will be displayed at, base en defaultSkin.label.
    public Rect startRect = new Rect(10, 10, 130, 80); // The rect the window is initially displayed at.
    public bool updateColor = true; // Do you want the color to change if the FPS gets low

    public override GameObject GetSingletonParent()
    {
        return EAMainFrame.instance.gameObject;
    }

    protected override void Initialize()
    {
        EAMainFrame.onLazyUpdate.Remove(OnLazyUpdate);
        EAMainFrame.onLazyUpdate.Add(OnLazyUpdate);

        EAMainFrame.onUpdate.Remove(OnUpdate);
        EAMainFrame.onUpdate.Add(OnUpdate);

        timeleft = updateInterval;
    }

    protected override void Close()
    {
        EAMainFrame.onLazyUpdate.Remove(OnLazyUpdate);
        EAMainFrame.onUpdate.Remove(OnUpdate);
    }

    private void OnLazyUpdate(EAMainFrame.LazyUpdateType type)
    {
        if ((type & EAMainFrame.LazyUpdateType.Every1s) == 0) return;

        float msec = (1.0f / curFps) * 1000.0f;
        sFPS = curFps.ToString("F2") + " FPS " + System.Environment.NewLine + msec.ToString("F2") + " ms";
    }

    private void OnUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            show = !show;
        }

        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        // Interval ended - update GUI text and start new interval
        if(timeleft <= 0.0)
        {
            // display two fractional digits (f2 format)
            curFps = accum / frames;

            timeleft = updateInterval;
            accum = 0f;
            frames = 0f;
        }
    }

    private void OnGUI()
    {
        if(show)
        {
            // Copy the default label skin, change the color and the alignment
            if(style == null)
            {
                style = new GUIStyle(GUI.skin.label);
                style.normal.textColor = Color.white;
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = 20;
            }

            if (curFps < targetFPS)
                color = Color.blue;
            else if (curFps < 10)
                color = Color.red;
            else
                color = Color.green;

            float xFactor = Screen.width / EAMainFrame.screenX;
            float yFactor = Screen.height / EAMainFrame.screenY;
            GUIUtility.ScaleAroundPivot(new Vector2(xFactor, yFactor), Vector2.zero);
            
            if (updateColor) GUI.color = color;
            startRect = GUI.Window(0, startRect, DoMyWindow, "");
        }
    }

    private void DoMyWindow(int windowID)
    {
        GUI.Label(new Rect(0, 0, startRect.width, startRect.height), sFPS, style);
    }
}
