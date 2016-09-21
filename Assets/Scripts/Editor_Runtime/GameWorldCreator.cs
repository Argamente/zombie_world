using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using System.IO;

public class GameWorldCreator : MonoBehaviour
{

    private static GameWorldCreator _instance = null;
    public static GameWorldCreator GetInstance()
    {
        return _instance;
    }



    #region UI Component

    public Text m_MousePos;
    public Button btn_Save;
    public Button btn_ClearAll;
    public Button btn_Load;

    #endregion



    #region 摄相机控制

    // 摄相机控制相关
    private float scrollSpeed = 100;
    private float maxCameraSize = 50.0f;
    private float minCameraSize = 5.0f;
    private float moveSpeed = 25f;

    // 用于鼠标控制摄相机移动
    private Vector3 prevMousePos = Vector3.zero;
    private Vector3 currMousePos = Vector3.zero;

    private Camera mainCam;
    private Transform mainCamTrans;

    #endregion


    #region 世界的创造

    // 世界创造相关
    private float unitWorldSize = 1;
    private Vector2 originWorldPos = Vector2.zero;
    /// <summary>
    /// x 外层字典的Key, y 为内层字典的key, 内层字典的value 为 x,y的单元状态
    /// </summary>
    public Dictionary<int, Dictionary<int, object>> worldMap = new Dictionary<int, Dictionary<int, object>> ();
    private Dictionary<int,Dictionary<int, GameObject>> worldAssetsMap = new Dictionary<int, Dictionary<int, GameObject>> ();

    // 当前鼠标所在单元相对于世界（0，0）位置的偏移单元数
    int offsetX = 0;
    int offsetY = 0;

    #endregion


    void Awake ()
    {
        _instance = this;

        this.mainCam = Camera.main;
        this.mainCamTrans = this.mainCam.transform;

        btn_Save.onClick.AddListener (this.OnSave);
        btn_ClearAll.onClick.AddListener (this.OnClearAll);
        btn_Load.onClick.AddListener (this.OnLoad);
    }


    void Update ()
    {
        // 如果鼠标放在了UI上，不要做任何操作
        if (EventSystem.current.IsPointerOverGameObject ())
        {
            return;
        }

        // 鼠标的时实位置
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint (mousePos);
        int offsetX = Mathf.RoundToInt ((worldPos.x / unitWorldSize));
        int offsetY = Mathf.RoundToInt ((worldPos.y / unitWorldSize));

        m_MousePos.text = "X: " + offsetX + "    Y: " + offsetY;

        #region 放僵尸
        if(Input.GetKeyDown(KeyCode.Z))
        {
            worldPos.x = offsetX * unitWorldSize;
            worldPos.y = offsetY * unitWorldSize;
            worldPos.z = 0;

            if (!worldMap.ContainsKey(offsetX) || !worldMap[offsetX].ContainsKey(offsetY))
            {

                // create world unit asset
                GameObject obj = Instantiate(Resources.Load("Zombies/Zombie001")) as GameObject;
                obj.transform.position = worldPos;
            }
            return;
        }

        #endregion



        #region 世界创造相关
        // 创造一个世界单元
        if (Input.GetMouseButton (0))
        {
            worldPos.x = offsetX * unitWorldSize;
            worldPos.y = offsetY * unitWorldSize;
            worldPos.z = 0;

            if (!worldMap.ContainsKey (offsetX) || !worldMap [offsetX].ContainsKey (offsetY))
            {
                if (!worldMap.ContainsKey (offsetX))
                {
                    // 创建存储节
                    worldMap.Add (offsetX, new Dictionary<int, object> ());
                    worldAssetsMap.Add (offsetX, new Dictionary<int,GameObject> ());
                }

                WorldUnit unit = new WorldUnit (offsetX, offsetY, 1);
                // save the world unit to world map
                worldMap [offsetX].Add (offsetY, unit);

                // create world unit asset
                GameObject obj = Instantiate (Resources.Load ("unit")) as GameObject;
                obj.transform.position = worldPos;

                // save the world unit asset to world assets map
                worldAssetsMap [offsetX].Add (offsetY, obj);
            }
        }

        // 销毁一个世界单元
        if (Input.GetMouseButton (1))
        {
            if (worldMap.ContainsKey (offsetX) && worldMap [offsetX].ContainsKey (offsetY))
            {
                // remove world unit from world map
                WorldUnit unit = (WorldUnit)worldMap [offsetX] [offsetY];
                worldMap [offsetX].Remove (offsetY);

                // remove world unit asset from world asset map
                GameObject obj = worldAssetsMap [offsetX] [offsetY];
                worldAssetsMap [offsetX].Remove (offsetY);

                // destroy world unit asset
                GameObject.Destroy (obj);
            }

        }

        #endregion 世界创造代码结束


        #region 摄相机控制相关
        // 摄相机大小控制，通过滚轮
        if (Input.GetAxis ("Mouse ScrollWheel") < 0)
        {
            Camera.main.orthographicSize += Time.deltaTime * scrollSpeed;
            Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize, minCameraSize, maxCameraSize);
        }

