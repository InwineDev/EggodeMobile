using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using Unity.VisualScripting;
using System.Globalization;
using RuntimeNodeEditor;
using Mirror.BouncyCastle.Asn1;

public class scriptor : NetworkBehaviour
{
    /*    [SyncVar]
        public GraphData graph = new GraphData();
    */
    /*    public SyncList<NodeData> nodeDatas = new SyncList<NodeData>();
        public SyncList<ConnectionData> connDatas = new SyncList<ConnectionData>();
    */
    public string nodesCode;

    public GameObject OnTriggerData;
    public GameObject OnE;

    [SyncVar]
    public string TpCord;

    [SyncVar]
    public string Destroy;

    [SyncVar]
    public string Damagenum;

    [SyncVar]
    public string Speed;

    [SyncVar]
    public string Jump;

    [SyncVar]
    public string SetSize;

    [SyncVar]
    public string PlayAnim;

    [SyncVar]
    public int type = 0;

    [SyncVar(hook = nameof(IISet))]
    public bool II;

    [SyncVar(hook = nameof(AnimSet))]
    public string Animation;

    [SyncVar]
    public string SetPlayerVarible;

    [SyncVar]
    public int SetIntPlayerVarible;

    [SyncVar]
    public string PlayerVaribleIf;

    [SyncVar]
    public string PlayerVaribleIfMoreInt;

    [SyncVar]
    public string AddItem;

    public GraphData graphData;

    [System.Serializable]
    public class NodeData
    {
        public string id;
        public List<NodeValue> values;
        public float posX, posY;
        public string path;
        public List<string> inputSocketIds;
        public List<string> outputSocketIds;
    }

    [System.Serializable]
    public class NodeValue
    {
        public string key;
        public string value;
    }

    [System.Serializable]
    public class ConnectionData
    {
        public string id;
        public string outputSocketId;
        public string inputSocketId;
    }

    [System.Serializable]
    public class GraphData
    {
        public List<NodeData> nodes;
        public List<ConnectionData> connections;
    }
    private void Start()
    {
        //graphData = JsonUtility.FromJson<GraphData>(nodesCode);
        CmdStart();
    }

    [Command(requiresAuthority = false)]
    void CmdStart()
    {
        var executor = new VirtualGraphExecutor();
        executor.Load(nodesCode, gameObject, null);
        executor.Execute(gameObject, "Nodes/OnStartNode", gameObject);
    }

    [Command(requiresAuthority = false)]
    void CmdTrigger(GameObject toSave)
    {
        OnTriggerData = toSave;
        var executor = new VirtualGraphExecutor();
        executor.Load(nodesCode, gameObject, toSave.gameObject);
        executor.Execute(gameObject, "Nodes/OnTriggerEnterNode", toSave.gameObject);
    }
    [Command(requiresAuthority = false)]
    void CmdCollision(GameObject toSave)
    {
        //OnTriggerData = toSave;
        var executor = new VirtualGraphExecutor();
        executor.Load(nodesCode, gameObject, toSave.gameObject);
        executor.Execute(gameObject, "Nodes/OnCollisionEnterNode", toSave.gameObject);
    }

    void AnimSet(string old1, string new1)
    {
        gameObject.AddComponent<Animat>().animtext = new1;
    }

    void IISet(bool old1, bool new1)
    {
        gameObject.AddComponent<IIScript>();
    }

    private void OnTriggerEnter(Collider other2)
    {
        if (other2.GetComponent<NetworkIdentity>().isOwned)
        {
            CmdTrigger(other2.gameObject);
            print(other2);
        }
        if (type == 0)
        {
            typeController(other2.gameObject);
        }
    }

