using Eloi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DemoListenToStudentsMono : MonoBehaviour
{
    [Multiline(5)]
    public string m_manual="" +
        "Name:Command\n" +
        "Left, Right, -x , +x \n" +
        "Down, Up, -y , +y\n" +
        "Back, Forward, -z, +z";


    [Header("Target")]
    public Cursor65Mono m_target;

    [Header("Players")]    
    public List<NameToCursor> m_players = new List<NameToCursor>();
    public float m_costByBytes = 0.00001f;

    [Header("Event")]
    public UnityEvent<string> m_onMessageReceived;
    public UnityEvent<string> m_onPlayersPointsAsString;
    public UnityEvent<string> m_onTargetPositionAsString;


    [Header("Anti-spam")]
    public float m_timeBetweenCommands = 0.1f;





    [Header("Debug")]
    public string m_lastReceivedMessage;
    public string m_name;
    public string m_command;
    public string m_parametre;


    private void Start()
    {
        ResetRandomTargetPosition();
    }

    public void ResetRandomTargetPosition() { 
        m_target.Cursor.Randomize();
        m_target.RefreshIfPossible();
    }

    public void Update()
    {
        if (OnePlayerAtTargetPosition()) { 
            ResetRandomTargetPosition();
        }


        int x = (int) m_target.Cursor.MeterX;
        int y = (int) m_target.Cursor.MeterY;
        int z = (int) m_target.Cursor.MeterZ;
        m_onTargetPositionAsString.Invoke("Target: "+x + "|" + y + "|" + z);

        string playerPointsState = "";
        foreach (NameToCursor player in m_players)
        {
            playerPointsState += player.m_name + " : " + player.m_point + " points\n";
        }
        m_onPlayersPointsAsString.Invoke(playerPointsState);
    }



 
 
 
 
 
 

 
    private bool OnePlayerAtTargetPosition()
    {

        int m_targetX;
        int m_targetY;
        int m_targetZ;
        int m_playerX;
        int m_playerY;
        int m_playerZ;

        float m_distance = 0;

        foreach (NameToCursor player in m_players)
        {

            m_targetX = (int)m_target.Cursor.MeterX;
            m_targetY = (int)m_target.Cursor.MeterY;
            m_targetZ = (int)m_target.Cursor.MeterZ;
            m_playerX = (int)player.m_cursor.Cursor.MeterX;
            m_playerY = (int)player.m_cursor.Cursor.MeterY;
            m_playerZ = (int)player.m_cursor.Cursor.MeterZ;

             m_distance = Mathf.Sqrt(
                (m_targetX - m_playerX) * (m_targetX - m_playerX) +
                (m_targetY - m_playerY) * (m_targetY - m_playerY) +
                (m_targetZ - m_playerZ) * (m_targetZ - m_playerZ)
                );


            if (m_distance<3f

                ) {

                player.m_point++;
                return true;
            
            }
        }
        return false;
    }

    [System.Serializable]
    public class NameToCursor {
        public string m_name;
        public Cursor65Mono m_cursor;
        public string m_lastMessage;
        public int m_messageCount;
        public long m_bytesCount;
        public double m_cost;
        public List<string> m_waitingCommands= new List<string>();
        public int m_point;
    }

    public void PushInManager(string message)
    {
        m_lastReceivedMessage = message;
        m_onMessageReceived.Invoke(message);

        string[] tokens = message.Split(':');
        if (tokens.Length > 1) { 
        
            string name = tokens[0];
            string command = tokens.Length >= 2? tokens[1]:"";
            string parametre = tokens.Length >= 3 ? tokens[2] : ""; 
            m_name = name;
            m_command = command;
            m_parametre = parametre;

            foreach(NameToCursor player in m_players)
            {
                if (player.m_name.ToLower() == name.ToLower())
                {
                   // player.m_cursor.Cursor.Randomize();
                    player.m_cursor.gameObject.name = "Cursor:"+name;
                    player.m_cursor.RefreshIfPossible();
                    player.m_lastMessage = message;
                    player.m_messageCount++;
                    player.m_bytesCount += message.Length;
                    player.m_cost = player.m_bytesCount* m_costByBytes;
                    player.m_waitingCommands.Insert(0,command);
                }
            }


        }
    }

    private void OnEnable()
    {
        StartCoroutine(PushCommandIfPresent());
    }

    private IEnumerator PushCommandIfPresent()
    {
        while (true) {

            yield return new WaitForSeconds(m_timeBetweenCommands);
            yield return new WaitForEndOfFrame();

            foreach (NameToCursor player in m_players)
            {
                if (player.m_waitingCommands.Count > 0)
                {
                    string command = player.m_waitingCommands[player.m_waitingCommands.Count - 1];
                    player.m_waitingCommands.RemoveAt(player.m_waitingCommands.Count - 1);
                    command = command.ToLower();
                    if (command == "up" || command == "+y")
                        player.m_cursor.Cursor.MeterY += 1;
                    if (command == "down" || command == "-y")
                        player.m_cursor.Cursor.MeterY -= 1;
                    if (command == "left"|| command == "-x")
                        player.m_cursor.Cursor.MeterX -= 1;
                    if (command == "right" || command == "+x")
                        player.m_cursor.Cursor.MeterX += 1;
                    if (command == "forward" || command == "+z")
                        player.m_cursor.Cursor.MeterZ += 1;
                    if (command == "back" || command=="-z")
                        player.m_cursor.Cursor.MeterZ -= 1;
                    if (command == "reset" || command == "zero" || command == "0")
                    {
                        player.m_cursor.Cursor.MeterX = 0;
                        player.m_cursor.Cursor.MeterY = 0;
                        player.m_cursor.Cursor.MeterZ = 0;
                    }

                }
            }
        }

    }
}
