using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DN.VN
{
    [System.Serializable]
    public class DialogueNode : Node
    {
        public string name;
        public string dialogue;

        public DialogueNode() : base(Node.NodeType.DIALOGUE)
        {
            name = "";
            dialogue = "";
        }
    }
}