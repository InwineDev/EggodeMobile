using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Mirror;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;
using System.Reflection.Emit;
using UnityEngine.Networking;
using RuntimeNodeEditor;
using RuntimeNodeEditor.Eggode;
using RuntimeHandle;

public class strelka_pos : MonoBehaviour
{
    public static strelka_pos me;
    private Vector3 _offset;
    private bool _isDragging;
    private bool _isRotating;
    public GameObject vibron;
    public TMP_InputField[] posrotsca;
    public AnimationCompiler aCompiler;

    [SerializeField] private textureList strah;
    public Toggle[] tog;

    public GameObject npcButton;

    public TMP_Dropdown dropType;

    public bool active = true;

    [SerializeField] private GameObject GizmoPos, GizmoScale, GizmoRotate;

    [SerializeField] private MapEditorNodeEditor mene;

    private Stack<UndoOperation> undoStack = new Stack<UndoOperation>();


    private GameObject runtimeTransformGameObj;
    private RuntimeTransformHandle runtimeTransformHandle;

    private void Start()
    {
        me = this;
        runtimeTransformGameObj = new GameObject();
        runtimeTransformHandle = runtimeTransformGameObj.AddComponent<RuntimeTransformHandle>();
/*        runtimeTransformGameObj.layer = runtimeTransformLayer;
        runtimeTransformLayerMask = 1 << runtimeTransformLayer; */
        runtimeTransformHandle.type = HandleType.POSITION;
        runtimeTransformHandle.autoScale = true;
        runtimeTransformHandle.autoScaleFactor = 1.0f;
        runtimeTransformGameObj.tag = "Gizmo";
        runtimeTransformGameObj.layer = 9;
        runtimeTransformGameObj.SetActive(false);
    }
    private void OnMouseDown()
    {

        _isDragging = true;
    }

    private void OnMouseUp()
    {
        _isDragging = false;
        vibron.transform.position = transform.position;
    }

    public float movementSpeed = 10f;
    public float rotationSpeed = 100f;

