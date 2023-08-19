using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField]
    private Button hostBtn;
    [SerializeField]
    private Button serverBtn;
    [SerializeField]
    private Button clientBtn;

    [SerializeField]
    private Button room1;
    [SerializeField]
    private Button room2;

    // Start is called before the first frame update
    void Start()
    {
        var args = System.Environment.GetCommandLineArgs(); 

        // ���ﲻҪд��ͬһ��ѭ�����Ϊ�������������Ⱥ�˳��Ĺ�ϵ��Ӧ��ָ���˿ں�
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
        
        room1.onClick.AddListener(() =>
        {
            var transport = GetComponent<UnityTransport>();
            transport.ConnectionData.Port = 7777;
            NetworkManager.Singleton.StartClient();
            DestoryAllButtons();
        });
        room2.onClick.AddListener(() =>
        {
            var transport = GetComponent<UnityTransport>();
            transport.ConnectionData.Port = 7778;
            NetworkManager.Singleton.StartClient();
            DestoryAllButtons();
        });

        
        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            DestoryAllButtons();
        });
        serverBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
            DestoryAllButtons();
        });
        clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            DestoryAllButtons();
        });

    }

    // ɾ�����а�ť
    private void DestoryAllButtons()
    {
        Destroy(hostBtn.gameObject);
        Destroy(serverBtn.gameObject);
        Destroy(clientBtn.gameObject);
        Destroy(room1.gameObject);
        Destroy(room2.gameObject);
    }
}