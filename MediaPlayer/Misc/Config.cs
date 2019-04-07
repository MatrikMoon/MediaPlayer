using MediaPlayer.SimpleJSON;
using System;
using System.IO;
using UnityEngine;

namespace MediaPlayer.Misc
{
    class Config
    {
        public static Vector3 Position { get; set; }
        public static Vector3 Rotation { get; set; }
        public static Vector3 Size { get; set; }
        public static Vector3 Scale { get; set; }
        public static Color Color { get; set; }

        private static string ConfigLocation = $"{Environment.CurrentDirectory}/UserData/MediaPanel.txt";

        public static void LoadConfig()
        {
            if (File.Exists(ConfigLocation))
            {
                JSONNode node = JSON.Parse(File.ReadAllText(ConfigLocation));
                Position = Vector3FromNode("Position", node);
                Rotation = Vector3FromNode("Rotation", node);
                Size = Vector3FromNode("Size", node);
                Scale = Vector3FromNode("Scale", node);
                Color = ColorFromNode("Color", node);
            }
            else
            {
                Position = new Vector3(0, 5f, 4.5f);
                Rotation = new Vector3(0, 0, 0);
                Size = new Vector2(500, 250);
                Scale = new Vector3(0.01f, 0.01f, 0.01f);
                Color = Color.white;
                SaveConfig();
            }
        }

        private static Color ColorFromNode(string colorName, JSONNode node)
        {
            float r = float.Parse(node[$"{colorName}-R"].Value);
            float g = float.Parse(node[$"{colorName}-G"].Value);
            float b = float.Parse(node[$"{colorName}-B"].Value);
            float a = float.Parse(node[$"{colorName}-A"].Value);
            return new Color(r, g, b, a);
        }

        private static Vector3 Vector3FromNode(string vectorName, JSONNode node)
        {
            float x = float.Parse(node[$"{vectorName}-X"].Value);
            float y = float.Parse(node[$"{vectorName}-Y"].Value);
            float z = float.Parse(node[$"{vectorName}-Z"].Value);
            return new Vector3(x, y, z);
        }

        private static void NodeFromColor(string colorName, Color color, JSONNode node)
        {
            node[$"{colorName}-R"] = color.r;
            node[$"{colorName}-G"] = color.g;
            node[$"{colorName}-B"] = color.b;
            node[$"{colorName}-A"] = color.a;
        }

        private static void NodeFromVector3(string vectorName, Vector3 vector, JSONNode node)
        {
            node[$"{vectorName}-X"] = vector.x;
            node[$"{vectorName}-Y"] = vector.y;
            node[$"{vectorName}-Z"] = vector.z;
        }

        public static void SaveConfig()
        {
            JSONNode node = new JSONObject();
            NodeFromVector3("Position", Position, node);
            NodeFromVector3("Rotation", Rotation, node);
            NodeFromVector3("Size", Size, node);
            NodeFromVector3("Scale", Scale, node);
            NodeFromColor("Color", Color, node);
            File.WriteAllText(ConfigLocation, node.ToString());
        }
    }
}
