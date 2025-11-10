using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHUD hud;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] AudioSource audioSource;
    [SerializeField] List<AudioClip> audioClips;

    public Creature Creature { get; private set; }

    public bool IsPlayerUnit => isPlayerUnit;

    Vector3 originalPos;
    Color originalColor;

    void Awake()
    {
        originalPos = transform.localPosition;
        originalColor = spriteRenderer.color;
    }

    public void Setup(Creature creature)
    {
        Creature = creature;
        if (isPlayerUnit)
            spriteRenderer.sprite = creature.Base.BackSprite;
        else
            spriteRenderer.sprite = creature.Base.FrontSprite;

        hud.SetData(creature);

        // Reset visuals in case of reuse
        transform.localPosition = originalPos;
        spriteRenderer.color = originalColor;

        PlayEnterAnimation();
    }

    public void PlayEnterAnimation()
    {
        StartCoroutine(EnterAnimation());
    }

    IEnumerator EnterAnimation()
    {
        var startX = isPlayerUnit ? -8f : 8f;
        var endX = originalPos.x;

        transform.localPosition = new Vector3(startX, originalPos.y);

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, t);
            yield return null;
        }
    }

    public void PlayAttackAnimation()
    {
        StartCoroutine(AttackAnimation());
    }

    IEnumerator AttackAnimation()
    {
        var targetPos = originalPos + new Vector3(isPlayerUnit ? 1f : -1f, 0);
        yield return MoveTo(targetPos, 0.1f);
        yield return MoveTo(originalPos, 0.1f);
    }

    public void PlayHitAnimation()
    {
        audioSource.clip = audioClips[0];
        audioSource.Play();
        StartCoroutine(HitAnimation());
    }

    IEnumerator HitAnimation()
    {
        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = Color.gray;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }

        if(Creature.HP <= Creature.Base.MaxHP / 5 && isPlayerUnit)
        {
            audioSource.clip = audioClips[1];
            audioSource.Play();
        }
    }

    public void PlayFaintAnimation()
    {
        StartCoroutine(FaintAnimation());
    }

    IEnumerator FaintAnimation()
    {
        float t = 0;
        Vector3 start = originalPos;
        Vector3 end = originalPos + Vector3.down * 1.5f;

        while (t < 1)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(start, end, t);
            spriteRenderer.color = new Color(1, 1, 1, 1 - t);
            yield return null;
        }
    }

    IEnumerator MoveTo(Vector3 target, float duration)
    {
        Vector3 start = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.localPosition = Vector3.Lerp(start, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = target;
    }
}
