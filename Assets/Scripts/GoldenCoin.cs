using System.Collections;
using DG.Tweening;
using PickMaster.DI.Signals;
using UnityEngine;
using Zenject;

public class GoldenCoin : MonoBehaviour
{
    private Sequence anim;
    private GameObject target;
    public float burstRange;
    public GameObject collectFX;
    public int value;

    private SignalBus signalBus;
    
    [Inject]
    private void Init(SignalBus signalBus)
    {
        this.signalBus = signalBus;
    }
    
    private void Start()
    {
        target = GameObject.Find("GoldNuggetTarget");
    }

    public void FlyWithDelay()
    {
        var randTime = Random.Range(0.7f, 1f);
        StartCoroutine(FlyCoroutine(randTime));
    }

    private IEnumerator FlyCoroutine(float randTime)
    {
        yield return new WaitForSeconds(randTime);
        FlyToTarget();
    }

    public void FlyToTarget()
    {
        anim = DOTween.Sequence()
            .Append(transform.DOLocalMoveY(Random.Range(0.3f, 1.1f), Random.Range(0.2f, 0.5f)))
            .Append(transform.DOMove(target.gameObject.transform.position, Random.Range(0.3f, 0.7f))
                .SetEase(Ease.InQuad)
                .OnComplete(OnReachTarget));
        anim.Play();
    }
    
    public void OnReachTarget()
    {
        signalBus.Fire(new GoldCollectedSignal(value));
        var newFX = Instantiate(collectFX, transform.position, Quaternion.identity);
        Destroy(newFX, 1f);
        Destroy(gameObject);
    }
}
