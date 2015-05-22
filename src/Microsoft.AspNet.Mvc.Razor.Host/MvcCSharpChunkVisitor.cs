// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Razor.Generator;
using Microsoft.AspNet.Razor.Generator.Compiler;
using Microsoft.AspNet.Razor.Generator.Compiler.CSharp;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Mvc.Razor
{
    public abstract class MvcCSharpChunkVisitor : CodeVisitor<CSharpCodeWriter>
    {
        public MvcCSharpChunkVisitor([NotNull] CSharpCodeWriter writer,
                                     [NotNull] CodeBuilderContext context)
            : base(writer, context)
        {
        }

        public override void Accept(Chunk chunk)
        {
            if (chunk is Utf8Chunk)
            {
                Visit((Utf8Chunk)chunk);
            }
            if (chunk is InjectChunk)
            {
                Visit((InjectChunk)chunk);
            }
            else if (chunk is ModelChunk)
            {
                Visit((ModelChunk)chunk);
            }
            else
            {
                base.Accept(chunk);
            }
        }

        protected abstract void Visit(Utf8Chunk chunk);
        protected abstract void Visit(InjectChunk chunk);
        protected abstract void Visit(ModelChunk chunk);
    }
}