using UnityEngine.UI;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    //LayerMask Visible
    [SerializeField, Tooltip("Layer pour les door")] private LayerMask m_layerDoor;
    [SerializeField, Tooltip("Layer pour les key")] private LayerMask m_layerKey;
    
    //LayerMaske Invisible
    [SerializeField, Tooltip("Layer pour les doorInvisible")] private LayerMask m_layerDoorInvisible;
    [SerializeField, Tooltip("Layer pour les keyInvisible")] private LayerMask m_layerKeyInvisible;

    [SerializeField, Tooltip("Trousseau de clé")] private KeyType m_trousseauKey;
    [SerializeField, Tooltip("UI de la clés ingame")] private Image m_KeyUI;

    private GameObject m_keyObject;
    private Material m_currentAimObject;

    public void VerifyFeedbackInteract(Transform p_target)
    {
        if ((m_layerKey.value & (1 << p_target.gameObject.layer)) > 0 || (m_layerKeyInvisible.value & (1 << p_target.gameObject.layer)) > 0)
        {
            Material targetMaterial = p_target.GetComponent<LootBox>().m_key.m_keyMat;
            
            if (targetMaterial != null && targetMaterial.GetFloat("_isAim") != 1)
            {
                m_currentAimObject = targetMaterial;
                targetMaterial.SetFloat("_isAim", 1);
            }
        }
        else if ((m_layerDoor.value & (1 << p_target.gameObject.layer)) > 0 || (m_layerDoorInvisible.value & (1 << p_target.gameObject.layer)) > 0)
        {
            Material targetMaterial = p_target.GetComponent<Renderer>().material;
            
            if (targetMaterial != null && targetMaterial.GetFloat("_isAim") != 1)
            {
                m_currentAimObject = targetMaterial;
                targetMaterial.SetFloat("_isAim", 1);
            }
        }
    }

    public void ResetFeedbackInteract()
    {
        if(m_currentAimObject != null && m_currentAimObject.GetFloat("_isAim") != 0)
        {
            m_currentAimObject.SetFloat("_isAim", 0);
        }
    }
    
    public void VerifyLayer(Transform p_target)
    {
        if ((m_layerKey.value & (1 << p_target.gameObject.layer)) > 0 || (m_layerKeyInvisible.value & (1 << p_target.gameObject.layer)) > 0)
        {
            LootBox myLootBox = p_target.GetComponent<LootBox>();
            if( myLootBox && myLootBox.OpenChest(out KeyType key))
            {
                if (m_trousseauKey == null)
                {
                    m_trousseauKey = key;
                    m_keyObject = p_target.gameObject;
                    SetUIKey(myLootBox);
                }
                else
                {
                    //Enlever la précedente clé
                    EjectKey();
                    m_trousseauKey = key;
                    m_keyObject = p_target.gameObject;
                    SetUIKey(myLootBox);
                }
            }
        }

        else if ((m_layerDoor.value & (1 << p_target.gameObject.layer)) > 0 || (m_layerDoorInvisible.value & (1 << p_target.gameObject.layer)) > 0) 
        {
            Door myDoor =  p_target.GetComponent<Door>();
            if (myDoor)
            {
                if (myDoor.OpenDoor(m_trousseauKey, p_target.gameObject))
                {
                    m_trousseauKey = null;
                    StartCoroutine(m_keyObject.GetComponent<LootBox>().DestroySelf());
                    m_KeyUI.color = Color.clear;
                }
            }
        }
        else
        {
            Debug.Log("Rien pour intéragir");
        }
    }

    private void SetUIKey(LootBox p_key)
    {
        Debug.Log(p_key.m_key.m_keyMat);

        m_KeyUI.color = new Vector4(p_key.m_key.m_keyMat.GetColor("_BaseColor").r,p_key.m_key.m_keyMat.GetColor("_BaseColor").g,p_key.m_key.m_keyMat.GetColor("_BaseColor").b,1);
    }
    
    private void EjectKey()
    {
        //Ejecter la clé
        Debug.Log("clear");
        m_keyObject.transform.position = transform.position + transform.forward;
        m_KeyUI.color = Color.clear;
    }
}