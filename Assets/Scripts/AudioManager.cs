using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Rendering;

public enum SoundType
{
    Love,
    LoveShort,
    Finances,
    FinancesShort,
    Nature,
    NatureShort,
    Health,
    HealthShort,
    Spirituality,
    SpiritualityShort,
    Friendship,
    FriendshipShort,
    Career,
    CareerShort,
    CollisionBall,
    CollisionEdge,
    Cue,
    Pocketting,
    Click,
    Music
}

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _soundList;
    private float _fastBallSoundTiming = 0.25f;
    private float _slowBallSoundTiming = 0.4f;

    public static AudioManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        EventBus.Subscribe<EventApplyForceToWhiteRequest>(PlayCueOnWhiteSound);
        EventBus.Subscribe<EventCollisionSignal>(PlayBallCollisionSound);
        EventBus.Subscribe<EventBounceOnBandSignal>(PlayBounceOnBandSound);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EventApplyForceToWhiteRequest>(PlayCueOnWhiteSound);
        EventBus.Unsubscribe<EventCollisionSignal>(PlayBallCollisionSound);
        EventBus.Unsubscribe<EventBounceOnBandSignal>(PlayBounceOnBandSound);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<EventApplyForceToWhiteRequest>(PlayCueOnWhiteSound);
        EventBus.Unsubscribe<EventCollisionSignal>(PlayBallCollisionSound);
        EventBus.Unsubscribe<EventBounceOnBandSignal>(PlayBounceOnBandSound);
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType sound, float volume = 1)
    {
        Instance._audioSource.PlayOneShot(Instance._soundList[(int)sound], volume);
    }

    private void PlayCueOnWhiteSound(EventApplyForceToWhiteRequest eventRequest)
    {
        Instance._audioSource.PlayOneShot(Instance._soundList[(int)SoundType.Cue], 1.0f);
    }

    private void PlayBallCollisionSound(EventCollisionSignal eventRequest)
    {
        if (eventRequest._slowestBallTheme != "")
        {
            Instance._audioSource.PlayOneShot(Instance._soundList[(int)SoundType.CollisionBall], 1.0f);
            switch (eventRequest._fastestBallTheme)
            {
                case "Love":
                    StartCoroutine(TimedCollisionSound(SoundType.LoveShort, _fastBallSoundTiming));
                    break;
                case "Finances":
                    StartCoroutine(TimedCollisionSound(SoundType.FinancesShort, _fastBallSoundTiming));
                    break;
                case "Nature":
                    StartCoroutine(TimedCollisionSound(SoundType.NatureShort, _fastBallSoundTiming));
                    break;
                case "Health":
                    StartCoroutine(TimedCollisionSound(SoundType.HealthShort, _fastBallSoundTiming));
                    break;
                case "Spirituality":
                    StartCoroutine(TimedCollisionSound(SoundType.SpiritualityShort, _fastBallSoundTiming));
                    break;
                case "Friendship":
                    StartCoroutine(TimedCollisionSound(SoundType.FriendshipShort, _fastBallSoundTiming));
                    break;
                case "Career":
                    StartCoroutine(TimedCollisionSound(SoundType.CareerShort, _fastBallSoundTiming));
                    break;
                default:
                    break;
            }
            switch (eventRequest._slowestBallTheme)
            {
                case "Love":
                    StartCoroutine(TimedCollisionSound(SoundType.LoveShort, _slowBallSoundTiming));
                    break;
                case "Finances":
                    StartCoroutine(TimedCollisionSound(SoundType.FinancesShort, _slowBallSoundTiming));
                    break;
                case "Nature":
                    StartCoroutine(TimedCollisionSound(SoundType.NatureShort, _slowBallSoundTiming));
                    break;
                case "Health":
                    StartCoroutine(TimedCollisionSound(SoundType.HealthShort, _slowBallSoundTiming));
                    break;
                case "Spirituality":
                    StartCoroutine(TimedCollisionSound(SoundType.SpiritualityShort, _slowBallSoundTiming));
                    break;
                case "Friendship":
                    StartCoroutine(TimedCollisionSound(SoundType.FriendshipShort, _slowBallSoundTiming));
                    break;
                case "Career":
                    StartCoroutine(TimedCollisionSound(SoundType.CareerShort, _slowBallSoundTiming));
                    break;
                default:
                    break;
            }
        }
    }

    private IEnumerator TimedCollisionSound(SoundType sound, float delay)
    {
        yield return new WaitForSeconds(delay);
        Instance._audioSource.PlayOneShot(Instance._soundList[(int)sound], 1.0f);
    }

    private void PlayBounceOnBandSound(EventBounceOnBandSignal eventSignal)
    {
        Instance._audioSource.PlayOneShot(Instance._soundList[(int)SoundType.CollisionEdge], 1.0f);
    }
}
