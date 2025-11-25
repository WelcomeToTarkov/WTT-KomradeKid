#if !UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBoyEmulator.Managers;

public class AnimationManager : MonoBehaviour
{
    private bool _isPlayingAnimation ;
    private IAnimator _animator;
    public DefaultEmulatorManager emulatorManager;
    public AudioSource animationManagerAudioSource;
    private static AnimationManager Instance { get; set; }
    private readonly Queue<(string animationName, int animationLayer)> _animationQueue = new Queue<(string, int)>();
    private readonly Dictionary<string, List<AnimationTrigger>> _animationTriggers = new Dictionary<string, List<AnimationTrigger>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void Init(IAnimator animator, DefaultEmulatorManager defaultEmulatorManager)
    {
        _animator = animator;
        emulatorManager = defaultEmulatorManager;

        if (!gameObject.GetComponent<AudioSource>())
        {
            animationManagerAudioSource = gameObject.AddComponent<AudioSource>();
            animationManagerAudioSource.playOnAwake = false; 
        }
    }
    private void Update()
    {
        if (_isPlayingAnimation)
        {
            _animator.ResetTrigger("Look");
        }
    }
    public void PlayAnimation(string animationName, int animationLayer)
    {
        if (_isPlayingAnimation)
        {
            _animationQueue.Enqueue((animationName, animationLayer));
        }
        else
        {
            StartCoroutine(PlayAnimationCoroutine(animationName, animationLayer));
        }
    }


    private IEnumerator PlayAnimationCoroutine(string animationName, int animationLayer)
    {
        _isPlayingAnimation = true;

        yield return new WaitForSeconds(0.5f);

        int animationHash = Animator.StringToHash(animationName);
        _animator.SetTrigger(animationHash);

        float animationStartTime = Time.time;

        while (true)
        {
            float elapsedTime = Time.time - animationStartTime;

            if (_animationTriggers.TryGetValue(animationName, out var triggers))
            {
                foreach (var trigger in triggers)
                {
                    if (!trigger.HasTriggered && elapsedTime >= trigger.TriggerTime)
                    {
                        trigger.Execute();
                    }
                }
            }

            if (elapsedTime >= _animator.GetCurrentAnimatorStateInfo(animationLayer).length)
            {

                if (_animationTriggers.ContainsKey(animationName))
                {
                    if (triggers != null)
                        foreach (var trigger in triggers)
                        {
                            trigger.Reset();
                        }
                }
                break;
            }

            yield return null;
        }



        _isPlayingAnimation = false;

        if (_animationQueue.Count > 0)
        {
            var (nextAnimationName, nextAnimationLayer) = _animationQueue.Dequeue();
            StartCoroutine(PlayAnimationCoroutine(nextAnimationName, nextAnimationLayer));

        }
    }

    public void StopAllAnimations()
    {
        _animationQueue.Clear();
        _isPlayingAnimation = false;
        _animator.ResetTrigger(_animator.GetCurrentAnimatorStateInfo(0).ToString());
    }

    public void AddAnimationTrigger(string animationName, AnimationTrigger trigger)
    {
        if (!_animationTriggers.ContainsKey(animationName))
        {
            _animationTriggers[animationName] = new List<AnimationTrigger>();
        }

        var existingTriggerIndex = _animationTriggers[animationName].FindIndex(existingTrigger =>
            existingTrigger.GetType() == trigger.GetType() && Mathf.Approximately(existingTrigger.TriggerTime, trigger.TriggerTime));

        if (existingTriggerIndex == -1)
        {
            _animationTriggers[animationName].Add(trigger);
        }
        else
        {
            _animationTriggers[animationName][existingTriggerIndex] = trigger;
        }
    }

}

public abstract class AnimationTrigger(float triggerTime)
{
    public float TriggerTime { get; } = triggerTime;
    public bool HasTriggered { get; private set; }

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

public class GameObjectEnableTrigger(float triggerTime, GameObject targetObject, bool enable)
    : AnimationTrigger(triggerTime)
{
    protected override void TriggerAction()
    {
        targetObject.SetActive(enable);
    }
}

public class GameObjectDestroyTrigger(float triggerTime, GameObject targetObject) : AnimationTrigger(triggerTime)
{
    protected override void TriggerAction()
    {
        Object.Destroy(targetObject); 
    }
}


public class SoundTrigger(float triggerTime, AudioSource audioSource, AudioClip audioClip = null)
    : AnimationTrigger(triggerTime)
{
    protected override void TriggerAction()
    {
        if (audioClip)
        {
            audioSource.clip = audioClip;
        }
        if (audioSource.clip)
        {
            audioSource.Play();
        }
    }


}

#endif