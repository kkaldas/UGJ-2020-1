﻿using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class Card : SkillsMediatorUser
{
    #region Attributes

    /// explaining deathCount
    /// the size is the number of attacks to consider in counting: Ex: let's consider how many died in the last '10' attacks
    /// in this case 10 would be the size of the deathCount array.
    /// it starts as 1,1,1... because it will be used to see if the game tied. They will consider the game tied if in the
    /// last 10 attacks, any card have died. 1,1,1... suggests at least 10 cards have died in the last 10 attacks
    /// but unfortunately, each time the player ask an attack, it count's 2 or 3 times here because the way
    /// skills ware implemented
    private static int[] deathCount = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
    private static int deathCountIndex = 0;

    [SerializeField]
    public ClassInfo classInfo = null;

    public CardDragAndDrop cardDragAndDrop = null;

    [SerializeField]
    private int attackPower = 99;
    [SerializeField]
    private int vitality = 99;
    private int vitalityLimit;
    private int initialVitality;

    [SerializeField]
    private Text vitalityText = null;

    [SerializeField]
    private Text attackPowerText = null;

    [SerializeField]
    private OldSkill skills = null;

    [SerializeField]
    private Text skillText = null;

    [SerializeField]
    private Image obfuscator = null;

    [SerializeField]
    private Image cardImage = null;

    [SerializeField]
    private GameObject DamageTextPrototype = null;

    [SerializeField]
    private AudioRequisitor audioRequisitor = null;
    [SerializeField]
    private AudioClip critSFX = null;

    private bool freezing = false;

    private GameObject freezingEffect = null;

    private Battlefield battlefield;

    [SerializeField]
    private Sprite horizontalSprite = null;

    [SerializeField]
    private RectTransform skillsHorizontalSpot = null;
    [SerializeField]
    private RectTransform attackPowerHorizontalSpot = null;
    [SerializeField]
    private RectTransform vitalityHorizontalSpot = null;

    [SerializeField]
    private Shakeable shakeable = null;

    [SerializeField]
    private float fadingDurationOnDeath = 0.5f;

    [SerializeField]
    private Color normalVitalityColor = Color.green;
    private Color overhealedColor = Color.yellow;

    #endregion

    #region Properties

    private static int[] DeathCount { get => deathCount;  set => deathCount = value;  }
    public bool Freezing { get => freezing; }
    public int Vitality { get => vitality; }
    public int AttackPower { get => attackPower; set => attackPower = value; }
    public Battlefield Battlefield { get => battlefield; set => battlefield = value; }
    public Classes Classe { get => classInfo.Classe; }

    public OldSkill Skills {
        get => skills;
        set {
            skills = value;
            skillText.text = skills.Acronym;
        }
    }

    #endregion

    private void Awake()
    {
        classInfo.TryToRegisterCardInClass(this);

        attackPowerText.text = AttackPower.ToString();
        
        skillText.text = skills.Acronym;

        AjustInitialAndLimitVitality();
    }

    private void Start()
    {
        if (skills == null)
        {
            skills = skillsMediator.GetBasicAttackSkill();
        }

        overhealedColor = ClassInfo.GetColorOfClass(Classes.CLERIC);
        SetVitalityAndUpdateTextLooks(Vitality);
    }

    private void AjustInitialAndLimitVitality()
    {
        initialVitality = vitality;
        vitalityLimit = vitality + vitality;
    }

    public void AttackSelectedCard(Battlefield opponentBattlefield, Battlefield attackerBattlefield)
    {
        Skills.ApplyEffectsConsideringSelectedTarget(opponentBattlefield, attackerBattlefield);
    }

    #region damage
    public void TakeDamageAndManageCardState(int damage)
    {
        if (damage >= 0)
        {
            if (damage >= 4)
            {
                audioRequisitor.RequestSFX(critSFX);
            }

            SetVitalityAndUpdateTextLooks(Vitality - damage);

            if (damage > 0)
            {
                CreateDamageAnimatedText(damage);
            }

            shakeable.Shake();

            if (Vitality <= 0)
            {
                RemoveFreezing();
                battlefield.Remove(this);
                StartCoroutine(DieWithAnimation());
            }
        }
        else if (damage < 0)
        {
            Debug.LogError("[Card] tryed to apply negative damage. That's wrong! Use Heal method instead");
        }

        if (Vitality > 0)
        {
            RegisterSurvived();
        }
        else
        {
            RegisterDeath();
        }
    }

    private void CreateDamageAnimatedText(int damage)
    {
        RectTransform damageTextTransform = Instantiate(DamageTextPrototype).GetComponent<RectTransform>();

        damageTextTransform.SetParent(UIBattle.parentOfDynamicUIThatMustAppear, false);
        damageTextTransform.position = DamageTextPrototype.transform.position;
        damageTextTransform.Rotate(new UnityEngine.Vector3(0, 0, 90));

        damageTextTransform.GetComponent<Text>().text = damage.ToString();

        damageTextTransform.gameObject.SetActive(true);
    }

    private void RegisterDeath()
    {
        deathCountIndex = (deathCountIndex + 1) % DeathCount.Length;
        DeathCount[deathCountIndex] = 1;
    }

    private void RegisterSurvived()
    {
        deathCountIndex = (deathCountIndex + 1) % DeathCount.Length;
        DeathCount[deathCountIndex] = 0;
    }

    // TODO: create script to put in each object that will fade
    private IEnumerator DieWithAnimation()
    {
        float fadePercentage = 1.0f;
        float countDownFadeTimer = fadingDurationOnDeath;

        Image[] images = GetComponentsInChildren<Image>();

        Text[] texts = GetComponentsInChildren<Text>();

        Color[] originalColors = new Color[images.Length];

        float[] r = new float[originalColors.Length]; float[] g = new float[originalColors.Length];
        float[] b = new float[originalColors.Length];

        for (int i = 0; i < images.Length; i++)
        {
            originalColors[i] = images[i].color;
            Color color = originalColors[i];
            r[i] = color.r; g[i] = color.g; b[i] = color.b;
        }

        float[] tr = new float[originalColors.Length]; float[] tg = new float[originalColors.Length];
        float[] tb = new float[originalColors.Length];
        for (int i = 0; i < texts.Length; i++)
        {
            tr[i] = texts[i].color.r;
            tg[i] = texts[i].color.g;
            tb[i] = texts[i].color.b;
        }

        while (countDownFadeTimer > 0.0f)
        {
            fadePercentage = countDownFadeTimer / fadingDurationOnDeath;

            for (int i = 0; i < images.Length; i++)
            {
                if (images[i] != null)
                {
                    images[i].color = new Color(r[i], g[i], b[i], fadePercentage); ;
                }
            }

            for (int i = 0; i < texts.Length; i++)
            {
                if (texts[i] != null)
                {
                    texts[i].color = new Color(tr[i], tg[i], tb[i], fadePercentage);
                }
            }

            countDownFadeTimer -= Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    public void AjustCardToDifficult(int difficultyLevel)
    {
        attackPower = attackPower * difficultyLevel;
    }

    public void InconditionalHealing(int healAmount)
    {
        SetVitalityAndUpdateTextLooks(Vitality + healAmount);
    }

    public void HealNotExceedingDoubleVitalityLimit(int healAmount)
    {
        int vit = Mathf.Min(vitalityLimit, vitality + healAmount);

        SetVitalityAndUpdateTextLooks(vit);
    }

    public bool CanBeHealed()
    {
        return vitality < vitalityLimit;
    }

    private void SetVitalityAndUpdateTextLooks(int value)
    {   
        vitality = value;
        UpdateVitalityTextAndItsColor();
    }

    private void UpdateVitalityTextAndItsColor()
    {
        vitalityText.text = vitality.ToString();

        if (vitality <= initialVitality)
        {
            vitalityText.color = normalVitalityColor;
        }
        else
        {
            vitalityText.color = overhealedColor;
        }
    }
    #endregion

    #region Has XXX Effect
    public bool HasBlockSkill()
    {
        return Skills.HasBlockEffect();
    }

    public bool HasHeavyArmorSkill()
    {
        return Skills.HasHeavyArmorEffect();
    }

    public bool HasReflectSkill()
    {
        return Skills.HasReflectEffect();
    }
    #endregion

    public string GetSkillFullName()
    {
        return skills.FullName;
    }

    public void SetObfuscate(bool obfuscate)
    {
        obfuscator.gameObject.SetActive(obfuscate);
    }

    public void ShowDefenseVFXandSFXIfHasBlockOrReflect(float attackerYPosition)
    {
        if(skills.HasReflectEffect() || skills.HasHeavyArmorEffect())
        {
            ShowDefenseVFXandSFX( attackerYPosition );
        }
    }

    public void ShowDefenseVFXandSFX(float attackerYPosition)
    {
        ShowDefenseVFX(attackerYPosition);
        ShowDefenseSFX();
    }

    public void ShowDefenseVFX(float attackerYPosition)
    {
        UnityEngine.Vector3 forwards = new UnityEngine.Vector3(0, 0, -transform.position.y);
        UnityEngine.Vector3 upwards = new UnityEngine.Vector3(0, 0, -1);
        UnityEngine.Quaternion lookRotation = UnityEngine.Quaternion.LookRotation(forwards, upwards);
        GameObject vfx = Instantiate(skills.DefenseVFX, transform.position, UnityEngine.Quaternion.identity);
        vfx.GetComponent<RectTransform>().SetParent(transform, false);
        vfx.GetComponent<RectTransform>().localPosition = UnityEngine.Vector3.zero;

        float y = attackerYPosition - transform.position.y;

        if (y > 0)
        {
            vfx.transform.eulerAngles = new UnityEngine.Vector3(0, 0, 0);
        }
        else
        {
            vfx.transform.eulerAngles = new UnityEngine.Vector3(0, 0, 180);
        }
    }

    public void ChangeToHorizontalVersion()
    {
        cardImage.sprite = horizontalSprite;

        attackPowerText.transform.Rotate(new UnityEngine.Vector3(0,0,-90));
        vitalityText.transform.Rotate(new UnityEngine.Vector3(0, 0, -90));
        skillText.transform.parent.Rotate(new UnityEngine.Vector3(0, 0, -90));

        vitalityText.transform.parent.position = vitalityHorizontalSpot.position;
        attackPowerText.transform.parent.position = attackPowerHorizontalSpot.position;
        skillText.transform.parent.position = skillsHorizontalSpot.position;
    }

    private void ShowDefenseSFX()
    {
        skills.PlayDefenseSFX();
    }

    public void ApplyFreezing(GameObject freezingEffect)
    {
        RemoveFreezing();
        freezing = true;
        this.freezingEffect = freezingEffect;
        ChildMaker.AdoptAndTeleport(transform, freezingEffect.GetComponent<RectTransform>());
        freezingEffect.transform.localScale = UnityEngine.Vector3.one;
    }

    public void RemoveFreezing()
    {
        freezing = false;
        if (freezingEffect != null)
        {
            Destroy(freezingEffect);
        }
        freezingEffect = null;
    }

    public float GetDamageReductionPercentage()
    {
        return skills.DamageReductionPercentage;
    }
    public float GetDamageReflectionPercentage()
    {
        return skills.DamageReflectionPercentage;
    }

    public string GetColoredTitleForTooltip()
    {
        return "<color=#"+ classInfo.ColorHexCode + ">"+ Classe+"</color>";
    }

    public Sprite GetCardSprite()
    {
        return cardImage.sprite;
    }

    public string GetExplanatoryText()
    {
        return GetSkillsExplanatoryText() + "\n" +
               "<color=#FD7878>Attack Power: " + attackPower + "</color>\n" +
               "<color=#9EFA9D>Vitality: " + vitality + "</color>\n";
    }

    public string GetSkillsExplanatoryText()
    {
        return skills.GetExplanatoryText(attackPower);
    }

    public void SumPlayerBonuses()
    {
        attackPower += classInfo.AttackPowerBonus;
        attackPowerText.text = attackPower.ToString();

        vitality += classInfo.VitalityBonus;
        AjustInitialAndLimitVitality();
        UpdateVitalityTextAndItsColor();
    }

    public static int GetDeathCount()
    {
        int counter = 0;
        for (int i = 0; i < DeathCount.Length; i++)
        {
            counter += DeathCount[i];
        }
        return counter;
    }

    public static void ResetDeathCount()
    {
        for (int i = 0; i < DeathCount.Length; i++)
        {
            DeathCount[i] = 1;
        }
    }

    public void BuffAttackPowerForThisMatch()
    {
        attackPower++;
        attackPowerText.text = attackPower.ToString();
    }

    public bool IsAnotherInstanceOf(Card card)
    {
        return card.Skills == skills;
    }

    public void MakeColorGray()
    {
        const float GRAY_INTENSITY = 0.5f;
        const float ALPHA = 1.0f;
        cardImage.color = new Color(GRAY_INTENSITY, GRAY_INTENSITY, GRAY_INTENSITY, ALPHA);
    }

    public void MakeColorDefault()
    {
        cardImage.color = Color.white;
    }

    public Card GetClone()
    {
        return Instantiate(this);
    }
}
