using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DN.VN
{
    public class StartNode : StoryNode
    {
        public StartNode(Vector2 position, float width, float height, Action<ConnectionPoint> OnClickOutPoint) : base(position, width, height, null, OnClickOutPoint)
        {

        }

        public override void Draw()
        {
            inPoint.Draw();
            outPoint.Draw();

            linesCount = 2;
            linesCountCurrent = 0;
            rect.height = linesCount * entryHeigth;
            GUI.Box(rect, title);

            GUI.BeginGroup(rect);
            GUI.Label(NewRect(), "Start");
            GUI.EndGroup();
        }

        public override JSONTemplate GetJSONObject()
        {
            JSONTemplate json = new JSONTemplate();
            json.AddKeyValue("type", "start");
            json.AddKeyValue("posX", this.GetPosition().x);
            json.AddKeyValue("posY", this.GetPosition().y);
            return json;
        }
    }
}