using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Text.RegularExpressions;

[CreateAssetMenu(fileName = "CodeDomTest", menuName = "Misc/CodeDomTest")]
public class CodeDomTest : ScriptableObject
{
    //https://learn.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/how-to-create-an-xml-documentation-file-using-codedom?redirectedfrom=MSDN
    #region Microsoft Code

    // Man, this stuff makes so little sense to me
    // What am I looking at?
    // Hmm though, looking at some of the things this thing is having to do
    // It has to delete a file
    // And it also generates a .exe for some reason
    // Does it have to do these things?

    // And then this method, BuildHelloWorldGraph
    // The graph building process, yeah I don't get it.
    // Hmm, no okay, that doesn't even matter though does it? No it doesn't.
    // What I need is an example that is getting the comments from something
    // This is seemingly just manually creating comments with hard-coded strings

    // Okay, yeh, no
    // I think this example is just showing how you can use CodeDom to create an XML file

    // It outputed this xml and an .exe and a microsoft access file or something
    /*
<doc>
    <assembly>
        <name>HelloWorld</name>
    </assembly>
    <members>
        <member name="T:Samples.Class1">
             <summary>
             Create a Hello World application.
             </summary>
             <seealso cref="M:Samples.Class1.Main" />
            </member>
        <member name="M:Samples.Class1.Main">
             <summary>
             Main method for HelloWorld application.
             <para>Add a new paragraph to the description.</para>
             </summary>
            </member>
    </members>
</doc>
    */
    static string providerName = "cs";
    static string sourceFileName = "test.cs";
    public void MicrosoftReadMeExample()
    {
        CodeDomProvider provider =
            CodeDomProvider.CreateProvider(providerName);

        Debug.Log("Building CodeDOM graph...");

        CodeCompileUnit cu = new CodeCompileUnit();

        cu = BuildHelloWorldGraph();

        StreamWriter sourceFile = new StreamWriter(sourceFileName);
        provider.GenerateCodeFromCompileUnit(cu, sourceFile, null);
        sourceFile.Close();

        CompilerParameters opt = new CompilerParameters(new string[]{
                                      "System.dll" });
        opt.GenerateExecutable = true;
        opt.OutputAssembly = "HelloWorld.exe";
        opt.TreatWarningsAsErrors = true;
        opt.IncludeDebugInformation = true;
        opt.GenerateInMemory = true;
        opt.CompilerOptions = "/doc:HelloWorldDoc.xml";

        CompilerResults results;

        Debug.Log("Compiling with " + providerName);
        results = provider.CompileAssemblyFromFile(opt, sourceFileName);

        OutputResults(results);
        if (results.NativeCompilerReturnValue != 0)
        {
            Debug.Log("");
            Debug.Log("Compilation failed.");
        }
        else
        {
            Debug.Log("");
            Debug.Log("Demo completed successfully.");
        }
        File.Delete(sourceFileName);
    }

    // Build a Hello World program graph using
    // System.CodeDom types.
    public static CodeCompileUnit BuildHelloWorldGraph()
    {
        // Create a new CodeCompileUnit to contain
        // the program graph.
        CodeCompileUnit compileUnit = new CodeCompileUnit();

        // Declare a new namespace called Samples.
        CodeNamespace samples = new CodeNamespace("Samples");
        // Add the new namespace to the compile unit.
        compileUnit.Namespaces.Add(samples);

        // Add the new namespace import for the System namespace.
        samples.Imports.Add(new CodeNamespaceImport("System"));

        // Declare a new type called Class1.
        CodeTypeDeclaration class1 = new CodeTypeDeclaration("Class1");

        class1.Comments.Add(new CodeCommentStatement("<summary>", true));
        class1.Comments.Add(new CodeCommentStatement(
            "Create a Hello World application.", true));
        class1.Comments.Add(new CodeCommentStatement("</summary>", true));
        class1.Comments.Add(new CodeCommentStatement(
            @"<seealso cref=" + '"' + "Class1.Main" + '"' + "/>", true));

        // Add the new type to the namespace type collection.
        samples.Types.Add(class1);

        // Declare a new code entry point method.
        CodeEntryPointMethod start = new CodeEntryPointMethod();
        start.Comments.Add(new CodeCommentStatement("<summary>", true));
        start.Comments.Add(new CodeCommentStatement(
            "Main method for HelloWorld application.", true));
        start.Comments.Add(new CodeCommentStatement(
            @"<para>Add a new paragraph to the description.</para>", true));
        start.Comments.Add(new CodeCommentStatement("</summary>", true));

        // Create a type reference for the System.Console class.
        CodeTypeReferenceExpression csSystemConsoleType =
            new CodeTypeReferenceExpression("System.Console");

        // Build a Console.WriteLine statement.
        CodeMethodInvokeExpression cs1 = new CodeMethodInvokeExpression(
            csSystemConsoleType, "WriteLine",
            new CodePrimitiveExpression("Hello World!"));

        // Add the WriteLine call to the statement collection.
        start.Statements.Add(cs1);

        // Build another Console.WriteLine statement.
        CodeMethodInvokeExpression cs2 = new CodeMethodInvokeExpression(
            csSystemConsoleType, "WriteLine", new CodePrimitiveExpression(
            "Press the ENTER key to continue."));

        // Add the WriteLine call to the statement collection.
        start.Statements.Add(cs2);

        // Build a call to System.Console.ReadLine.
        CodeMethodInvokeExpression csReadLine =
            new CodeMethodInvokeExpression(csSystemConsoleType, "ReadLine");

        // Add the ReadLine statement.
        start.Statements.Add(csReadLine);

        // Add the code entry point method to
        // the Members collection of the type.
        class1.Members.Add(start);

        return compileUnit;
    }


    static void OutputResults(CompilerResults results)
    {
        Debug.Log("NativeCompilerReturnValue=" +
            results.NativeCompilerReturnValue.ToString());
        foreach (string s in results.Output)
        {
            Debug.Log(s);
        }
    }
    #endregion
}

