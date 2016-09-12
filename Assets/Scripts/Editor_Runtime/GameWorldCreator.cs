using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameWorldCreator : MonoBehaviour {
    public Text m_MousePos;

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
    public float unitWorldSize = 1;
    private Vector2 originWorldPos = Vector2.zero;
    /// <summary>
    /// x 外层字典的Key, y 为内层字典的key, 内层字典的value 为 x,y的单元状态
    /// </summary>
    private Dictionary<int, Dictionary<int, WorldUnit>> worldMap = new Dictionary<int, Dictionary<int, WorldUnit>>();

    // 当前鼠标所在单元相对于世界（0，0）位置的偏移单元数
    int offsetX = 0;
    int offsetY = 0;

    #endregion


    void Awake()
    {
        this.mainCam = Camera.main;
        this.mainCamTrans = this.mainCam.transform;
    }


    void Update()
    {
        // 鼠标的时实位置
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        int offsetX = Mathf.RoundToInt((worldPos.x / unitWorldSize));
        int offsetY = Mathf.RoundToInt((worldPos.y / unitWorldSize));

        m_MousePos.text = "X: " + offsetX + "    Y: " + offsetY;

        #region 世界创造相关
        // 创造一个世界单元
        if (Input.GetMouseButton(0))
        {
            worldPos.x = offsetX * unitWorldSize;
            worldPos.y = offsetY * unitWorldSize;
            worldPos.z = 0;

            if(!worldMap.ContainsKey(offsetX) || !worldMap[offsetX].ContainsKey(offsetY))
            {
                if (!worldMap.ContainsKey(offsetX))
                {
                    worldMap.Add(offsetX, new Dictionary<int, WorldUnit>());
                }

                WorldUnit unit = new WorldUnit(offsetX,offsetY,1);

                worldMap[offsetX].Add(offsetY, unit);

                GameObject obj = Instantiate(Resources.Load("unit")) as GameObject;
                obj.transform.position = worldPos;
                unit.asset = obj;
            }
        }

        // 销毁一个世界单元
        if (Input.GetMouseButton(1))
        {
            if(worldMap.ContainsKey(offsetX) && worldMap[offsetX].ContainsKey(offsetY))
            {
                WorldUnit unit = worldMap[offsetX][offsetY];
                worldMap[offsetX].Remove(offsetY);
                GameObject.Destroy(unit.asset);
            }

        }

        #endregion 世界创造代码结束


        #region 摄相机控制相关
        // 摄相机大小控制，通过滚轮
        if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Camera.main.orthographicSize += Time.deltaTime * scrollSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minCameraSize, maxCameraSize);
        }

        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Camera.main.orthographicSize -= Time.deltaTime * scrollSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minCameraSize, maxCameraSize);
        }


        // 用键盘控制摄相机位置控制
        if (Input.GetKey(KeyCode.A))
        {
            Vector3 cameraPos = Camera.main.transform.position;
            cameraPos.x -= Time.deltaTime * moveSpeed;
            Camera.main.transform.position = cameraPos;
        }

        if (Input.GetKey(KeyCode.D))
        {
            Vector3 cameraPos = Camera.main.transform.position;
            cameraPos.x += Time.deltaTime * moveSpeed;
            Camera.main.transform.position = cameraPos;
        }

        if (Input.GetKey(KeyCode.W))
        {
            Vector3 cameraPos = Camera.main.transform.position;
            cameraPos.y += Time.deltaTime * moveSpeed;
            Camera.main.transform.position = cameraPos;
        }

        if (Input.GetKey(KeyCode.S))
        {
            Vector3 cameraPos = Camera.main.transform.position;
            cameraPos.y -= Time.deltaTime * moveSpeed;
            Camera.main.transform.position = cameraPos;
        }


        // 下面是用鼠标中键控制摄相机移动
        // 摄相机准备移动
        if (Input.GetMouseButtonDown(2))
        {
            // 按下中键时鼠标的世界坐标
            this.prevMousePos = Input.mousePosition;
        }


        // 通过鼠标中键左右移动摄相机
        if (Input.GetMouseButton(2))
        {
            // 当前的鼠标世界坐标
            this.currMousePos = Input.mousePosition;
            Vector3 offset = this.currMousePos - this.prevMousePos;
            if(Mathf.Abs(offset.x) > Mathf.Abs(offset.y))
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
        if(!Input.GetMouseButton(2) && Input.GetKeyDown(KeyCode.F))
        {
            Vector3 camPos = Camera.main.transform.position;
            camPos.x = 0;
            camPos.y = 0;
            Camera.main.transform.position = camPos;
        }


        // 下面是鼠标超出摄相机范围时自动移动

        if (Input.GetKey(KeyCode.V))
        {
            if (Input.mousePosition.x >= Screen.width)
            {
                Camera.main.transform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);
            }

            if (Input.mousePosition.x <= 0)
            {
                this.mainCamTrans.position -= new Vector3(moveSpeed * Time.deltaTime, 0, 0);
            }

            if (Input.mousePosition.y >= Screen.height)
            {
                this.mainCamTrans.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
            }

            if (Input.mousePosition.y <= 0)
            {
                this.mainCamTrans.position -= new Vector3(0, moveSpeed * Time.deltaTime, 0);
            }
        }
        #endregion 摄相机控制代码结束



    }

}
