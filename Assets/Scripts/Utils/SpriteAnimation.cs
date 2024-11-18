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
    public string key; // �ִϸ��̼� Ű
    public List<Sprite> sprites; // ��������Ʈ ����Ʈ
    public bool isLoop = true; // ���� ����
    public ButtonClickedEvent endCallback; // �ִϸ��̼� ���� �� �ݹ�
}

public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] private List<AnimationSpriteGroup> animations;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float moveToFrame = 20;

    private float nowFrame;
    private int nowIndex;
    private AnimationSpriteGroup nowAnimation;

    private eAnimationState currentState = eAnimationState.Idle; // ���� ����

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
        if (currentState == state) return; // ������ ���·� ��ȯ���� ����

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
