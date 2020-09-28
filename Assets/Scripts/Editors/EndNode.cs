using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DN.VN
{
    public class EndNode : StoryNode
    {
        public EndNode(Vector2 position, float width, float height, Action<ConnectionPoint> OnClickInPoint) : base(position, width, height, OnClickInPoint, null)
        {

        }

        public override void Draw()
        {
            inPoint.Draw();

            linesCount = 2;
            linesCountCurrent = 0;
            rect.height = linesCount * entryHeigth;
            GUI.Box(rect, title);

            GUI.BeginGroup(rect);
            GUI.Label(NewRect(), "End");
            GUI.EndGroup();
        }

        public override JSONTemplate GetJSONObject()
        {
            JSONTemplate json = new JSONTemplate();
            json.AddKeyValue("type", "end");
            json.AddKeyValue("posX", this.GetPosition().x);
            json.AddKeyValue("posY", this.GetPosition().y);
            return json;
        }
    }
}