// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "I don't care to document things that have far better documentation elsewhere.", Scope = "namespaceanddescendants", Target = "~N:ServerApp.NativeAPI")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "Windows API doesn't follow this rule.", Scope = "namespaceanddescendants", Target = "~N:ServerApp.NativeAPI")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:Field names should not begin with underscore", Justification = "Windows API doesn't follow this rule.", Scope = "namespaceanddescendants", Target = "~N:ServerApp.NativeAPI")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Windows API doesn't follow this rule.", Scope = "namespaceanddescendants", Target = "~N:ServerApp.NativeAPI")]
