#if !UNITY_EDITOR
using EFT.UI;
using System.Collections;
using System.Collections.Generic;
using Comfort.Common;
using UnityEngine;
using System.Reflection;

public class AnimationManager : MonoBehaviour
{
    private bool isPlayingAnimation = false;
    private IAnimator animator;
    public DefaultEmulatorManager emulatorManager;
    public AudioSource animationManagerAudioSource;
    public static AnimationManager Instance { get; private set; }
    private Queue<(string animationName, int animationLayer)> animationQueue = new Queue<(string, int)>();
    private Dictionary<string, List<AnimationTrigger>> animationTriggers = new Dictionary<string, List<AnimationTrigger>>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void Init(IAnimator _animator, DefaultEmulatorManager defaultEmulatorManager)
    {
        animator = _animator;
        emulatorManager = defaultEmulatorManager;

        if (gameObject.GetComponent<AudioSource>() == null)
        {
            animationManagerAudioSource = gameObject.AddComponent<AudioSource>();
            animationManagerAudioSource.playOnAwake = false; 
        }
    }
    private void Update()
    {
        if (isPlayingAnimation)
        {
            animator.ResetTrigger("Look");
        }
    }
    public void PlayAnimation(string animationName, int animationLayer)
    {
        if (isPlayingAnimation)
        {
            animationQueue.Enqueue((animationName, animationLayer));
        }
        else
        {
            StartCoroutine(PlayAnimationCoroutine(animationName, animationLayer));
        }
    }


    private IEnumerator PlayAnimationCoroutine(string animationName, int animationLayer)
    {
        isPlayingAnimation = true;

        yield return new WaitForSeconds(0.5f);

        int animationHash = Animator.StringToHash(animationName);
        animator.SetTrigger(animationHash);

        float animationStartTime = Time.time;

        while (true)
        {
            float elapsedTime = Time.time - animationStartTime;

            if (animationTriggers.TryGetValue(animationName, out var triggers))
            {
                foreach (var trigger in triggers)
                {
                    if (!trigger.HasTriggered && elapsedTime >= trigger.TriggerTime)
                    {
                        trigger.Execute();
                    }
                }
            }

            if (elapsedTime >= animator.GetCurrentAnimatorStateInfo(animationLayer).length)
            {

                if (animationTriggers.ContainsKey(animationName))
                {
                    foreach (var trigger in triggers)
                    {
                        trigger.Reset();
                    }
                }
                break;
            }

            yield return null;
        }



        isPlayingAnimation = false;

        if (animationQueue.Count > 0)
        {
            var (nextAnimationName, nextAnimationLayer) = animationQueue.Dequeue();
            StartCoroutine(PlayAnimationCoroutine(nextAnimationName, nextAnimationLayer));

        }
    }


    public void StopAllAnimations()
    {
        animationQueue.Clear();
        isPlayingAnimation = false;
        animator.ResetTrigger(animator.GetCurrentAnimatorStateInfo(0).ToString());
    }

    public void AddAnimationTrigger(string animationName, AnimationTrigger trigger)
    {
        if (!animationTriggers.ContainsKey(animationName))
        {
            animationTriggers[animationName] = new List<AnimationTrigger>();
        }

        var existingTriggerIndex = animationTriggers[animationName].FindIndex(existingTrigger =>
            existingTrigger.GetType() == trigger.GetType() && existingTrigger.TriggerTime == trigger.TriggerTime);

        if (existingTriggerIndex == -1)
        {
            animationTriggers[animationName].Add(trigger);
        }
        else
        {
            animationTriggers[animationName][existingTriggerIndex] = trigger;
        }
    }

}

public abstract class AnimationTrigger
{
    public float TriggerTime { get; private set; }
    public bool HasTriggered { get; private set; }

    public AnimationTrigger(float triggerTime)
    {
        TriggerTime = triggerTime;
        HasTriggered = false;
    }

    public void Execute()
    {
        if (!HasTriggered)
        {
            TriggerAction();
            HasTriggered = true;
        }
    }

    public void Reset()
    {
        HasTriggered = false;
    }

    protected abstract void TriggerAction();
}

public class GameObjectEnableTrigger : AnimationTrigger
{
    private GameObject targetObject;
    private bool enable;

    public GameObjectEnableTrigger(float triggerTime, GameObject targetObject, bool enable) : base(triggerTime)
    {
        this.targetObject = targetObject;
        this.enable = enable;
    }

    protected override void TriggerAction()
    {
        targetObject.SetActive(enable);
    }
}

public class GameObjectDestroyTrigger : AnimationTrigger
{
    private GameObject targetObject;

    public GameObjectDestroyTrigger(float triggerTime, GameObject targetObject) : base(triggerTime)
    {
        this.targetObject = targetObject;
    }

    protected override void TriggerAction()
    {
        Object.Destroy(targetObject); 
    }
}


public class SoundTrigger : AnimationTrigger
{
    private AudioSource audioSource;
    private AudioClip audioClip;

    public SoundTrigger(float triggerTime, AudioSource audioSource, AudioClip audioClip = null) : base(triggerTime)
    {
        this.audioSource = audioSource;
        this.audioClip = audioClip;
    }

    protected override void TriggerAction()
    {
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
        }


        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
    }


}

#endif