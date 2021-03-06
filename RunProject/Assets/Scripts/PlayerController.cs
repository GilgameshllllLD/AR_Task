﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;                                                  

public class PlayerController : MonoBehaviour
{

    public CharacterController playController;
    public Animator playAnimtor;
    public Vector3 MoveIncrements;
    [SerializeField]
    float transverseSpeed = 5.0f;                                   
    public float moveSpeed = 6.0f;                                  
    public float jumpPower;                                         
    [HideInInspector]
    public GameObject nowRoad;                                      
    bool isTurnleftEnd = true;                                      
    bool isTurnRightEnd = true;                                     
    bool isJumpState;                                               
    RuntimeAnimatorController nowController;                        
    AnimationClip[] cilps;
    void Start()
    {
        jumpPower = 3.0f;
        playController = GetComponent<CharacterController>();
        playAnimtor = GetComponent<Animator>();
        nowController = playAnimtor.runtimeAnimatorController;
        cilps = nowController.animationClips;
        for (int i = 0; i < cilps.Length; i++)
        {
            if (cilps[i].events.Length <= 0)
            {
                switch (cilps[i].name)
                {
                    case "JUMP00":
                        AnimationEvent endEvent = new AnimationEvent();
                        endEvent.functionName = "JumpEnd";
                        endEvent.time = cilps[i].length - (20.0f / 56.0f) * 1.83f;
                        cilps[i].AddEvent(endEvent);
                        AnimationEvent centerEvent = new AnimationEvent();
                        centerEvent.functionName = "JumpCenter";
                        centerEvent.time = cilps[i].length * 0.3f;
                        cilps[i].AddEvent(centerEvent);
                        break;
                    case "SLIDE00":
                        AnimationEvent slideEvent = new AnimationEvent();
                        slideEvent.functionName = "SlideEnd";
                        slideEvent.time = cilps[i].length - (15.0f / 42.0f) * 1.33f;
                        cilps[i].AddEvent(slideEvent);
                        break;
                }
            }
        }


    }
    void Update()
    {
        //moveSpeed += Time.deltaTime * 0.3f;
        moveSpeed = 0;
        float moveDir = Input.GetAxis("Horizontal");
        MoveIncrements = transform.forward * moveSpeed * Time.deltaTime;
        MoveIncrements += transform.right * transverseSpeed * Time.deltaTime * moveDir;
        if (isJumpState)                        //如果现在正在进行跳跃
        {
            MoveIncrements.y += jumpPower * Time.deltaTime;
        }
        else
        {
            MoveIncrements.y += playController.isGrounded ? 0f : -5.0f * Time.deltaTime * 1f;           //更新重力
        }

        playController.Move(MoveIncrements);
        playAnimtor.SetFloat("MoveSpeed", playController.velocity.magnitude);                            //动画状态更新



        if (Input.GetKeyDown(KeyCode.J) && isTurnleftEnd)
        {
            isTurnleftEnd = false;                                                                              //更新转向状态
            transform.Rotate(Vector3.up, -90);
            Quaternion tmpQuaternion = transform.rotation;                                                      //计算转向后的四元数并保存
            transform.Rotate(Vector3.up, 90);                                                                   //角度回滚
            Tween tween = transform.DORotateQuaternion(tmpQuaternion, 0.3f);                                    //使用DoTween插件进行转向的平滑运动
            tween.OnComplete(() => isTurnleftEnd = true);                                                       //动画结束后转向状态更新
        }
        if (Input.GetKeyDown(KeyCode.L) && isTurnRightEnd)
        {
            isTurnRightEnd = false;
            transform.Rotate(Vector3.up, 90);
            Quaternion tmpQuaternion = transform.rotation;
            transform.Rotate(Vector3.up, -90);
            Tween tween = transform.DORotateQuaternion(tmpQuaternion, 0.3f);
            tween.OnComplete(() => isTurnRightEnd = true);
        }
        if (Input.GetKeyDown(KeyCode.Space) && playController.isGrounded)
        {
            isJumpState = true;                     //更新跳跃状态
            playAnimtor.SetBool("IsJump", true);    //播放跳跃动画
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            playAnimtor.SetBool("IsSlide", true);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject != nowRoad)                                             //去重复避免删除错误
        {
            nowRoad = hit.gameObject;
            Destroy(hit.gameObject, 3.0f);
            GameMode.instance.BuidRoad();                                        //生成道路
            GameMode.instance.CloseRoadAnimator();
        }
    }

    public void JumpEnd()
    {
        playAnimtor.SetBool("IsJump", false);
    }

    public void JumpCenter()
    {
        isJumpState = false;
    }

    public void SlideEnd()
    {
        playAnimtor.SetBool("IsSlide", false);
    }
}
