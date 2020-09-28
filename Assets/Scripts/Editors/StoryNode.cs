using System;
using UnityEditor;
using UnityEngine;

namespace DN.VN
{
    public class StoryNode
    {
        public Rect rect;
        public string title;

        public GUIStyle style;

        public const int leftIndent = 10;
        public const int topIndent = 10;
        public const int entryHeigth = 20;
        public const float inputRatio = 0.9f;
        public bool isDragged;

        protected int linesCount = 0;
        protected int linesCountCurrent = 0;

        public string nameInput = "";
        public string dialogueInput = "";

        public ConnectionPoint inPoint;
        public ConnectionPoint outPoint;

        public StoryNode(Vector2 position, float width, float height, Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint)
        {
            rect = new Rect(position.x, position.y, width, height);
            if (OnClickInPoint != null)
            {
                inPoint = new ConnectionPoint(this, ConnectionPointType.In, OnClickInPoint);
            }
            if (OnClickOutPoint != null)
            {
                outPoint = new ConnectionPoint(this, ConnectionPointType.Out, OnClickOutPoint);
            }
        }

        public void Drag(Vector2 delta)
        {
            rect.position += delta;
        }

        public Vector2 GetPosition()
        {
            return rect.position;
        }

        public virtual void Draw()
        {
            inPoint.Draw();
            outPoint.Draw();

            linesCount = 7;
            linesCountCurrent = 0;
            rect.height = linesCount * entryHeigth;
            GUI.Box(rect, title);
            GUI.BeginGroup(rect);

            GUI.Label(NewRect(), "Name");
            Rect nameRect = NewRect();
            nameRect.width *= inputRatio;
            nameInput = GUI.TextField(nameRect, nameInput, 100);

            GUI.Label(NewRect(), "Dialogue");
            Rect dialogueRect = NewRect();
            dialogueRect.width *= inputRatio;
            dialogueRect.height = entryHeigth * 3;
            AddEntry(2);
            dialogueInput = GUI.TextField(dialogueRect, dialogueInput, 100);

            GUI.EndGroup();
        }

        protected void AddEntry()
        {
            this.linesCountCurrent += 1;
        }

        protected void AddEntry(int lines)
        {
            this.linesCountCurrent += lines;
        }

        protected Rect NewRect()
        {
            AddEntry();
            return new Rect(new Vector2(leftIndent, topIndent + entryHeigth * (linesCountCurrent - 1)), new Vector2(rect.width, entryHeigth));
        }

        public bool ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (rect.Contains(e.mousePosition))
                        {
                            isDragged = true;
                            GUI.changed = true;
                        }
                        else
                        {
                            GUI.changed = true;
                        }
                    }
                    break;

                case EventType.MouseUp:
                    isDragged = false;
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && isDragged)
                    {
                        Drag(e.delta);
                        e.Use();
                        return true;
                    }
                    break;
                case EventType.KeyDown:

                    break;
            }

            return false;
        }

        public virtual JSONTemplate GetJSONObject()
        {
            JSONTemplate json = new JSONTemplate();
            json.AddKeyValue("type", "dialogue");
            json.AddKeyValue("name", this.nameInput);
            json.AddKeyValue("dialogue", this.dialogueInput);
            json.AddKeyValue("posX", this.GetPosition().x);
            json.AddKeyValue("posY", this.GetPosition().y);
            return json;
        }
    }
}
