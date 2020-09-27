using System;
using UnityEngine;

namespace DN.VN
{
    public enum ConnectionPointType { In, Out }

    public class ConnectionPoint
    {
        public Rect rect;

        public ConnectionPointType type;

        public StoryNode node;

        public Action<ConnectionPoint> OnClickConnectionPoint;

        public ConnectionPoint(StoryNode node, ConnectionPointType type, Action<ConnectionPoint> OnClickConnectionPoint)
        {
            this.node = node;
            this.type = type;
            this.OnClickConnectionPoint = OnClickConnectionPoint;
            rect = new Rect(0, 0, 20f, 20f);
        }

        public void Draw()
        {
            switch (type)
            {
                case ConnectionPointType.In:
                    rect.x = node.rect.x - rect.width + 8f;
                    rect.y = node.rect.y;
                    break;
                case ConnectionPointType.Out:
                    rect.x = node.rect.x + node.rect.width - 8f;
                    rect.y = node.rect.y + (node.rect.height * 0.5f);
                    break;
            }

            if (GUI.Button(rect, ""))
            {
                if (OnClickConnectionPoint != null)
                {
                    OnClickConnectionPoint(this);
                }
            }
        }
    }
}