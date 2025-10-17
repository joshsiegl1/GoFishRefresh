#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion
public interface ISelectable
{
    bool IsSelected { get; set; }
    bool IsHighlighted { get; set; }
    void Select();
    void Deselect();
    void UpdateSelection(MouseState MS, GraphicsDeviceManager graphics); 
}