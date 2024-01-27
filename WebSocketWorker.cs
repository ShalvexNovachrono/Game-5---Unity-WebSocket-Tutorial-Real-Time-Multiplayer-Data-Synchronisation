using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WebSocketSharp;

public class WebSocketWorker : MonoBehaviour
{
    public string url;
    public string id;
    WebSocket ws;



    public GameObject playerNPC;


    [System.Serializable]
    public class Object
    {
        public string id;
        public string type;
        public string pos;
        public float rotationY;
        public int delete = 0;
    }


    [System.Serializable]
    public class Object_Array
    {
        public Object[] Object_Details;
        public static object Object { get; internal set; }
    }

    public Object_Array Object_Array_ = new Object_Array();


    private void Start()
    {
        id = GenerateRandomID();
        ws = new WebSocket(url);
        ws.Connect();
        ws.OnMessage += (sender, e) =>
        {
            // Debug.Log(e.Data);
            try
            {
                Object_Array_ = JsonUtility.FromJson<Object_Array>(e.Data);
            } catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            RemoveObjectsToDelete();
            NPCupdate();
        };
    }
    // crash file %USERPROFILE%\AppData\LocalLow\DefaultCompany\Game 5\Player.log
    public void SendPlayerData(Transform player)
    {

            Debug.Log("Attempting to send player data...");

            if (id != "" && ws != null && ws.ReadyState == WebSocketState.Open)
            {
                String data = "{" +
                    "\"id\": \"" + id + "\", " +
                    "\"type\": \"player\", " +
                    "\"pos\": \"" + player.position.x.ToString() + "," + player.position.y.ToString() + "," + player.position.z.ToString() + "\", " +
                    "\"rotationY\": " + player.rotation.eulerAngles.y.ToString() + ", " +
                    "\"delete\": 0" +
                    "}";
                SendDataToServer(data);
                Debug.Log("Player data sent successfully.");
            }
            else
            {
                Debug.LogWarning("WebSocket connection is not open. Unable to send player data.");
            }
        }

        public string GenerateRandomID()
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        int idLength = 8; // You can adjust the length of the ID as needed
        char[] randomChars = new char[idLength];

        for (int i = 0; i < idLength; i++)
        {
            randomChars[i] = chars[UnityEngine.Random.Range(0, chars.Length)];
        }

        return new string(randomChars);
    }

    public void NPCupdate()
    {
        foreach (var obj in Object_Array_.Object_Details)
        {
            if (obj.id != id)
            {
                GameObject findThisNPCplayerBody = GameObject.Find(obj.id);
                if (findThisNPCplayerBody != null)
                {
                    Transform NPC = findThisNPCplayerBody.transform;
                    string POS = obj.pos;
                    string[] POSsplit = POS.Split(',');
                    Vector3 pos = new Vector3(float.Parse(POSsplit[0]), float.Parse(POSsplit[1]), float.Parse(POSsplit[2]));
                    NPC.position = pos;
                    NPC.transform.rotation = Quaternion.Euler(0, obj.rotationY, 0);

                }
                else
                {
                    string POS = obj.pos;
                    string[] POSsplit = POS.Split(',');
                    Vector3 pos = new Vector3(float.Parse(POSsplit[0]), float.Parse(POSsplit[1]), float.Parse(POSsplit[2]));
                    CreateNewNPCplayer(obj.id, pos, obj.rotationY);
                }
            }
        }
    }

    void CreateNewNPCplayer(string newId, Vector3 pos, float rotationY)
    {
        GameObject newChild = Instantiate(playerNPC, transform.position, Quaternion.identity);

        newChild.name = newId;
        newChild.transform.position = pos;
        newChild.transform.rotation = Quaternion.Euler(0, rotationY, 0);
        //newChild.AddComponent<NPC>();
        //newChild.GetComponent<NPC>().id = newChild.transform.GetComponentInChildren<TMP_Text>();
        //newChild.GetComponent<NPC>().id.text = newId;
        Debug.Log("Created a new child with id: " + newId);
    }


    void RemoveObjectsToDelete()
    {
        // Create a new list to store objects without delete == 1
        var updatedObjects = new List<Object>();

        // Iterate through the existing objects
        foreach (var obj in Object_Array_.Object_Details)
        {
            // Check if delete == 1, skip if true
            if (obj.delete == 1)
            {
                GameObject findThisNPCplayerBody = GameObject.Find(obj.id);
                if (findThisNPCplayerBody != null)
                {
                    Destroy(findThisNPCplayerBody);
                }
                continue;
            }

            // If delete != 1, add the object to the updated list
            updatedObjects.Add(obj);
        }

        // Update the Object_Array with the filtered list
        Object_Array_.Object_Details = updatedObjects.ToArray();
    }

    void SendDataToServer(string DataThatWillBeSent)
    {
        if (id != "" && ws != null && ws.ReadyState == WebSocketState.Open)
        {
            ws.Send(DataThatWillBeSent);
        }
        else
        {
            Debug.LogWarning("WebSocket connection is not open. Unable to send data.");
        }
    }

}