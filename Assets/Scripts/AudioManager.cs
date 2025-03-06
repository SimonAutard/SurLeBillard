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
    ClickMenu,
    ClickDialog,
    Music
}

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    private AudioSource[] _audioSources;
    [SerializeField] private AudioClip[] _soundList;
    private float _fastBallSoundTiming = 0.15f;
    private float _slowBallSoundTiming = 0.3f;
    private int _nextSourceId = 1;

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
        EventBus.Subscribe<EventPocketingSignal>(PlayPocketingSound);
        EventBus.Subscribe<EventMenuClickSignal>(PlayMenuClickSound);
        EventBus.Subscribe<EventDialogClickSignal>(PlayDialogClickSound);
        EventBus.Subscribe<EventNewGameRequest>(StartBackgroundMusic);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EventApplyForceToWhiteRequest>(PlayCueOnWhiteSound);
        EventBus.Unsubscribe<EventCollisionSignal>(PlayBallCollisionSound);
        EventBus.Unsubscribe<EventBounceOnBandSignal>(PlayBounceOnBandSound);
        EventBus.Unsubscribe<EventPocketingSignal>(PlayPocketingSound);
        EventBus.Unsubscribe<EventMenuClickSignal>(PlayMenuClickSound);
        EventBus.Unsubscribe<EventDialogClickSignal>(PlayDialogClickSound);
        EventBus.Unsubscribe<EventNewGameRequest>(StartBackgroundMusic);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<EventApplyForceToWhiteRequest>(PlayCueOnWhiteSound);
        EventBus.Unsubscribe<EventCollisionSignal>(PlayBallCollisionSound);
        EventBus.Unsubscribe<EventBounceOnBandSignal>(PlayBounceOnBandSound);
        EventBus.Unsubscribe<EventPocketingSignal>(PlayPocketingSound);
        EventBus.Unsubscribe<EventMenuClickSignal>(PlayMenuClickSound);
        EventBus.Unsubscribe<EventDialogClickSignal>(PlayDialogClickSound);
        EventBus.Unsubscribe<EventNewGameRequest>(StartBackgroundMusic);
    }

    private void Start()
    {
        _audioSources = GetComponents<AudioSource>();
    }

    private AudioSource NextAudioSource()
    {
        _nextSourceId++;
        if (_nextSourceId >= _audioSources.Length)
        {
            _nextSourceId = 1;
        }
        return _audioSources[_nextSourceId];
    }

    public static void PlaySound(SoundType sound, float volume = 1)
    {
        Instance.NextAudioSource().PlayOneShot(Instance._soundList[(int)sound], volume);
    }

    private void PlayCueOnWhiteSound(EventApplyForceToWhiteRequest eventRequest)
    {
        Instance.NextAudioSource().PlayOneShot(Instance._soundList[(int)SoundType.Cue], 0.4f);
    }

    private void PlayBallCollisionSound(EventCollisionSignal eventRequest)
    {
        Instance.NextAudioSource().PlayOneShot(Instance._soundList[(int)SoundType.CollisionBall], 0.4f);
        if (eventRequest._slowestBall != 0 && eventRequest._fastestBall != 0)
        {
            switch (eventRequest._fastestBallTheme)
            {
                case "Love":
                    StartCoroutine(TimedCollisionSound(SoundType.LoveShort, _fastBallSoundTiming, 0.4f));
                    break;
                case "Finances":
                    StartCoroutine(TimedCollisionSound(SoundType.FinancesShort, _fastBallSoundTiming, 1.0f));
                    break;
                case "Nature":
                    StartCoroutine(TimedCollisionSound(SoundType.NatureShort, _fastBallSoundTiming, 0.4f));
                    break;
                case "Health":
                    StartCoroutine(TimedCollisionSound(SoundType.HealthShort, _fastBallSoundTiming, 0.4f));
                    break;
                case "Spirituality":
                    StartCoroutine(TimedCollisionSound(SoundType.SpiritualityShort, _fastBallSoundTiming, 0.4f));
                    break;
                case "Friendship":
                    StartCoroutine(TimedCollisionSound(SoundType.FriendshipShort, _fastBallSoundTiming, 0.4f));
                    break;
                case "Career":
                    StartCoroutine(TimedCollisionSound(SoundType.CareerShort, _fastBallSoundTiming, 1.0f));
                    break;
                default:
                    break;
            }
            switch (eventRequest._slowestBallTheme)
            {
                case "Love":
                    StartCoroutine(TimedCollisionSound(SoundType.LoveShort, _slowBallSoundTiming, 0.4f));
                    break;
                case "Finances":
                    StartCoroutine(TimedCollisionSound(SoundType.FinancesShort, _slowBallSoundTiming, 1.0f));
                    break;
                case "Nature":
                    StartCoroutine(TimedCollisionSound(SoundType.NatureShort, _slowBallSoundTiming, 0.4f));
                    break;
                case "Health":
                    StartCoroutine(TimedCollisionSound(SoundType.HealthShort, _slowBallSoundTiming, 0.4f));
                    break;
                case "Spirituality":
                    StartCoroutine(TimedCollisionSound(SoundType.SpiritualityShort, _slowBallSoundTiming, 0.4f));
                    break;
                case "Friendship":
                    StartCoroutine(TimedCollisionSound(SoundType.FriendshipShort, _slowBallSoundTiming, 0.4f));
                    break;
                case "Career":
                    StartCoroutine(TimedCollisionSound(SoundType.CareerShort, _slowBallSoundTiming, 1.0f));
                    break;
                default:
                    break;
            }
        }
    }

    private IEnumerator TimedCollisionSound(SoundType sound, float delay, float volume)
    {
        yield return new WaitForSeconds(delay);
        Instance.NextAudioSource().PlayOneShot(Instance._soundList[(int)sound], volume);
    }

    private void PlayBounceOnBandSound(EventBounceOnBandSignal eventSignal)
    {
        Instance.NextAudioSource().PlayOneShot(Instance._soundList[(int)SoundType.CollisionEdge], 1.0f);
    }

    private void PlayPocketingSound(EventPocketingSignal eventSignal)
    {
        Instance.NextAudioSource().PlayOneShot(Instance._soundList[(int)SoundType.Pocketting], 0.5f);
    }

    private void PlayMenuClickSound(EventMenuClickSignal eventSignal)
    {
        Instance.NextAudioSource().PlayOneShot(Instance._soundList[(int)SoundType.ClickMenu], 0.4f);
    }

    private void PlayDialogClickSound(EventDialogClickSignal eventSignal)
    {
        Instance.NextAudioSource().PlayOneShot(Instance._soundList[(int)SoundType.ClickDialog], 0.18f);
    }

    private void StartBackgroundMusic(EventNewGameRequest eventRequest)
    {
        Instance._audioSources[0].clip = Instance._soundList[(int)SoundType.Music];
        Instance._audioSources[0].volume = 0.5f;
        Instance._audioSources[0].Play();
    }
}
