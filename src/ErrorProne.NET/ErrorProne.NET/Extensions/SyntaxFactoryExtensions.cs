﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ErrorProne.NET.Extensions
{
    internal enum VisibilityModifier
    {
        Public,
        Internal,
        Protected,
        ProtectedInternal,
        Private
    }

    internal static class SyntaxFactoryExtensions
    {
        private static readonly SyntaxKind[] m_visibilityModifierKinds = new[]
        {
            SyntaxKind.InternalKeyword, SyntaxKind.PublicKeyword, SyntaxKind.ProtectedKeyword, SyntaxKind.PrivateKeyword
        };

        private static IEnumerable<SyntaxToken> GetModifiersFor(VisibilityModifier modifier)
        {
            switch (modifier)
            {
                case VisibilityModifier.Public:
                    return new[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword) };
                case VisibilityModifier.Internal:
                    return new[] { SyntaxFactory.Token(SyntaxKind.InternalKeyword) };
                case VisibilityModifier.Protected:
                    return new[] { SyntaxFactory.Token(SyntaxKind.ProtectedKeyword) };
                case VisibilityModifier.ProtectedInternal:
                    return new[] { SyntaxFactory.Token(SyntaxKind.ProtectedKeyword), SyntaxFactory.Token(SyntaxKind.InternalKeyword) };
                case VisibilityModifier.Private:
                    return new[] { SyntaxFactory.Token(SyntaxKind.PrivateKeyword) };
                default:
                    throw new ArgumentOutOfRangeException(nameof(modifier), modifier, $"Unknown modifier '{modifier}'.");
            }
        }

        public static MethodDeclarationSyntax WithVisibilityModifier(this MethodDeclarationSyntax methodDeclaration, VisibilityModifier modifier)
        {
            Contract.Requires(methodDeclaration != null);
            Contract.Ensures(Contract.Result<MethodDeclarationSyntax>() != null);

            var oldModifiers = methodDeclaration.Modifiers.Where(m => !m_visibilityModifierKinds.Contains(m.Kind()));

            var modifiers =
                new SyntaxTokenList()
                    .AddRange(GetModifiersFor(modifier))
                    .AddRange(oldModifiers);

            return methodDeclaration.WithModifiers(modifiers);
        }

        public static MethodDeclarationSyntax WithoutModifiers(this MethodDeclarationSyntax methodDeclaration, Func<SyntaxToken, bool> predicate)
        {
            Contract.Requires(methodDeclaration != null);
            Contract.Ensures(Contract.Result<MethodDeclarationSyntax>() != null);

            return methodDeclaration.WithModifiers(new SyntaxTokenList().AddRange(methodDeclaration.Modifiers.Where(x => !predicate(x))));
        }

        public static ConstructorDeclarationSyntax WithVisibilityModifier(this ConstructorDeclarationSyntax constructorDeclaration, VisibilityModifier modifier)
        {
            Contract.Requires(constructorDeclaration != null);
            Contract.Ensures(Contract.Result<ConstructorDeclarationSyntax>() != null);

            var oldModifiers = constructorDeclaration.Modifiers.Where(m => !m_visibilityModifierKinds.Contains(m.Kind()));
            var modifiers =
                new SyntaxTokenList()
                    .AddRange(GetModifiersFor(modifier))
                    .AddRange(oldModifiers);

            return constructorDeclaration.WithModifiers(modifiers);
        }

        public static TSyntax WithEndLineTrivia<TSyntax>(this TSyntax node, params SyntaxTrivia[] trivia)
            where TSyntax : SyntaxNode
        {
            return node.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);
        }

        public static BlockSyntax WithTriviaFromBlock(this BlockSyntax block, BlockSyntax other)
        {
            return
                block.WithOpenBraceToken(
                    SyntaxFactory.Token(SyntaxKind.OpenBraceToken).WithTriviaFrom(other.OpenBraceToken))
                .WithCloseBraceToken(
                    SyntaxFactory.Token(SyntaxKind.CloseBraceToken).WithTriviaFrom(other.CloseBraceToken));
        }
    }
}