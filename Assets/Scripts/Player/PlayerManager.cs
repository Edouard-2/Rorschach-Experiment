using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerLook))]
[RequireComponent(typeof(PlayerVision))]
[RequireComponent(typeof(PlayerInteractions))]
public class PlayerManager : Singleton<PlayerManager>
{
    //Constante
    private const float m_gravity = -9.81f;

    //Scripts
    [SerializeField, Tooltip("Script player controller")] private PlayerController m_controllerScript;
    [SerializeField, Tooltip("Script player look")] private PlayerLook m_lookScript;
    [SerializeField, Tooltip("Script player vision")] private PlayerVision m_visionScript;
    [SerializeField, Tooltip("Script player door")] private PlayerInteractions m_doorActivationScript;

    private float m_timeVision;
    private float tTime;
    public float Gravity
    {
        get => m_gravity;
    }

    public delegate void DoVisionSwitch();
    public DoVisionSwitch DoVisibleToInvisibleHandler;
    
    private void Awake()
    {
        m_visionScript.m_matVision.SetFloat("_BlurSize",0);
        m_visionScript.m_matInvisibleVisible.SetFloat("_StepStrenght",-0.03f);
        m_visionScript.m_matVisibleInvisible.SetFloat("_StepStrenght",-0.03f);

        if (m_controllerScript == null)
            m_controllerScript = GetComponent<PlayerController>();

        if (m_lookScript == null)
            m_lookScript = GetComponent<PlayerLook>();

        if (m_visionScript == null)
            m_visionScript = GetComponent<PlayerVision>();
        
        if (m_doorActivationScript == null)
            m_doorActivationScript = GetComponent<PlayerInteractions>();
    }
    
    private void Update()
    {
        //Mouvement du Joueur
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            m_controllerScript.Mouvement();
        }

        //Mouvement de la camera
        m_lookScript.CursorMouvement();

        
        
        //Input Blur Effect
        IsInputDown();
    }
    
    public void InitVariableChangement()
    {
        if (m_visionScript.m_readyInitVision)
        {
            Debug.Log("Init variable pour vision");
            m_timeVision = Time.time;
            tTime = Time.time - m_timeVision;

            m_visionScript.m_readyEnd = Mathf.Abs(m_visionScript.m_readyEnd - 1);

            m_visionScript.m_resetTimeVisionComp = true;
            m_visionScript.m_resetTimeVisionMat = true;
            
            DoVisibleToInvisibleHandler?.Invoke();
        }
    }

    public void IsInputDown()
    {
        tTime = Time.time - m_timeVision;

        //Changement de vision
        if (Input.GetKeyDown(KeyCode.Space) && !m_visionScript.m_resetTimeVisionComp && !m_visionScript.m_resetTimeVisionMat )
        {

            InitVariableChangement();

            Debug.Log(m_visionScript.m_readyEnd);

            if (m_visionScript.m_readyEnd == 0)
            {
                Debug.Log("hey");
                //Ajout du cran sur la BV
                m_visionScript.AddStepBV();
            }

        }

        if (m_visionScript.m_readyEnd == 0)
        {
            if (m_visionScript.m_resetTimeVisionComp)
            {
                //Debug.Log("Start");

                //DoSwitchView(allé)
                m_visionScript.DoSwitchView(tTime, m_visionScript.m_curveVisionStart);
            }
            if (m_visionScript.m_resetTimeVisionMat)
            {
                //DoSwitchMaterial(allé)
                m_visionScript.DoSwitchMaterial(tTime, m_visionScript.m_curveMatVisionStart);
            }

            //Lancement de la consommation de BV
            m_visionScript.DecreaseBV();
        }

        else if (m_visionScript.m_readyEnd == 1)
        {
            if (m_visionScript.m_resetTimeVisionComp)
            {
                //Debug.Log("Fin");
                //DoSwitchView(retour)
                m_visionScript.DoSwitchView(tTime, m_visionScript.m_curveVisionFinish);
            }
            if (m_visionScript.m_resetTimeVisionMat)
            {
                //DoSwitchMaterial(retour)
                m_visionScript.DoSwitchMaterial(tTime, m_visionScript.m_curveMatVisionFinish);
            }
            m_visionScript.IncreaseBV();
        }
    }

    protected override string GetSingletonName()
    {
        return "PlayerManager";
    }
}