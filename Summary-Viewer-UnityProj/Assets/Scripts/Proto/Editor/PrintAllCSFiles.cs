using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;

public class PrintAllCSFiles : MonoBehaviour
{

    public void Start()
    {
        #region Code reading comments
        //System.Reflection.Emit
        //System.Runtime.CompilerServices

        // HMM
        // big issue im having rn
        // is that I essentially have to use the C# compilier for this to work
        // I have to use its logic and reverse engineer it kind of
        // but idk how to get the compiler, and i can't find any System stuff that does this for me

        // https://stackoverflow.com/questions/15602606/programmatically-get-summary-comments-at-runtime
        // hmm

        /*"
        If you have access to the .cs file you're trying to read, you can use CodeDOM to compile it and extract the XML doc comments. MSDN even has a howto that demonstrates some 
        the APIs: msdn.microsoft.com/en-us/library/ms404261.aspx (It goes the other way around though, by generating code at runtime, but it should be possible to somehow feed a
        CS file to CodeDOM.) – millimoose
        Mar 24, 2013 at 19:11 
         "*/

        // Okay, so it must be this System.CodeDom; Although this bit at the end 
        // "(It goes the other way around though, by generating code at runtime, but it should be possible to somehow feed a CS file to CodeDOM.)"
        // That makes me go hmm, hopefully they're correct

        // System.CodeDom isn't implemented in Unity or something
        // Hopefully I can import it

        //https://docs.unity3d.com/Manual/overview-of-dot-net-in-unity.html
        // Hmm?
        // "While Unity tries its best to support as much of the .NET ecosystem as possible, there are some exceptions to parts of the .NET system libraries that Unity explicitly doesn’t support."
        // I hope I can make this work regardless

        // OKAY NO IT DOES WORK
        // YES!!!!!!!
        // It's just that it has to be an editor script
        #endregion

        #region CodeDom Pre-Test
        // Okay, two options here
        // One is to use this CodeDom thing
        // The other is to just make a thing that will read these files and get what it needs

        // CodeDom doesn't seem to be made specifically for my purpose
        // It's hard to tell what it is honestly
        // Somethign to do with code generation

        // I think it is possible to use it for my purposes
        // But I don't understand it at all
        // And finding out how it works? Well I think that can be done, it'll just take a while.

        // As for manual
        // Well it's simple
        // It'll just be a case where I have to keep updating it for a while
        // To get all the bugs out of it
        // I also worry that it'll be super slow, not sure if it actually will be though

        // So what is better?
        // Bunkering down and just figuring out CodeDom?
        // Or just focusing on making progress and doing it manually?
        #endregion

        // Okay, I think I gotta just make a compiler then
        // Just straight up have this thing read the .cs files
        // And I program a bunch of logic for processing them

        // (And then I found out about the code analysis stuff from microsoft)
    }

    void PrintAllFileLengthsInLines()
    {
        string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Assets\\Scripts", "*.cs", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            FileInfo fileInfo = new FileInfo(file);

            string[] lines = File.ReadAllLines(file);

            Debug.Log(lines.Length + " " + fileInfo.Name);
        }
    }

    void PrintAllCsFileDirectories()
    {
        string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Assets\\Scripts", "*.cs", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            Debug.Log(file);
        }
        Debug.Log(files.Length);
    }
}
