using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DN.VN
{
    [System.Serializable]
    public class Node : PolymorphicObject
    {
        public enum NodeType
        {
            START = 0,
            DIALOGUE = 1,
            BRANCH = 2,
            CALLBACK = 3,
            END = 100
        }
        // Attributes
        public NodeType type;
        public int id;
        public float posX;
        public float posY;
        // End Attributs

        public Node(NodeType type)
        {
            this.type = type;
            this.id = -1;
            this.posX = StoryEditor.initialPosition.x;
            this.posY = StoryEditor.initialPosition.y;
        }

        public Node Pos(Vector2 pos)
        {
            this.posX = pos.x;
            this.posY = pos.y;
            return this;
        }

        public string ToJSON()
        {
            string str = "";
            str += "{" + Utility.Str2Key("type") + this.type + "}";
            return str;
        }
    }
}