        if (Input.GetAxis ("Mouse ScrollWheel") > 0)
        {
            Camera.main.orthographicSize -= Time.deltaTime * scrollSpeed;
            Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize, minCameraSize, maxCameraSize);
        }


        // 用键盘控制摄相机位置控制
        if (Input.GetKey (KeyCode.A))
        {
            Vector3 cameraPos = Camera.main.transform.position;
            cameraPos.x -= Time.deltaTime * moveSpeed;
            Camera.main.transform.position = cameraPos;
        }

        if (Input.GetKey (KeyCode.D))
        {
            Vector3 cameraPos = Camera.main.transform.position;
            cameraPos.x += Time.deltaTime * moveSpeed;
            Camera.main.transform.position = cameraPos;
        }

        if (Input.GetKey (KeyCode.W))
        {
            Vector3 cameraPos = Camera.main.transform.position;
            cameraPos.y += Time.deltaTime * moveSpeed;
            Camera.main.transform.position = cameraPos;
        }

        if (Input.GetKey (KeyCode.S))
        {
            Vector3 cameraPos = Camera.main.transform.position;
            cameraPos.y -= Time.deltaTime * moveSpeed;
            Camera.main.transform.position = cameraPos;
        }


        // 下面是用鼠标中键控制摄相机移动
        // 摄相机准备移动
        if (Input.GetMouseButtonDown (2))
        {
            // 按下中键时鼠标的世界坐标
            this.prevMousePos = Input.mousePosition;
        }


        // 通过鼠标中键左右移动摄相机
        if (Input.GetMouseButton (2))
        {
            // 当前的鼠标世界坐标
            this.currMousePos = Input.mousePosition;
            Vector3 offset = this.currMousePos - this.prevMousePos;
            if (Mathf.Abs (offset.x) > Mathf.Abs (offset.y))
            {
                offset.y = 0;
            }
            else
            {
                offset.x = 0;
            }
            Vector3 dir = offset.normalized;
            Camera.main.transform.position += dir * moveSpeed * Time.deltaTime;
            this.prevMousePos = this.currMousePos;
        }

        // 在没有按下中键的情况下，按F键可以将摄相机位置重置为0，0
        if (!Input.GetMouseButton (2) && Input.GetKeyDown (KeyCode.F))
        {
            Vector3 camPos = Camera.main.transform.position;
            camPos.x = 0;
            camPos.y = 0;
            Camera.main.transform.position = camPos;
        }


        // 下面是鼠标超出摄相机范围时自动移动

        if (Input.GetKey (KeyCode.V))
        {
            if (Input.mousePosition.x >= Screen.width)
            {
                Camera.main.transform.position += new Vector3 (moveSpeed * Time.deltaTime, 0, 0);
            }

            if (Input.mousePosition.x <= 0)
            {
                this.mainCamTrans.position -= new Vector3 (moveSpeed * Time.deltaTime, 0, 0);
            }

            if (Input.mousePosition.y >= Screen.height)
            {
                this.mainCamTrans.position += new Vector3 (0, moveSpeed * Time.deltaTime, 0);
            }

            if (Input.mousePosition.y <= 0)
            {
                this.mainCamTrans.position -= new Vector3 (0, moveSpeed * Time.deltaTime, 0);
            }
        }
        #endregion 摄相机控制代码结束



    }


    public bool IsAvaliableOffset(int offsetX, int offsetY)
    {
        if (!worldMap.ContainsKey(offsetX) || !worldMap[offsetX].ContainsKey(offsetY))
        {
            return true;
        }
        return false;
    }


    // load world map from file
    private void OnLoad ()
    {
        string fileName = Application.streamingAssetsPath + "/worldmap.data";
        StreamReader reader = File.OpenText (fileName);
        string worldMapStr = reader.ReadToEnd ();
        reader.Close ();

        // first , clear current world map
        this.OnClearAll ();

        try
        {
            this.worldMap = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int,object>>> (worldMapStr);
        }
        catch (System.Exception e)
        {
            Debug.LogError ("Deserialize Object Error");
            return;
        }

        InitGameWorldFromWorldMap ();

    }

    private void OnLoad_WithList ()
    {
        
    }


    private void InitGameWorldFromWorldMap ()
    {
        foreach (KeyValuePair<int, Dictionary<int, object>> kv in worldMap)
        {
            foreach (KeyValuePair<int, object> subKv in kv.Value)
            {
                if (this.worldAssetsMap.ContainsKey (kv.Key) == false)
                {
                    this.worldAssetsMap.Add (kv.Key, new Dictionary<int, GameObject> ());
                }

                GameObject obj = Instantiate (Resources.Load ("unit")) as GameObject;
                int of_x = kv.Key;
                int of_y = subKv.Key;

                obj.transform.position = new Vector3 (of_x * unitWorldSize, of_y * unitWorldSize, 0);
                this.worldAssetsMap [of_x].Add (of_y, obj);
            }
        }
        
    }





    /// <summary>
    /// Save the world to json
    /// </summary>
    private void OnSave ()
    {

        string worldMapStr = JsonConvert.SerializeObject (worldMap);
        string fileName = Application.streamingAssetsPath + "/worldmap.data";
        StreamWriter writer = File.CreateText (fileName);
        writer.Write (worldMapStr);
        writer.Close ();
    }


    private void OnSave_WithList ()
    {
        List<WorldUnit> worldList = new List<WorldUnit> ();
        foreach (KeyValuePair<int,  Dictionary<int,object>> kv in worldMap)
        {
            foreach (KeyValuePair<int,object> unitKv in kv.Value)
            {
                WorldUnit unit = (WorldUnit)unitKv.Value;
                worldList.Add (unit);
            }
        }

        string worldMapStr = JsonConvert.SerializeObject (worldList);
        string fileName = Application.streamingAssetsPath + "/worldmaplist.data";
        StreamWriter writer = File.CreateText (fileName);
        writer.Write (worldMapStr);
        writer.Close ();
    }



    /// <summary>
    /// Clear all world unit
    /// </summary>
    private void OnClearAll ()
    {
        // first clear world map data
        foreach (KeyValuePair<int,Dictionary<int,object>> kv in worldMap)
        {
            kv.Value.Clear ();
        }

        worldMap.Clear ();


        // then clear world asset map and destroy all world unit asset
        foreach (KeyValuePair<int, Dictionary<int,GameObject>> kv in worldAssetsMap)
        {
            foreach (KeyValuePair<int,GameObject> subKv in kv.Value)
            {
                if (subKv.Value != null)
                {
                    GameObject.Destroy (subKv.Value);
                } 
            }

            kv.Value.Clear ();
        }

        worldAssetsMap.Clear ();
    }

}
