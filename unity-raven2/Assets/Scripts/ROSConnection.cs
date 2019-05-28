﻿using UnityEngine;
using System;
using RosSharp.RosBridgeClient;
using std_msgs = RosSharp.RosBridgeClient.Messages.Standard;
using rosapi = RosSharp.RosBridgeClient.Services.RosApi;


public class ROSConnection : MonoBehaviour
{
    RosSocket rosSocket;
    string stringParamTempValue;

    private bool rosSocketIsAlive;

    // Start is called before the first frame update
    void Start()
    {
        // WebSocket Connection:
        rosSocket = new RosSocket(new RosSharp.RosBridgeClient.Protocols.WebSocketNetProtocol("ws://localhost:9090"));

        // Subscription:
        string subscription_id = rosSocket.Subscribe<std_msgs.String>("/subscription_test", SubscriptionHandler);
    }

    // Update is called once per frame
    void Update()
    {
        rosSocketIsAlive = rosSocket.protocol.IsAlive();

        // Publication:
        string publication_id = rosSocket.Advertise<std_msgs.String>("publication_test");
        std_msgs.String message = new std_msgs.String();
        message.data = "publication test message data";
        rosSocket.Publish(publication_id, message);
    }

    private static void SubscriptionHandler(std_msgs.String message)
    {
        Debug.Log((message).data);
    }

    public bool IsRosSocketAlive()
    {
        return rosSocketIsAlive;
    }

    public RosSocket getRosSocket()
    {
        return rosSocket;
    }

    public string getStringParam(string paramName)
    {
        rosSocket.CallService<rosapi.GetParamRequest, rosapi.GetParamResponse>(
            "/rosapi/get_param", 
            handleStringParam, 
            new rosapi.GetParamRequest(paramName, "default")
        );
        return stringParamTempValue;
    }

    private void handleStringParam(object serviceResponse)
    {
        stringParamTempValue = ((rosapi.GetParamResponse)serviceResponse).value;
    }

    void OnDestroy()
    {
        // Close ROS socket connection
        rosSocket.Close();
    }
    
}
