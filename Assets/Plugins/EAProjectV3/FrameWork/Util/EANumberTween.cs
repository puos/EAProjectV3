using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Use real number increasing dotweener
/// http://dotween.demigiant.com/
/// </summary>
public class EANumberTween
{
    private const float DEFAULT_SPEED = 5;
    private const float DEFAULT_MAXDURTIME = 1;

    private float number;

    public struct Event
    {
        public float number;
        public object[] parms;
    }

    public delegate void Callback(Event tweenEvent);
    public Callback onUpdate;
    public Callback onComplete;

    private Tweener tweener;
    Ease easeType = Ease.Linear;

    // Specify initial value
    public EANumberTween(float initNum,Ease easeType = Ease.Linear)
    {
        number = initNum;
        this.easeType = easeType;
    }

    // Start increasing animation. Recall is possible, at which point it will continue with the existing value.
    public void StartTween(float numTo,params object[] userData)
    {
        StartTween(numTo, DEFAULT_SPEED, DEFAULT_MAXDURTIME, userData);
    }
    
    /// <summary>
    /// Start increasing animation. Recall is possible, in which case it will continue with the value that was increasing.
    /// </summary>
    /// <param name="numTo">Final value</param>
    /// <param name="speed">Increments per second</param>
    /// <param name="maxDurTime">Incremental maximum time</param>
    /// <param name="userData">User data to be passed to the callback</param>
    public void StartTween(float numTo, float speed, float maxDurTime, params object[] userData)
    {
        if (tweener != null) tweener.Kill();

        float unitTime = (speed == 0) ? 0.001f : (1f / speed);
        float dTime = Mathf.Min(maxDurTime, Mathf.Abs(numTo - number) * unitTime);

        tweener = DOTween.To(() => this.number, x => this.number = x, numTo, dTime).SetEase(easeType).SetTarget(this);

        tweener.OnUpdate(() =>
        {
            OnTweenUpdate(userData);
        });

        tweener.OnComplete(()=> 
        {
            OnTweenUpdate(userData);
            OnTweenComplete(userData);
        });
    }

    private void OnTweenUpdate(params object[] userData)
    {
        if (onUpdate == null) return;

        Event e = new Event()
        {
            number = number,
            parms = userData,
        };

        try 
        {
            onUpdate(e);
        }
        catch(MissingReferenceException)
        {
            tweener.Kill();
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }

    private void OnTweenComplete(params object[] userData)
    {
        if (onComplete == null) return;

        Event e = new Event()
        {
            number = number,
            parms  = userData,
        };

        try 
        {
            onComplete(e);
        }
        catch(MissingReferenceException) 
        {
            
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }
    public static EANumberTween Start(float numFrom, float numTo , params object[] userData)
    {
        return Start(numFrom, numTo, DEFAULT_SPEED, DEFAULT_MAXDURTIME, userData);
    }

    public static EANumberTween Start(float numFrom, float numTo, float speed , float maxTime , params object[] userData)
    {
        EANumberTween numTween = new EANumberTween(numFrom);
        numTween.StartTween(numTo, speed, maxTime, userData);
        return numTween;
    }

    public static EANumberTween Start(float numFrom,float numTo,float speed,float maxTime,Ease easeType,params object[] userData)
    {
        EANumberTween numTween = new EANumberTween(numFrom, easeType);
        numTween.StartTween(numTo, speed, maxTime, userData);
        return numTween;
    }
    public void Pause() 
    {
        if (tweener != null) DOTween.Pause(tweener);
    }

}
