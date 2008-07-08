using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

// TODO: replace this with the type you want to read.
using TRead = Minesweeper.Skin;

namespace Minesweeper
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content
    /// Pipeline to read the specified data type from binary .xnb format.
    /// 
    /// Unlike the other Content Pipeline support classes, this should
    /// be a part of your main game project, and not the Content Pipeline
    /// Extension Library project.
    /// </summary>
    public class SkinfoReader : ContentTypeReader<TRead>
    {
        protected override TRead Read(ContentReader input, TRead existingInstance)
        {
            // TODO: read a value from the input ContentReader.
            string name = input.ReadString();
            string creator = input.ReadString();
            bool itemsBacked = input.ReadBoolean();
            bool headersBacked = input.ReadBoolean();
            Vector3 background = input.ReadVector3();
            Vector3 coloredText = input.ReadVector3();
            Vector3 uncoloredText = input.ReadVector3();
            return new TRead(name, creator, itemsBacked, headersBacked, background, coloredText, uncoloredText);
        }
    }
}
