using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField]
    private Button refreshButton;
    [SerializeField]
    private Button buildButton;

    [SerializeField]
    private Canvas menuUI;
    [SerializeField]
    private GameObject roomButtonPrefab;

    private List<Button> rooms = new List<Button>();

    private int buildRoomPort = -1;

    // Start is called before the first frame update
    void Start()
    {
        setConfig();
        initButtons();  
        RefreshRoomList();
    }

    private void OnApplicationQuit()
    {
        if (buildRoomPort != -1) // 是房主，退出时移除房间
        {
            RemoveRoom();
        }
    }

    private void setConfig()
    {
        var args = System.Environment.GetCommandLineArgs();

        // 这里不要写在同一个循环里，因为这两个操作有先后顺序的关系，应先指定端口号
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-port")
            {
                ushort port = ushort.Parse(args[i + 1]);
                var transport = GetComponent<UnityTransport>();
                transport.ConnectionData.Port = port;
            }
        }

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-launch-as-server")
            {
                NetworkManager.Singleton.StartServer();
                DestoryAllButtons();
            }
        }
    }

    private void initButtons()
    {
        refreshButton.onClick.AddListener(() =>
        {
            RefreshRoomList();
        });
        buildButton.onClick.AddListener(() =>
        {
            BuildRoom();
        });
    }

    private void RefreshRoomList()
    {
        StartCoroutine(RefreshRoomListRequest("http://47.120.2.107:8080/fpsapp/get_room_list/"));
    }

    IEnumerator RefreshRoomListRequest(string uri)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.ConnectionError)
        {
            var resp = JsonUtility.FromJson<GetRoomListResponse>(uwr.downloadHandler.text);

            foreach (var room in rooms)
            {
                room.onClick.RemoveAllListeners();
                Destroy(room.gameObject);
            }
            rooms.Clear();

            int k = 0;
            foreach (var room in resp.rooms)
            {
                GameObject buttonObj = Instantiate(roomButtonPrefab, menuUI.transform);
                buttonObj.transform.localPosition = new Vector3(-3, 57 - k * 59, 0);
                Button button = buttonObj.GetComponent<Button>();
                button.GetComponentInChildren<TextMeshProUGUI>().text = room.name;
                button.onClick.AddListener(() =>
                {
                    var transport = GetComponent<UnityTransport>();
                    transport.ConnectionData.Port = (ushort)room.port;
                    NetworkManager.Singleton.StartClient();
                    DestoryAllButtons();
                });
                rooms.Add(button);
                k++;
            }
        }
    }

    private void BuildRoom()
    {
        StartCoroutine(BuildRoomRequest("http://47.120.2.107:8080/fpsapp/build_room/"));
    }

    IEnumerator BuildRoomRequest(string uri)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.ConnectionError)
        {
            var resp = JsonUtility.FromJson<BuildRoomResponse>(uwr.downloadHandler.text);
            if (resp.error_message == "success")
            {
                var transport = GetComponent<UnityTransport>();
                transport.ConnectionData.Port = (ushort)resp.port;
                buildRoomPort = resp.port;
                NetworkManager.Singleton.StartClient();
                DestoryAllButtons();
            }
        }
    }

    private void RemoveRoom()
    {
        StartCoroutine(RemoveRoomRequest("http://47.120.2.107:8080/fpsapp/remove_room/?port=" + buildRoomPort.ToString()));
    }

    IEnumerator RemoveRoomRequest(string uri)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.ConnectionError)
        {
            var resp = JsonUtility.FromJson<RemoveRoomResponse>(uwr.downloadHandler.text);
            
            if (resp.error_message == "success")
            {

            }
        }
    }

    // 删除所有按钮
    private void DestoryAllButtons()
    {
        refreshButton.onClick.RemoveAllListeners();
        buildButton.onClick.RemoveAllListeners();
        Destroy(refreshButton.gameObject);
        Destroy(buildButton.gameObject);

        foreach (var room in rooms)
        {
            room.onClick.RemoveAllListeners();
            Destroy(room.gameObject);
        }
    }
}