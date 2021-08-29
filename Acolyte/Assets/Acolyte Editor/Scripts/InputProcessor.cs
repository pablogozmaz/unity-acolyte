using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Acolyte.Editor
{
    public class InputProcessor : MonoBehaviour
    {
        public static string Clipboard
        {
            get
            {
                return GUIUtility.systemCopyBuffer;
            }
            set
            {
                GUIUtility.systemCopyBuffer = value;
            }
        }

        protected void KeyPressed(Event evt)
        {
            var currentEventModifiers = evt.modifiers;
            bool ctrl = SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX ? (currentEventModifiers & EventModifiers.Command) != 0 : (currentEventModifiers & EventModifiers.Control) != 0;
            bool shift = (currentEventModifiers & EventModifiers.Shift) != 0;
            bool alt = (currentEventModifiers & EventModifiers.Alt) != 0;
            bool ctrlOnly = ctrl && !alt && !shift;

            switch(evt.keyCode)
            {
                case KeyCode.Backspace:
                {
                    Backspace();
                    break;
                }

                case KeyCode.Delete:
                {
                    DeleteKey();
                    break;
                }

                case KeyCode.Home:
                {
                    MoveToStartOfLine(shift, ctrl);
                    break;
                }

                case KeyCode.End:
                {
                    MoveToEndOfLine(shift, ctrl);
                    break;
                }

                // Select All
                case KeyCode.A:
                {
                    if(ctrlOnly)
                    {
                        SelectAll();
                        break;
                    }
                    break;
                }

                // Copy
                case KeyCode.C:
                {
                    if(ctrlOnly)
                    {
                        Clipboard = GetSelectedString();
                        break;
                    }
                    break;
                }

                // Paste
                case KeyCode.V:
                {
                    if(ctrlOnly)
                    {
                        Append(Clipboard);
                        break;
                    }
                    break;
                }

                // Cut
                case KeyCode.X:
                {
                    if(ctrlOnly)
                    {
                        Clipboard = GetSelectedString();
                        Delete();
                        UpdateTouchKeyboardFromEditChanges();
                        SendOnValueChangedAndUpdateLabel();
                        break;
                    }
                    break;
                }

                case KeyCode.LeftArrow:
                {
                    MoveLeft(shift, ctrl);
                    break;
                }

                case KeyCode.RightArrow:
                {
                    MoveRight(shift, ctrl);
                    break;
                }

                case KeyCode.UpArrow:
                {
                    MoveUp(shift);
                    break;
                }

                case KeyCode.DownArrow:
                {
                    MoveDown(shift);
                    break;
                }

                case KeyCode.PageUp:
                {
                    MovePageUp(shift);
                    break;
                }

                case KeyCode.PageDown:
                {
                    MovePageDown(shift);
                    break;
                }

                // Submit
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                {
                    // Add a new line 
                    break;
                }

                case KeyCode.Escape:
                {
                    //m_ReleaseSelection = true;
                    // m_WasCanceled = true;
                    break;
                }
            }

            char c = evt.character;

            // Convert carriage return and end-of-text characters to newline.
            if(c == '\r' || (int)c == 3)
                c = '\n';

            // Convert Shift Enter to Vertical tab
            if(shift && c == '\n')
                c = '\v';

            if(IsValidChar(c))
            {
                Append(c);
            }

            if(c == 0)
            {
                /*
                if(compositionLength > 0)
                {
                    UpdateLabel();
                }*/
            }
        }

        private void MovePageDown(bool shift)
        {
            throw new NotImplementedException();
        }

        private void MovePageUp(bool shift)
        {
            throw new NotImplementedException();
        }

        private void MoveDown(bool shift)
        {
            throw new NotImplementedException();
        }

        private void MoveRight(bool shift, bool ctrl)
        {
            throw new NotImplementedException();
        }

        private void MoveUp(bool shift)
        {
            throw new NotImplementedException();
        }

        private void MoveLeft(bool shift, bool ctrl)
        {
            throw new NotImplementedException();
        }

        private void SendOnValueChangedAndUpdateLabel()
        {
            throw new NotImplementedException();
        }

        private void UpdateTouchKeyboardFromEditChanges()
        {
            throw new NotImplementedException();
        }

        private void Delete()
        {
            throw new NotImplementedException();
        }

        private void Append(string str)
        {
            throw new NotImplementedException();
        }

        private void Append(char c)
        {

        }

        private string GetSelectedString()
        {
            throw new NotImplementedException();
        }

        private void SelectAll()
        {
            throw new NotImplementedException();
        }

        private void MoveToEndOfLine(bool shift, bool ctrl)
        {
            throw new NotImplementedException();
        }

        private void MoveToStartOfLine(bool shift, bool ctrl)
        {
            throw new NotImplementedException();
        }

        private void DeleteKey()
        {
            throw new NotImplementedException();
        }

        private void Backspace()
        {
            throw new NotImplementedException();
        }

        protected virtual bool IsValidChar(char c)
        {
            // Null character
            if(c == 0)
                return false;

            // Delete key on mac
            if(c == 127)
                return false;

            // Accept newline and tab
            if(c == '\t' || c == '\n')
                return true;

            return true;
        }
    }
}