    IEnumerator Pon(string texture2, GameObject sus)
    {
                if (texture2.Contains("eggodetexture//"))
                {
                    Texture2D tex = new Texture2D(500, 500, TextureFormat.PVRTC_RGBA4, false);

                    foreach (var item in strah.textures)
                    {
                        if(texture2 != "eggodetexture//" + item.myname) break;
                        byte[] pvrtcBytes = item.textureBytes;

                        tex.LoadRawTextureData(pvrtcBytes);
                        tex.Apply();

                        sus.GetComponent<Renderer>().material.mainTexture = tex;
                    }
                }
                else
                {
                    print(texture2);
                    var req = UnityWebRequestTexture.GetTexture(texture2);
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
            {
                print("otpariv");
                        Texture2D texture = DownloadHandlerTexture.GetContent(req);

                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));

                        print("poluchil");

                        sus.GetComponent<Renderer>().material.mainTexture = texture;

                        print("postavil");

                    }
                    else
                    {
                        print("Error loading image: " + req.error);
                    }
                }
        yield return null;
    }

    public void namana(string texture2)
    {
        print(texture2);
        StartCoroutine(Pon(texture2, vibron));
    }

    public void StopPolet()
    {
        active = false;
    }

    public void StartPolet()
    {
        active = true;
    }

    public void ChangePosition()
    {
        if (vibron == null) return;
        vibron.transform.position = new Vector3(float.Parse(posrotsca[0].text), float.Parse(posrotsca[1].text), float.Parse(posrotsca[2].text));
    }
    public void ChangeRotation()
    {
        if (vibron == null) return;
        vibron.transform.localEulerAngles = new Vector3(float.Parse(posrotsca[3].text), float.Parse(posrotsca[4].text), float.Parse(posrotsca[5].text));
    }
    public void ChangeScale()
    {
        if (vibron == null) return;
        vibron.transform.localScale = new Vector3(float.Parse(posrotsca[6].text), float.Parse(posrotsca[7].text), float.Parse(posrotsca[8].text));
    }
    public void ChangeColor()
    {
        if (vibron == null) return;
        if (vibron.GetComponent<Renderer>())
        {
            vibron.GetComponent<Renderer>().material.color = new Color(float.Parse(posrotsca[9].text) / 255f, float.Parse(posrotsca[10].text) / 255f, float.Parse(posrotsca[11].text) / 255f, float.Parse(posrotsca[13].text));
        }
    }
    public void ChangeTexture()
    {
        if (vibron == null) return;
        if (vibron.GetComponent<name24>().texture.Contains("eggodetexture//"))
        {
            foreach (var item in strah.textures)
            {
                if (posrotsca[12].text != "eggodetexture//" + item.myname) break;
                byte[] pvrtcBytes = item.textureBytes;
                foreach (byte b in pvrtcBytes)
                {
                    vibron.GetComponent<name24>().bytesForTexture.Add(b);
                }
                vibron.GetComponent<name24>().texture = posrotsca[12].text;
            }
        } else
        {
            vibron.GetComponent<name24>().texture = posrotsca[12].text;
        }
    }
    public void ChangetextureTile()
    {
        if (vibron == null) return;
        vibron.GetComponent<name24>().textureTile = posrotsca[29].text;
    }
    public void ChangeID()
    {
        if (vibron == null) return;
        vibron.GetComponent<name24>().id = posrotsca[14].text;
    }
    public void ChangeTP()
    {
        if (vibron == null) return;
        vibron.GetComponent<scriptor>().TpCord = posrotsca[15].text;
    }
    public void ChangeDestroy()
    {
        if (vibron == null) return;
        vibron.GetComponent<scriptor>().Destroy = posrotsca[16].text;
    }
    public void ChangeDamagenum()
    {
        if (vibron == null) return;
        vibron.GetComponent<scriptor>().Damagenum = posrotsca[17].text;
    }
    public void ChangeSpeed()
    {
        if (vibron == null) return;
        vibron.GetComponent<scriptor>().Speed = posrotsca[18].text;
    }
    public void ChangeJump()
    {
        if (vibron == null) return;
        vibron.GetComponent<scriptor>().Jump = posrotsca[19].text;
    }
    public void ChangeSetSize()
    {
        if (vibron == null) return;
        vibron.GetComponent<scriptor>().SetSize = posrotsca[20].text;
    }
    public void ChangePlayAnim()
    {
        if (vibron == null) return;
        vibron.GetComponent<scriptor>().PlayAnim = posrotsca[21].text;
    }
    public void ChangeAnimation()
    {
        if (vibron == null) return;
        vibron.GetComponent<scriptor>().Animation = posrotsca[22].text;
    }
    public void ChangeType()
    {
        if (vibron == null) return;
        vibron.GetComponent<scriptor>().type = dropType.value;
    }
    public void ChangeSetPlayerVarible()
    {
        if (vibron == null) return;
        vibron.GetComponent<scriptor>().SetPlayerVarible = posrotsca[24].text;
    }
    public void ChangePlayerVaribleIf()
    {
        if (vibron == null) return;
        vibron.GetComponent<scriptor>().PlayerVaribleIf = posrotsca[25].text;
    }
    public void ChangeAddItem()
    {
        if (vibron == null) return;
        vibron.GetComponent<scriptor>().AddItem = posrotsca[26].text;
    }

    public void ChangePlayerVaribleIfMoreInt()
    {
        if (vibron == null) return;
        vibron.GetComponent<scriptor>().PlayerVaribleIfMoreInt = posrotsca[27].text;
    }
    public void ChangeSetIntPlayerVarible()
    {
        if (vibron == null) return;
        vibron.GetComponent<scriptor>().SetIntPlayerVarible = int.Parse(posrotsca[28].text);
    }

    public void ChangenpcReplics()
    {
        if (vibron == null) return;
        if (vibron.GetComponent<npcController>())
        {
            vibron.GetComponent<npcController>().npcReplics = posrotsca[23].text;
        }
    }
    public void ChangeisLomatel()
    {
        if (vibron == null) return;
        if (tog[0].isOn)
        {
            vibron.GetComponent<name24>().isLomatel = true;
        }
        else
        {
            vibron.GetComponent<name24>().isLomatel = false;
        }
    }
    public void ChangeisRigidbody()
    {
        if (vibron == null) return;
        if (tog[1].isOn)
        {
            vibron.GetComponent<name24>().isRigidbody = true;
        }
        else
        {
            vibron.GetComponent<name24>().isRigidbody = false;
        }
    }

    public void ChangeisCollider()
    {
        if (vibron == null) return;
        if (tog[2].isOn)
        {
            vibron.GetComponent<name24>().isCollider = true;
        }
        else
        {
            vibron.GetComponent<name24>().isCollider = false;
        }
    }
    public void ChangenpcButton()
    {
        if (vibron == null) return;
        if (vibron.GetComponent<npcController>())
        {
            npcButton.SetActive(true);
        }
        else
        {
            npcButton.SetActive(false);
        }
    }
    public void AddPosition()
    {
        posrotsca[0].text = vibron.transform.position.x.ToString();
        posrotsca[1].text = vibron.transform.position.y.ToString();
        posrotsca[2].text = vibron.transform.position.z.ToString();
    }

    public void AddRotation()
    {
        posrotsca[3].text = vibron.transform.localEulerAngles.x.ToString();
        posrotsca[4].text = vibron.transform.localEulerAngles.y.ToString();
        posrotsca[5].text = vibron.transform.localEulerAngles.z.ToString();
    }

    public void AddScale()
    {
        posrotsca[6].text = vibron.transform.localScale.x.ToString();
        posrotsca[7].text = vibron.transform.localScale.y.ToString();
        posrotsca[8].text = vibron.transform.localScale.z.ToString();
    }
    void LateUpdate()
    {
        if (!active) return;
        if (vibron)
        {
            vibron.GetComponent<scriptor>().Animation = aCompiler.GetCompiledAnimationAsJson();
            posrotsca[0].text = vibron.transform.position.x.ToString();
            posrotsca[1].text = vibron.transform.position.y.ToString();
            posrotsca[2].text = vibron.transform.position.z.ToString();
        }
        if (Input.GetKeyDown(KeyCode.Delete))
            dellcube();

        if (Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.LeftControl))
            Undo();

        if (Input.GetKeyDown(KeyCode.D) & Input.GetKey(KeyCode.LeftControl))
            copycube();

        if (runtimeTransformGameObj.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                runtimeTransformHandle.type = HandleType.POSITION;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                runtimeTransformHandle.type = HandleType.ROTATION;
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                runtimeTransformHandle.type = HandleType.SCALE;
            }
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKeyDown(KeyCode.G))
                {
                    runtimeTransformHandle.space = HandleSpace.WORLD;
                }
                if (Input.GetKeyDown(KeyCode.L))
                {
                    runtimeTransformHandle.space = HandleSpace.LOCAL;
                }
            }
        }


        if (Input.GetMouseButton(1))
        {
            _isRotating = true;
        }
        else
        {
            _isRotating = false;
        }
        if (_isDragging)
        {
            // Обновляем позицию объекта в соответствии с положением курсора мыши
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePosition + _offset;
        }

        if (_isRotating)
        {
            // Вращение камеры
            float rotateHorizontal = Input.GetAxis("Mouse X");
            float rotateVertical = Input.GetAxis("Mouse Y");

            transform.Rotate(Vector3.up, rotateHorizontal * rotationSpeed * Time.deltaTime);
            transform.Rotate(Vector3.left, rotateVertical * rotationSpeed * Time.deltaTime);
            // Устанавливаем вращение по оси Z равным 0
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        }
        if (Input.GetMouseButtonDown(0)/* & Input.GetKey(KeyCode.LeftShift)*/)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            int layerMask = ~LayerMask.GetMask("UI", "Layer");

            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

            foreach (RaycastHit hit1 in hits)
            {
                if (hit1.transform.root.gameObject.layer == 9)
                {
                    Debug.Log("Нашёл объект на 9-м слое: " + hit1.transform.name);
                    return;
                }
            }

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                print(hit.transform.root);
                if (hit.transform.root.tag != "Gizmo")
                {
                    if (vibron != null)
                    {
                        Destroy(vibron.GetComponent<Outline>());
                    }
                    vibron = null;
                    GameObject localvibron = hit.collider.gameObject;
                    localvibron.AddComponent<Outline>();
                    posrotsca[0].text = localvibron.transform.position.x.ToString();
                    posrotsca[1].text = localvibron.transform.position.y.ToString();
                    posrotsca[2].text = localvibron.transform.position.z.ToString();

                    posrotsca[3].text = localvibron.transform.localEulerAngles.x.ToString(); 
                    posrotsca[4].text = localvibron.transform.localEulerAngles.y.ToString();
                    posrotsca[5].text = localvibron.transform.localEulerAngles.z.ToString();
                    posrotsca[29].text = localvibron.GetComponent<name24>().textureTile;
                    posrotsca[6].text = localvibron.transform.localScale.x.ToString();
                    posrotsca[7].text = localvibron.transform.localScale.y.ToString();
                    posrotsca[8].text = localvibron.transform.localScale.z.ToString();
                    posrotsca[12].text = localvibron.GetComponent<name24>().texture;

                    if (localvibron.GetComponent<Renderer>())
                    {
                        float omg1 = localvibron.GetComponent<Renderer>().material.color.r * 255f;
                        float omg2 = localvibron.GetComponent<Renderer>().material.color.g * 255f;
                        float omg3 = localvibron.GetComponent<Renderer>().material.color.b * 255f;
                        float omg4 = localvibron.GetComponent<Renderer>().material.color.a;

                        posrotsca[9].text = omg1.ToString();
                        posrotsca[10].text = omg2.ToString();
                        posrotsca[11].text = omg3.ToString();
                        posrotsca[13].text = omg4.ToString();
                    }
                    //localvibron.GetComponent<Renderer>()
                    posrotsca[14].text = localvibron.GetComponent<name24>().id;
                    posrotsca[15].text = localvibron.GetComponent<scriptor>().TpCord;
                    posrotsca[16].text = localvibron.GetComponent<scriptor>().Destroy;
                    posrotsca[17].text = localvibron.GetComponent<scriptor>().Damagenum;
                    posrotsca[18].text = localvibron.GetComponent<scriptor>().Speed;
                    posrotsca[19].text = localvibron.GetComponent<scriptor>().Jump;
                    posrotsca[20].text = localvibron.GetComponent<scriptor>().SetSize;
                    posrotsca[21].text = localvibron.GetComponent<scriptor>().PlayAnim;
                    posrotsca[22].text = localvibron.GetComponent<scriptor>().Animation;
                    dropType.value = localvibron.GetComponent<scriptor>().type;
                    posrotsca[24].text = localvibron.GetComponent<scriptor>().SetPlayerVarible;
                    posrotsca[25].text = localvibron.GetComponent<scriptor>().PlayerVaribleIf;
                    posrotsca[26].text = localvibron.GetComponent<scriptor>().AddItem;
                    posrotsca[27].text = localvibron.GetComponent<scriptor>().PlayerVaribleIfMoreInt;
                    posrotsca[28].text = localvibron.GetComponent<scriptor>().SetIntPlayerVarible.ToString();
                    mene.LoadGraph(localvibron.GetComponent<scriptor>().nodesCode);
                    if (localvibron.GetComponent<npcController>())
                    {
                        posrotsca[23].text = localvibron.GetComponent<npcController>().npcReplics;
                    }

                    if (hit.transform.gameObject.GetComponent<name24>().isRigidbody)
                    {
                        tog[1].isOn = true;
                    }
                    else
                    {
                        tog[1].isOn = false;
                    }

                    if (hit.transform.gameObject.GetComponent<name24>().isCollider)
                    {
                        tog[2].isOn = true;
                    }
                    else
                    {
                        tog[2].isOn = false;
                    }

                    if (hit.transform.gameObject.GetComponent<name24>().isLomatel)
                    {
                        tog[0].isOn = true;
                    }
                    else
                    {
                        tog[0].isOn = false;
                    }
                    vibron = localvibron;
                    //if(!GizmoPos.activeSelf & !GizmoRotate.activeSelf & !GizmoScale.activeSelf) GizmoPos.SetActive(true);
                    aCompiler.UpdateAnimations();

                    runtimeTransformGameObj.SetActive(true);
                    runtimeTransformHandle.target = vibron.transform;
                }
            }
        }

        // Перемещение камеры
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(moveHorizontal, 0, moveVertical);
        transform.Translate(moveDirection * movementSpeed * Time.deltaTime);
    }


    public GameObject slider;


    public void copycube()
    {
        GameObject localVibron = Instantiate(vibron);
        localVibron.transform.parent = slider.transform;
        localVibron.transform.localEulerAngles = vibron.transform.localEulerAngles;
        localVibron.SetActive(true);
        Destroy(vibron.GetComponent<Outline>());
        vibron = null;
        vibron = localVibron;
        vibron.AddComponent<Outline>();
    }

    public void dellcube()
    {
        Destroy(vibron);
        vibron = null;
    }
    private void Undo()
    {
        if (undoStack.Count == 0) return;

        var operation = undoStack.Pop();
        if (operation.isDeleted)
        {
            operation.target.SetActive(true);
            vibron = operation.target;
            vibron.AddComponent<Outline>();
        }
        else if (operation.isCreated)
        {
            Destroy(operation.target);
            vibron = null;
        }
        else if (operation.target != null)
        {
            operation.target.transform.position = operation.position;
            operation.target.transform.localEulerAngles = operation.rotation;
            operation.target.transform.localScale = operation.scale;
            var renderer = operation.target.GetComponent<Renderer>();
            if (renderer) renderer.material.color = operation.color;
            var name24Comp = operation.target.GetComponent<name24>();
            if (name24Comp) name24Comp.texture = operation.texture;
        }
    }
}

[System.Serializable]
public class UndoOperation
{
    public GameObject target;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public Color color;
    public string texture;

    public bool isDeleted;
    public bool isCreated;

    public UndoOperation(GameObject target)
    {
        this.target = target;
        if (target != null)
        {
            position = target.transform.position;
            rotation = target.transform.localEulerAngles;
            scale = target.transform.localScale;
            var renderer = target.GetComponent<Renderer>();
            color = renderer ? renderer.material.color : Color.white;
            var name24Comp = target.GetComponent<name24>();
            texture = name24Comp ? name24Comp.texture : "";
        }
    }
}