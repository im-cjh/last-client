using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.Button;

public enum eAnimationState
{
    Idle,
    Walk
}

[Serializable]
public class AnimationSpriteGroup
{
    public string key; // 애니메이션 키
    public List<Sprite> sprites; // 스프라이트 리스트
    public bool isLoop = true; // 루프 여부
    public ButtonClickedEvent endCallback; // 애니메이션 끝날 때 콜백
}

public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] private List<AnimationSpriteGroup> animations;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float moveToFrame = 20;

    private float nowFrame;
    private int nowIndex;
    private AnimationSpriteGroup nowAnimation;

    private eAnimationState currentState = eAnimationState.Idle; // 현재 상태

    private void Update()
    {
        if (nowAnimation == null || nowAnimation.sprites.Count == 0) return;

        nowFrame++;
        if (nowFrame > moveToFrame)
        {
            nowFrame = 0;
            nowIndex = (nowIndex + 1) % nowAnimation.sprites.Count;

            if (nowIndex == 0 && !nowAnimation.isLoop)
            {
                nowAnimation.endCallback?.Invoke();
                nowAnimation = null;
            }
            else
            {
                spriteRenderer.sprite = nowAnimation.sprites[nowIndex];
            }
        }
    }

    public void SetState(eAnimationState state)
    {
        if (currentState == state) return; // 동일한 상태로 전환하지 않음

        currentState = state;

        switch (state)
        {
            case eAnimationState.Idle:
                ChangeAnimation("Idle");
                break;
            case eAnimationState.Walk:
                ChangeAnimation("Walk");
                break;
        }
    }

    public void ChangeAnimation(string key)
    {
        nowAnimation = animations.Find(obj => obj.key == key);
        if (nowAnimation != null)
        {
            nowFrame = 0;
            nowIndex = 0;
            spriteRenderer.sprite = nowAnimation.sprites[nowIndex];
        }
        else
        {
            Debug.LogWarning($"Animation with key '{key}' not found.");
        }
    }
}