    private void OnCollisionEnter(Collision other2)
    {
        if (other2.gameObject.GetComponent<NetworkIdentity>().isOwned)
        {
            CmdCollision(other2.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CmdClearTriggerData();
    }

    [Command(requiresAuthority = false)]
    void CmdClearTriggerData()
    {
        OnTriggerData = null;
    }

    [Command]
    void CmdAnimStrart(Animat anim)
    {
        RpcAnimStart(anim);
    }
    [ClientRpc]
    void RpcAnimStart(Animat anim)
    {
        anim.animstart();
    }
    public void typeController(GameObject other)
    {
        if (!String.IsNullOrEmpty(PlayerVaribleIfMoreInt))
        {
            if (other.GetComponent<userSettingNotCam>().VaribleInt < int.Parse(PlayerVaribleIfMoreInt))
            {
                return;
            }
            else
            {
                other.GetComponent<userSettingNotCam>().VaribleInt -= int.Parse(PlayerVaribleIfMoreInt);
            }
        }

        if (!String.IsNullOrEmpty(PlayerVaribleIf))
        {
            if (other.GetComponent<userSettingNotCam>().Varible != PlayerVaribleIf)
            {
                return;
            }
        }

        if (!String.IsNullOrEmpty(TpCord))
        {

            string[] elements = TpCord.Split(',');
            if (elements.Length == 3)
            {
                float x = float.Parse(elements[0], NumberStyles.Any,
                      CultureInfo.InvariantCulture);
                float y = float.Parse(elements[1], NumberStyles.Any,
                      CultureInfo.InvariantCulture);
                float z = float.Parse(elements[2], NumberStyles.Any,
                      CultureInfo.InvariantCulture);

                Vector3 sus = new Vector3(x, y, z);
                other.transform.position = sus;
            }
            else
            {
                name24[] allObjects = GameObject.FindObjectsOfType<name24>();

                foreach (name24 obj in allObjects)
                {
                    if (obj.GetComponent<name24>().id == TpCord & !String.IsNullOrEmpty(obj.GetComponent<name24>().id))
                    {
                        Vector3 sus = obj.transform.position;
                        other.transform.position = sus;
                    }
                }
            }
        }
        if (!String.IsNullOrEmpty(Damagenum))
        {
            Damage(int.Parse(Damagenum), other.GetComponent<Health>());
        }
        if (!String.IsNullOrEmpty(Speed))
        {
            other.GetComponent<FirstPersonController>().walkSpeed = float.Parse(Speed);
            other.GetComponent<FirstPersonController>().sprintSpeed = float.Parse(Speed) + 2f;
        }
        if (!String.IsNullOrEmpty(Jump))
        {
            other.GetComponent<FirstPersonController>().jumpPower = float.Parse(Jump);
        }

        if (!String.IsNullOrEmpty(PlayAnim))
        {
            Animat[] allObjects = GameObject.FindObjectsOfType<Animat>();
            foreach (Animat obj in allObjects)
            {
                if (obj.GetComponent<name24>().id == PlayAnim && !String.IsNullOrEmpty(obj.GetComponent<name24>().id))
                {
                    obj.animstart();
                    //RpcAnimStart(obj);
                }
            }
        }
        if (!String.IsNullOrEmpty(SetSize))
        {
            string[] elements = SetSize.Split(',');

            float x = float.Parse(elements[0], NumberStyles.Any,
                  CultureInfo.InvariantCulture);
            float y = float.Parse(elements[1], NumberStyles.Any,
                  CultureInfo.InvariantCulture);
            float z = float.Parse(elements[2], NumberStyles.Any,
                  CultureInfo.InvariantCulture);

            Vector3 sus = new Vector3(x, y, z);
            other.transform.localScale = sus;
        }

        if (!String.IsNullOrEmpty(SetPlayerVarible))
        {
            userSettingNotCam s = other.GetComponent<userSettingNotCam>();
            if (s)
            {
                s.Varible = SetPlayerVarible;
            }
        }

        if (SetIntPlayerVarible != 0)
        {
            userSettingNotCam s = other.GetComponent<userSettingNotCam>();
            if (s)
            {
                s.VaribleInt += SetIntPlayerVarible;
            }
        }

        if (!String.IsNullOrEmpty(AddItem))
        {
            userSettings s = other.GetComponent<userSettingNotCam>().us;
            if (s)
            {
                string[] elements = AddItem.Split(',');

                int x = Int32.Parse(elements[0], NumberStyles.Any,
                      CultureInfo.InvariantCulture);
                int y = Int32.Parse(elements[1], NumberStyles.Any,
                      CultureInfo.InvariantCulture);
                s.items[x].GetComponent<SyncActive>().tpk.itemdat.gameObject.SetActive(true);
                s.items[x].GetComponent<SyncActive>().tpk.itemdat.amount += y;
                s.items[x].GetComponent<SyncActive>().tpk.itemdat.sus3.text = s.items[x].GetComponent<SyncActive>().tpk.itemdat.amount.ToString() + " штук";
            }
        }

        if (!String.IsNullOrEmpty(Destroy))
        {
            name24[] allObjects = GameObject.FindObjectsOfType<name24>();

            foreach (name24 obj in allObjects)
            {
                if (obj.id == Destroy & !String.IsNullOrEmpty(obj.id) & GetComponent<name24>() != obj)
                {
                    Del(obj.gameObject);
                }
            }
            if (GetComponent<name24>().id == Destroy) Del(gameObject);
        }
    }

    [Command(requiresAuthority = false)]
    void Del(GameObject delled)
    {
        NetworkServer.Destroy(delled);
    }
/*
    [Ta]
    void DamageRPC(int uron, Health sus)
    {
        Damage(uron, sus);
    }*/

    [Command(requiresAuthority = false)]
    void Damage(int uron, Health sus)
    {
        try
        {
            bool uron2 = serverProperties.instance.hp;
            if (uron2 & sus != null)
            {
                sus.health -= uron;
                if (sus.health <= 0)
                {
                    sus.health = 100;
                    sus.hp.text = $"{sus.health} HP";
                }
            }
        } 
        catch
        {
            print("strah");
        }
    }
}