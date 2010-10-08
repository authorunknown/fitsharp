﻿// Copyright © 2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Text;
using fitSharp.Machine.Model;
using fitSharp.Parser;
using NUnit.Framework;

namespace fitSharp.Test.NUnit.Parser {
    [TestFixture] public class TextTablesTest {
        [Test] public void ParsesEmptyInputAsEmptyTree() {
            Assert.AreEqual(0, Parse(string.Empty).Branches.Count);
        }

        [Test] public void ParsesWordAsCell() {
            AssertParse("stuff", " <p> <table><tr> <td> stuff</td> </tr></table></p>");
        }

        [Test] public void GeneratesTableForLineStartingWithSeparator() {
            AssertParse("\n|stuff", " <table class=\"fit_table\"> <tr> <td> stuff</td> </tr></table>");
        }

        [Test] public void EncodesCellContent() {
            AssertParse("<<stuff", " <p> <table><tr> <td> &lt;&lt;stuff</td> </tr></table></p>");
        }

        [Test] public void IgnoresLeadingNewlines() {
            AssertParse("\n\n\nstuff", " <p> <table><tr> <td> stuff</td> </tr></table></p>");
        }

        [Test] public void ParsesWordsAsSeparateCells() {
            AssertParse("more|stuff",
                " <p> <table><tr> <td> more</td>  <td> stuff</td> </tr></table></p>");
        }

        [Test] public void ParsesLinesAsSeparateRows() {
            AssertParse("more 'good'\nstuff",
                " <p> <table><tr> <td> more</td>  <td> good</td> </tr></table> <table><tr> <td> stuff</td> </tr></table></p>");
        }

        [Test] public void ParsesNestedTable() {
            AssertParse("one [\n two\n] three", " <p> <table><tr> <td> one</td>  <td> <p> <table><tr> <td> two</td> </tr></table></p></td>  <td> three</td> </tr></table></p>");
        }

        [Test] public void ParsesBlankLinesAsSeparateTables() {
            AssertParse("more\ngood\n\nstuff",
                " <p> <table><tr> <td> more</td> </tr></table> <table><tr> <td> good</td> </tr></table></p> <p> <table><tr> <td> stuff</td> </tr></table></p>");
        }

        [Test] public void IncludesLeaderInFirstTable() {
            Assert.AreEqual(
                " more test@ <p> <table><tr> <td> stuff</td> </tr></table></p>",
                Format(ParseRaw("more test@stuff")));
        }

        [Test] public void IncludesTrailerInLastTable() {
            Assert.AreEqual(
                " test@ <p> <table><tr> <td> stuff</td> </tr></table></p> @test",
                Format(ParseRaw("test@stuff@test")));
        }

        [Test] public void IncludesLeaderInSecondTable() {
            Assert.AreEqual(
                " test@ <p> <table><tr> <td> stuff</td> </tr></table></p> @test and test@ <p> <table><tr> <td> more</td> </tr></table></p>",
                Format(ParseRaw("test@stuff@test and test@more")));
        }

        static void AssertParse(string input, string expected) {
            Assert.AreEqual(
                " test@" + expected,
                Format(Parse(input)));
        }

        static Tree<CellBase> Parse(string input) {
            return ParseRaw("test@" + input);
        }

        static Tree<CellBase> ParseRaw(string input) {
            return new TextTables(new TextTableScanner(input, c => c.IsLetter)).Parse();
        }

        static string Format(Tree<CellBase> tree) {
            return Format(tree, " ");
        }

        static string Format(Tree<CellBase> tree, string separator) {
            var result = new StringBuilder();
            if (!string.IsNullOrEmpty(tree.Value.GetAttribute(CellAttribute.Leader)))
                result.AppendFormat("{0}{1}", tree.Value.GetAttribute(CellAttribute.Leader), separator);
            result.Append(tree.Value.GetAttribute(CellAttribute.StartTag));
            foreach (Tree<CellBase> branch in tree.Branches)
                result.AppendFormat("{0}{1}", separator, Format(branch, separator));
            if (!string.IsNullOrEmpty(tree.Value.GetAttribute(CellAttribute.Body)))
                result.AppendFormat("{0}{1}", separator, tree.Value.GetAttribute(CellAttribute.Body));
            result.Append(tree.Value.GetAttribute(CellAttribute.EndTag));
            if (!string.IsNullOrEmpty(tree.Value.GetAttribute(CellAttribute.Trailer)))
                result.AppendFormat("{0}{1}", separator, tree.Value.GetAttribute(CellAttribute.Trailer));
            return result.ToString();
        }
    }
}
