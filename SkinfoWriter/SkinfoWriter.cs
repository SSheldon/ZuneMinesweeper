using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

// TODO: replace this with the type you want to write out.
using TWrite = SkinfoWriter.Skinfo;

namespace SkinfoWriter
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class SkinfoWriter : ContentTypeWriter<TWrite>
    {
        protected override void Write(ContentWriter output, TWrite value)
        {
            // TODO: write the specified value to the output ContentWriter.
            output.Write(value.name);
            output.Write(value.creator);
            output.Write(value.itemsBacked);
            output.Write(value.headersBacked);
            output.Write(value.background);
            output.Write(value.coloredText);
            output.Write(value.uncoloredText);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            // TODO: change this to the name of your ContentTypeReader
            // class which will be used to load this data.
            return "Minesweeper.SkinfoReader, Minesweeper";
        }
    }
}
