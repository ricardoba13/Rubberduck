﻿using System;
using Rubberduck.Parsing.VBA;
using Rubberduck.VBEditor;

namespace Rubberduck.Parsing.Rewriter
{
    public class AttributesRewriteSession : RewriteSessionBase
    {
        private readonly IParseManager _parseManager;

        public AttributesRewriteSession(IParseManager parseManager, IRewriterProvider rewriterProvider,
            Func<IRewriteSession, bool> rewritingAllowed)
            : base(rewriterProvider, rewritingAllowed)
        {
            _parseManager = parseManager;
        }

        protected override IExecutableModuleRewriter ModuleRewriter(QualifiedModuleName module)
        {
            return RewriterProvider.AttributesModuleRewriter(module);
        }

        protected override void RewriteInternal()
        {
            //The suspension ensures that only one parse gets executed instead of two for each rewritten module.
            var result = _parseManager.OnSuspendParser(this, new[] {ParserState.Ready}, ExecuteAllRewriters);
            if(result != SuspensionResult.Completed)
            {
                Logger.Warn($"Rewriting attribute modules did not succeed. suspension result = {result}");
            }
        }

        private void ExecuteAllRewriters()
        {
            foreach (var rewriter in CheckedOutModuleRewriters.Values)
            {
                rewriter.Rewrite();
            }
        }
    }
}