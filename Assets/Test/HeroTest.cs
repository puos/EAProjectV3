using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[EASceneInfo(typeof(HeroTest))]
#endif
public class HeroTest : EASceneLogic
{
    private CHero hero = null;
    private IngameUI inGameUi = null;
    private CameraFollow cameraFollow = null;

    protected override void OnClose()
    {
        
    }

    protected override void OnInit()
    {
        hero = CHero.MainPlayerCreate();
        inGameUi = m_uiMgr.OpenPage<IngameUI>(TankUIPage.ingameUi);

        inGameUi.SetMoveEndEvent(() =>
        {
            if (hero != null) hero.Stop();
        });

        inGameUi.SetAttackEndEvent(() =>
        {
            if (hero != null) hero.StartFire();
        });

        {
            GameObject obj = GetData("CameraFollow");
            if (obj != null) cameraFollow = obj.GetComponent<CameraFollow>();
        }

        if (cameraFollow != null) cameraFollow.Initialize(hero.transform);
    }

    private void HeroMove()
    {
        Vector3 direction = inGameUi.GetMoveDirection();
        direction.z = direction.y;
        direction.y = 0;
        direction.Normalize();

        Vector3 pos = hero.GetPos() + direction * 3.0f;

        bool changed = Vector3.Dot(hero.rb.velocity, direction) < 0 ? true : false;

        if (direction.magnitude > 0) Debug.DrawLine(hero.GetPos(), pos, Color.red);

        if (changed == true) hero.Stop();
        if (direction.magnitude > 0) hero.MoveTo(pos,()=> 
        {
            Debug.Log("Move Complete");
        });
    }

    private void HeroAttack()
    {
        Vector3 direction = inGameUi.GetAttackDirection();
        direction.z = direction.y;
        direction.y = 0;
        direction.Normalize();

        if (direction.magnitude > 0) hero.SetRotationMuzzle(Quaternion.LookRotation(direction, Vector3.up));
    }

    protected override IEnumerator OnPostInit()
    {
        yield return null;
    }

    protected override void OnUpdate()
    {
        HeroMove();
        HeroAttack();
    }
}
