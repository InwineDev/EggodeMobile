using Mirror;
using Mirror.Experimental;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class name24 : NetworkBehaviour
{
    public string name244;
    public bool isLomatel;
    public bool sus = true;

    public LootTable lootTable;

    [SyncVar(hook = nameof(all_sync))]
    public string all;

    [SyncVar(hook = nameof(vzyato_syns))]
    public bool vzyato;

    [SyncVar(hook = nameof(texture_syns))]
    public string texture;

    [SyncVar]
    public SyncList<byte> bytesForTexture = new SyncList<byte>();

    [SyncVar]
    public int wid;

    [SyncVar]
    public int hei;

    [SyncVar(hook = nameof(textureTile_syns))]
    public string textureTile = "1";

    [SyncVar(hook = nameof(isRigidbody_syns))]
    public bool isRigidbody = true;

    [SyncVar(hook = nameof(isCollider_syns))]
    public bool isCollider;

    [SyncVar(hook = nameof(sugoma_syns))]
    public Color sugoma224;

    [SyncVar(hook = nameof(idController))]
    public string id;

    [SerializeField] private GameObject _Oblomki;

    [SyncVar(hook = nameof(DestroyObject))]
    public int _hp = 100;

    private void all_sync(string oldv, string newv)
    {
            string[] parts = newv.Split('^');
        if (parts.Length >= 7)
        {
            texture = parts[5];
            isCollider = bool.Parse(parts[4]);
            if (UnityEngine.ColorUtility.TryParseHtmlString(parts[1], out Color color))
            {
                sugoma224 = color;
            }
            else
            {
                // Обработка ошибки парсинга цвета
                Console.WriteLine("Error parsing color");
            }
            id = parts[0];
            isLomatel = bool.Parse(parts[2]);
            
            isRigidbody = bool.Parse(parts[3]);
        }
    }

    private void DestroyObject(int oldv, int newv)
    {
        if (newv <= 0 & _Oblomki != null & gameObject.tag == "object")
        {
            GameObject sus = Instantiate(_Oblomki, gameObject.transform.position, gameObject.transform.rotation);
            sus.transform.localScale = gameObject.transform.localScale;
            NetworkServer.Spawn(sus);
            /*            if (lootTable)
                        {
                            foreach (var item in loot())
                            {
                                GameObject itemSpawned = Instantiate(item, gameObject.transform.position, gameObject.transform.rotation);
                                NetworkServer.Spawn(itemSpawned);
                            }
                        }*/
            NetworkServer.Destroy(gameObject);
        }
        if (newv <= 0 & gameObject.tag == "object")
        {
            if (lootTable)
            {
                foreach (var item in loot())
                {
                    GameObject itemSpawned = Instantiate(item, gameObject.transform.position, gameObject.transform.rotation);
                    NetworkServer.Spawn(itemSpawned);
                }
            }
            NetworkServer.Destroy(gameObject);
        }
    }

    public List<GameObject> loot()
    {
        int howMany = UnityEngine.Random.Range(lootTable.minLoot, lootTable.maxLoot);
        List<GameObject> prefabs = new List<GameObject>();
        for (int i = 0; i < howMany; i++)
        {
            float totalWeight = 0f;
            foreach (var item in lootTable.loot)
            {
                totalWeight += item.chance;
            }

            // Генерируем случайное число в диапазоне от 0 до общего веса
            float randomValue = UnityEngine.Random.Range(0f, totalWeight);
            float currentWeight = 0f;

            // Проходим по списку и выбираем предмет, в диапазон которого попадает randomValue
            foreach (var item in lootTable.loot)
            {
                currentWeight += item.chance;
                if (randomValue <= currentWeight)
                {
                    prefabs.Add(item.lootPrefab);
                }
            }
        }
        return prefabs;
    }


    private void Start()
    {
        sus = true;
    }

    private void idController(string oldv, string newv)
    {
        if (gameObject.GetComponent<txtController>())
        {
            gameObject.GetComponent<txtController>().sus();
        }
    }

    private void sugoma_syns(Color oldv, Color newv)
    {
        if (gameObject.GetComponent<Renderer>())
        {
            gameObject.GetComponent<Renderer>().material.color = newv;
        }
    }

    private void isCollider_syns(bool oldv, bool newv)
    {
        gameObject.GetComponent<Collider>().isTrigger = newv;
    }

    private void isRigidbody_syns(bool oldv, bool newv)
    {
        if (newv == true && SceneManager.GetActiveScene().buildIndex == 2)
        {
            gameObject.AddComponent<Rigidbody>();
            NetworkRigidbodyReliable nrb = gameObject.AddComponent<NetworkRigidbodyReliable>();
            nrb.target = transform;
            nrb.enabled = true;
        }
        else
        {
            Destroy(gameObject.GetComponent<NetworkRigidbodyReliable>());
            Destroy(gameObject.GetComponent<NetworkRigidbodyUnreliable>());
            Destroy(gameObject.GetComponent<NetworkRigidbody>());
        }
    }

    private void vzyato_syns(bool oldName, bool newName)
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = newName;
    }

    private void texture_syns(string oldv, string newv)
    {
        sus = false;
        print(sus);
        if (newv == null || newv == "")
        {
            sus = true;
            print(sus);
        }

        StartCoroutine(LoadTexture(newv));
    }

    private void textureTile_syns(string oldv, string newv)
    {
        if (textureTile != "")
        {
            try
            {
                gameObject.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(int.Parse(textureTile), int.Parse(textureTile));
            }
            catch
            {

            }
        }
    }

        public IEnumerator dsuhusao(string texture2)
    {
        yield return new WaitForSeconds(0.1f);
        name24[] pensil = FindObjectsOfType<name24>();
        int sugoma24chasachellenge = 0;
        foreach (name24 sus in pensil)
        {
            if (sus.texture == texture && sus.sus == true)
            {
                gameObject.GetComponent<Renderer>().material.mainTexture = sus.texture22;
            }
            else
            {
                sugoma24chasachellenge++;
            }
        }
    }
    public Material myImage;
    public Sprite ms;
    public Texture2D texture22;

    private static Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();
    private const int MAX_RETRY_ATTEMPTS = 3;
    private const float RETRY_DELAY = 1f;

    public IEnumerator LoadTexture(string textureUrl)
    {
        Texture2D missingTextureMaterial = Resources.Load<Texture2D>("Texture/missingtexture228");
        GetComponent<Renderer>().material.mainTexture = missingTextureMaterial;

        // Если текстура уже в кеше, используем ее
        if (textureCache.TryGetValue(textureUrl, out Texture2D cachedTexture))
        {
            ApplyTexture(cachedTexture);
            yield break;
        }

        // Специальная обработка для eggodetexture
        if (textureUrl.Contains("eggodetexture//") && bytesForTexture != null)
        {
            Texture2D tex = new Texture2D(wid, hei, TextureFormat.ARGB32, false);
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Repeat;
            tex.LoadImage(bytesForTexture.ToArray());
            tex.Apply();

            CacheAndApplyTexture(textureUrl, tex);
            yield break;
        }

        // Пытаемся загрузить текстуру с повторами при ошибках
        yield return LoadTextureWithRetry(textureUrl, MAX_RETRY_ATTEMPTS);
    }

    private IEnumerator LoadTextureWithRetry(string textureUrl, int remainingAttempts)
    {
        using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(textureUrl))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(req);
                texture.filterMode = FilterMode.Point;
                texture.wrapMode = TextureWrapMode.Repeat;
                CacheAndApplyTexture(textureUrl, texture);
            }
            else
            {
                Debug.LogWarning($"Failed to load texture (attempts left: {remainingAttempts}): {req.error}");

                if (remainingAttempts > 0)
                {
                    yield return new WaitForSeconds(RETRY_DELAY);
                    yield return LoadTextureWithRetry(textureUrl, remainingAttempts - 1);
                }
                else
                {
                    Debug.LogError($"Final attempt failed for texture: {textureUrl}");
                }
            }
        }
    }

    private void CacheAndApplyTexture(string url, Texture2D texture)
    {
        // Кешируем текстуру
        if (!textureCache.ContainsKey(url))
        {
            textureCache.Add(url, texture);
        }

        // Применяем текстуру
        ApplyTexture(texture);
    }

    private void ApplyTexture(Texture2D texture)
    {
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.Apply(false);

        GetComponent<Renderer>().material.mainTexture = texture;
        GetComponent<Renderer>().material.mainTexture.filterMode = FilterMode.Point; 

        if (!string.IsNullOrEmpty(textureTile))
        {
            int tileValue = int.Parse(textureTile);
            GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(tileValue, tileValue);
        }
    }
}
