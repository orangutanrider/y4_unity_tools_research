using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Text;
//using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

[CreateAssetMenu(fileName = "CommentCompilerTest", menuName = "Misc/CommentCompilerTest")]
public class CommentCompiler : ScriptableObject
{
    public void TestButton()
    {
        //string[] filePaths = GetScriptPaths();
        //foreach (string filePath in filePaths)
        //{
        //    PrintAFileTest(StreamFile(filePath));
        //}

        string path = GetScriptPaths()[0];
        PrintClassSummary(File.ReadAllText(path));
    }

    string[] GetScriptPaths()
    {
        return Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Assets\\Scripts", "*.cs", SearchOption.AllDirectories);
    }

    FileStream StreamFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) == true)
        {
            Debug.LogError("Null or Empty file path");
            return null;
        }

        if (File.Exists(filePath) == false)
        {
            Debug.LogError("File doesn't exist");
            return null;
        }

        if (filePath.EndsWith(".cs") == false)
        {
            Debug.LogError("It isn't a .cs file");
            return null;
        }

        FileInfo fileInfo = new FileInfo(filePath);

        return fileInfo.OpenRead();
    }

    //https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream?view=net-7.0
    void PrintAFileTest(FileStream fileStream)
    {
        int readLength = (int)fileStream.Length;

        UTF8Encoding encoding = new UTF8Encoding(true);
        byte[] array = new byte[readLength];
        int readIndex;

        while ((readIndex = fileStream.Read(array, 0, readLength)) > 0)
        {
            Debug.Log(encoding.GetString(array, 0, readIndex));
        }
    }

    void PrintWordsTest(FileStream fileStream)
    {
        const int readLength = 1; // the while statement reads one character at a time

        UTF8Encoding encoding = new UTF8Encoding(true);
        byte[] array = new byte[readLength];
        int readIndex;

        string constructingWord = "";

        while ((readIndex = fileStream.Read(array, 0, readLength)) > 0)
        {
            char currentChar = encoding.GetString(array, 0, readIndex)[0];

            // something goes here for checking them

            constructingWord = constructingWord + currentChar;
        }
    }

    void PrintMembersTest(string programText)
    {
        // SOMETHING NEW
        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.csharpsyntaxwalker?view=roslyn-dotnet-4.6.0
        // https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/get-started/syntax-analysis

        SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);
        CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

        Debug.Log("programText: " + Environment.NewLine + Environment.NewLine + programText);

        Debug.Log("treeLength: " + tree.Length);

        foreach (MemberDeclarationSyntax member in root.Members)
        {
            Debug.Log(member.Kind());
        }

        foreach (SyntaxNode node in root.ChildNodes())
        {


            Debug.Log(node.Kind());

            foreach (SyntaxNode childNode in node.ChildNodes())
            {
                Debug.Log(childNode.Kind());

                if (childNode.Kind() == SyntaxKind.ClassDeclaration)
                {
                    foreach (SyntaxTrivia trivia in childNode.DescendantTrivia())
                    {
                        Debug.Log(trivia.Kind());
                        if (trivia.Kind() == SyntaxKind.SingleLineCommentTrivia)
                        {
                            Debug.Log("A!!!: " + CreateStringFromSpan(programText, trivia.Span));
                        }

                        if (trivia.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia)
                        {
                            Debug.Log("B!!!: " + CreateStringFromSpan(programText, trivia.FullSpan));
                        }

                        Debug.Log(trivia.Kind());
                        if (trivia.Kind() == SyntaxKind.MultiLineCommentTrivia)
                        {
                            Debug.Log("C!!!: " + CreateStringFromSpan(programText, trivia.Span));
                        }

                        if (trivia.Kind() == SyntaxKind.MultiLineDocumentationCommentTrivia)
                        {
                            Debug.Log("D!!!: " + CreateStringFromSpan(programText, trivia.FullSpan));
                        }
                    }
                }
            }
        }
    }

    void PrintTriviaTest(string programText)
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);
        CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

        foreach (SyntaxNode node in root.ChildNodes())
        {
            foreach (SyntaxNode childNode in node.ChildNodes())
            {
                List<SyntaxTrivia> triviaList = GetNodeTrivia(childNode);

                foreach(SyntaxTrivia trivia in triviaList)
                {
                    if(trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) == false) { continue; }
                    Debug.Log(CreateStringFromSpan(programText, trivia.FullSpan));
                }
            }
        }
    }

    string CreateStringFromSpan(string originalFullText, TextSpan textSpan)
    {
        string returnString = "";

        for (int loop = textSpan.Start; loop < textSpan.End; loop++)
        {
            returnString = returnString + originalFullText[loop];
        }

        return returnString;
    }

    List<SyntaxTrivia> GetNodeTrivia(SyntaxNode node)
    {
        List<SyntaxTrivia> returnList = new List<SyntaxTrivia>();

        for (int loop = node.FullSpan.Start; loop < node.FullSpan.End; loop++)
        {
            SyntaxTrivia trivia = node.FindTrivia(loop);
            if(trivia == null) { continue; }
            returnList.Add(trivia);
        }

        return returnList;
    }

    void CountAllClassDeclerations(string programText)
    {
        // open set, closed set
        // scan first node for all namespace and class declerations
        // add to open set
        // scan open set for the same, if nothing found add to closed set
        // if node is in the closed set, do not scan
        // return all documentation comments

        List<SyntaxNode> openSet = new List<SyntaxNode>();
        List<SyntaxNode> closedSet = new List<SyntaxNode>();

        List<SyntaxNode> classDeclerations = new List<SyntaxNode>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);
        CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
        openSet.Add(root);

        int current = 0;

        // SyntaxList
        // This thing probably does this but more effecient I'm guessing

        while (openSet.Count > 0)
        {
            if(closedSet.Contains(openSet[current]))
            {
                openSet.RemoveAt(current);
                continue;
            }

            foreach (SyntaxNode child in openSet[current].ChildNodes())
            {
                if (!child.IsKind(SyntaxKind.ClassDeclaration) && !child.IsKind(SyntaxKind.NamespaceDeclaration))
                {
                    closedSet.Add(child);
                    continue;
                }
                openSet.Add(child);
            }

            closedSet.Add(openSet[current]);
            if (openSet[current].IsKind(SyntaxKind.ClassDeclaration))
            {
                classDeclerations.Add(openSet[current]);
            }
            openSet.RemoveAt(current);
        }

        Debug.Log(programText);
        Debug.Log(classDeclerations.Count);

        foreach(SyntaxNode node in classDeclerations)
        {
            // (Span start doesn't correlate to program line, it correlates to character index)
            Debug.Log(node.SpanStart + " " + node.Kind());
        }
    }

    void PrintClassDocumentationComments(string programText)
    {
        #region CountAllClassDeclerations() copy pasted
        // look through node
        // if node is namespace or class
        // then descend one node

        // open set, closed set
        // scan first node for all namespace and class declerations
        // add to open set
        // scan open set for the same, if nothing found add to closed set
        // if node is in the closed set, do not scan
        // return all documentation comments

        List<SyntaxNode> openSet = new List<SyntaxNode>();
        List<SyntaxNode> closedSet = new List<SyntaxNode>();

        List<SyntaxNode> classDeclerations = new List<SyntaxNode>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);
        CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
        openSet.Add(root);

        const int current = 0;

        while (openSet.Count > 0)
        {
            if (closedSet.Contains(openSet[current]))
            {
                openSet.RemoveAt(current);
                continue;
            }

            foreach (SyntaxNode child in openSet[current].ChildNodes())
            {
                if (!child.IsKind(SyntaxKind.ClassDeclaration) && !child.IsKind(SyntaxKind.NamespaceDeclaration))
                {
                    closedSet.Add(child);
                    continue;
                }
                openSet.Add(child);
            }

            closedSet.Add(openSet[current]);
            if (openSet[current].IsKind(SyntaxKind.ClassDeclaration))
            {
                classDeclerations.Add(openSet[current]);
            }
            openSet.RemoveAt(current);
        }
        #endregion

        // when a class definition is found
        // scroll up until a trivia element matching of kind single line documentation comment is found
        // or if any non-trivia element is found before that, then stop looking

        // someting annoying with this
        // it should scroll up line by line
        // but I'm gonna have to do it character by character aren't I?
        // hmm

        foreach (SyntaxNode classDecleration in classDeclerations)
        {
            SyntaxTriviaList triviaList = classDecleration.GetLeadingTrivia();

            foreach (SyntaxTrivia trivia in triviaList)
            {
                if (!trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)) { continue; }
                Debug.Log(CreateStringFromSpan(programText, trivia.FullSpan));
            }
        }

        // okay never mind
        // i can just use this thing GetLeadingTrivia()
    }

    void PrintSinglePerClassDocumentationComments(string programText)
    {
        #region CountAllClassDeclerations() copy pasted
        // look through node
        // if node is namespace or class
        // then descend one node

        // open set, closed set
        // scan first node for all namespace and class declerations
        // add to open set
        // scan open set for the same, if nothing found add to closed set
        // if node is in the closed set, do not scan
        // return all documentation comments

        List<SyntaxNode> openSet = new List<SyntaxNode>();
        List<SyntaxNode> closedSet = new List<SyntaxNode>();

        List<SyntaxNode> classDeclerations = new List<SyntaxNode>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);
        CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
        openSet.Add(root);

        const int current = 0;

        while (openSet.Count > 0)
        {
            if (closedSet.Contains(openSet[current]))
            {
                openSet.RemoveAt(current);
                continue;
            }

            foreach (SyntaxNode child in openSet[current].ChildNodes())
            {
                if (!child.IsKind(SyntaxKind.ClassDeclaration) && !child.IsKind(SyntaxKind.NamespaceDeclaration))
                {
                    closedSet.Add(child);
                    continue;
                }
                openSet.Add(child);
            }

            closedSet.Add(openSet[current]);
            if (openSet[current].IsKind(SyntaxKind.ClassDeclaration))
            {
                classDeclerations.Add(openSet[current]);
            }
            openSet.RemoveAt(current);
        }
        #endregion

        foreach (SyntaxNode classDecleration in classDeclerations)
        {
            SyntaxTriviaList triviaList = classDecleration.GetLeadingTrivia();

            bool firstFound = false;

            for (int loop = triviaList.Count - 1; loop > -1; loop--)
            {
                SyntaxTrivia trivia = triviaList[loop];

                if (firstFound == true) { continue; }
                if (!trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)) { continue; }
                firstFound = true;
                Debug.Log(classDecleration.SpanStart.ToString() + " " + CreateStringFromSpan(programText, trivia.FullSpan));
            }
        }
    }

    void PrintClassDocumentationComment(string programText)
    {
        #region CountAllClassDeclerations() copy pasted
        // look through node
        // if node is namespace or class
        // then descend one node

        // open set, closed set
        // scan first node for all namespace and class declerations
        // add to open set
        // scan open set for the same, if nothing found add to closed set
        // if node is in the closed set, do not scan
        // return all documentation comments

        List<SyntaxNode> openSet = new List<SyntaxNode>();
        List<SyntaxNode> closedSet = new List<SyntaxNode>();

        List<SyntaxNode> classDeclerations = new List<SyntaxNode>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);
        CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
        openSet.Add(root);

        const int current = 0;

        while (openSet.Count > 0)
        {
            if (closedSet.Contains(openSet[current]))
            {
                openSet.RemoveAt(current);
                continue;
            }

            foreach (SyntaxNode child in openSet[current].ChildNodes())
            {
                if (!child.IsKind(SyntaxKind.ClassDeclaration) && !child.IsKind(SyntaxKind.NamespaceDeclaration))
                {
                    closedSet.Add(child);
                    continue;
                }
                openSet.Add(child);
            }

            closedSet.Add(openSet[current]);
            if (openSet[current].IsKind(SyntaxKind.ClassDeclaration))
            {
                classDeclerations.Add(openSet[current]);
            }
            openSet.RemoveAt(current);
        }
        #endregion

        List<string> classDocumentationComments = new List<string>();

        foreach (SyntaxNode classDecleration in classDeclerations)
        {
            SyntaxTriviaList triviaList = classDecleration.GetLeadingTrivia();

            bool firstFound = false;

            for (int loop = triviaList.Count - 1; loop > -1; loop--)
            {
                SyntaxTrivia trivia = triviaList[loop];

                if (firstFound == true) { continue; }
                if (!trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)) { continue; }
                firstFound = true;
                classDocumentationComments.Add(CreateStringFromSpan(programText, trivia.FullSpan));
            }
        }

        List<string> processedComments = new List<string>();

        foreach (string documentationComment in classDocumentationComments)
        {
            // https://stackoverflow.com/questions/1547476/split-a-string-on-newlines-in-net
            string[] lines = documentationComment.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            string processedComment = "";

            foreach (string line in lines)
            {
                // calculate amount of white space in indentation
                bool firstRealCharFound = false;
                int whiteSpaceCount = 0;
                foreach (char character in line)
                {
                    if(firstRealCharFound == true)
                    { continue; }

                    if (char.IsWhiteSpace(character))
                    {
                        whiteSpaceCount++;
                    }
                    else
                    {
                        firstRealCharFound = true;
                    }
                }

                Debug.Log(whiteSpaceCount);

                string processedLine = line;
                processedLine = processedLine.Remove(0, whiteSpaceCount); // remove indentation
                processedLine = processedLine.Remove(0, 3); // remove '///'

                // add to comment
                processedComment = processedComment + processedLine;
                processedComment = processedComment + Environment.NewLine;
            }

            Debug.Log(processedComment);
            processedComments.Add(processedComment);
        }
    }

    void PrintClassSummary(string programText)
    {
        #region CountAllClassDeclerations() copy pasted
        // look through node
        // if node is namespace or class
        // then descend one node

        // open set, closed set
        // scan first node for all namespace and class declerations
        // add to open set
        // scan open set for the same, if nothing found add to closed set
        // if node is in the closed set, do not scan
        // return all documentation comments

        List<SyntaxNode> openSet = new List<SyntaxNode>();
        List<SyntaxNode> closedSet = new List<SyntaxNode>();

        List<SyntaxNode> classDeclerations = new List<SyntaxNode>();

        SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);
        CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
        openSet.Add(root);

        const int current = 0;

        while (openSet.Count > 0)
        {
            if (closedSet.Contains(openSet[current]))
            {
                openSet.RemoveAt(current);
                continue;
            }

            foreach (SyntaxNode child in openSet[current].ChildNodes())
            {
                if (!child.IsKind(SyntaxKind.ClassDeclaration) && !child.IsKind(SyntaxKind.NamespaceDeclaration))
                {
                    closedSet.Add(child);
                    continue;
                }
                openSet.Add(child);
            }

            closedSet.Add(openSet[current]);
            if (openSet[current].IsKind(SyntaxKind.ClassDeclaration))
            {
                classDeclerations.Add(openSet[current]);
            }
            openSet.RemoveAt(current);
        }
        #endregion

        #region CommentProcessing copy and pasted
        List<string> classDocumentationComments = new List<string>();

        foreach (SyntaxNode classDecleration in classDeclerations)
        {
            SyntaxTriviaList triviaList = classDecleration.GetLeadingTrivia();

            bool firstFound = false;

            for (int loop = triviaList.Count - 1; loop > -1; loop--)
            {
                SyntaxTrivia trivia = triviaList[loop];

                if (firstFound == true) { continue; }
                if (!trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)) { continue; }
                firstFound = true;
                classDocumentationComments.Add(CreateStringFromSpan(programText, trivia.FullSpan));
            }
        }

        List<string> processedComments = new List<string>();

        foreach (string documentationComment in classDocumentationComments)
        {
            // https://stackoverflow.com/questions/1547476/split-a-string-on-newlines-in-net
            string[] lines = documentationComment.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            string processedComment = "";

            foreach (string line in lines)
            {
                // calculate amount of white space in indentation
                bool firstRealCharFound = false;
                int whiteSpaceCount = 0;
                foreach (char character in line)
                {
                    if (firstRealCharFound == true)
                    { continue; }

                    if (char.IsWhiteSpace(character))
                    {
                        whiteSpaceCount++;
                    }
                    else
                    {
                        firstRealCharFound = true;
                    }
                }

                string processedLine = line;
                processedLine = processedLine.Remove(0, whiteSpaceCount); // remove indentation
                processedLine = processedLine.Remove(0, 3); // remove '///'

                // add to comment
                processedComment = processedComment + processedLine;
                processedComment = processedComment + Environment.NewLine;
            }

            processedComments.Add(processedComment);
        }
        #endregion

        foreach (string comment in processedComments)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(comment);
            XmlNodeList summaryNodes = doc.GetElementsByTagName("summary");

            Debug.Log(summaryNodes.Count);

            foreach(XmlNode node in summaryNodes)
            {
                Debug.Log(node.InnerText);
            }
        }
    }
}