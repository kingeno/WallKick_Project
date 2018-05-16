using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class MyPlayerManager : MonoBehaviour {

    private GUIStyle guiStyle = new GUIStyle();

    public GameObject playerPrefab;

    const int maxPlayers = 2;

    List<Vector2> playerPositions = new List<Vector2>() {
            new Vector2( -8f, 2.6f),
            new Vector2( 8f, 2.6f),
        };

    public static List<PlayerController> players = new List<PlayerController>(maxPlayers);



    void Start()
    {
        InputManager.OnDeviceDetached += OnDeviceDetached;

        guiStyle.normal.textColor = Color.white;
    }


    void Update()
    {
        var inputDevice = InputManager.ActiveDevice;

        if (JoinButtonWasPressedOnDevice(inputDevice))
        {
            if (ThereIsNoPlayerUsingDevice(inputDevice))
            {
                CreatePlayer(inputDevice);
            }
        }
    }


    bool JoinButtonWasPressedOnDevice(InputDevice inputDevice)
    {
        return inputDevice.Action1.WasPressed || inputDevice.Action2.WasPressed || inputDevice.Action3.WasPressed || inputDevice.Action4.WasPressed || Input.GetKeyDown(KeyCode.Space);
    }


    PlayerController FindPlayerUsingDevice(InputDevice inputDevice)
    {
        var playerCount = players.Count;
        for (var i = 0; i < playerCount; i++)
        {
            var player = players[i];
            if (player.Device == inputDevice)
            {
                return player;
            }
        }

        return null;
    }


    bool ThereIsNoPlayerUsingDevice(InputDevice inputDevice)
    {
        return FindPlayerUsingDevice(inputDevice) == null;
    }


    void OnDeviceDetached(InputDevice inputDevice)
    {
        var player = FindPlayerUsingDevice(inputDevice);
        if (player != null)
        {
            RemovePlayer(player);
        }
    }


    PlayerController CreatePlayer(InputDevice inputDevice)
    {
        if (players.Count < maxPlayers)
        {
            // Pop a position off the list. We'll add it back if the player is removed.
            var playerPosition = playerPositions[0];
            playerPositions.RemoveAt(0);

            var gameObject = (GameObject)Instantiate(playerPrefab, playerPosition, Quaternion.identity);
            if (players.Count == 0)
            {
                gameObject.tag = "Player1";
                gameObject.name = "Player1";
            }
            else
            {
                gameObject.tag = "Player2";
                gameObject.name = "Player2";
            }

            var player = gameObject.GetComponent<PlayerController>();
            player.Device = inputDevice;
            players.Add(player);

            return player;
        }

        return null;
    }


    void RemovePlayer(PlayerController player)
    {
        playerPositions.Insert(0, player.transform.position);
        players.Remove(player);
        player.Device = null;
        Destroy(player.gameObject);
    }


    //void OnGUI()
    //{
    //    guiStyle.fontSize = 20;
    //    const float h = 22.0f;
    //    var y = 10.0f;

    //    GUI.Label(new Rect(10, y, 300, y + h), "Active players: " + players.Count + "/" + maxPlayers, guiStyle);
    //    y += h;

    //    if (players.Count < maxPlayers)
    //    {
    //        GUI.Label(new Rect(10, y, 300, y + h), "Press any button to join", guiStyle);
    //        y += h;
    //    }
    //}
}
