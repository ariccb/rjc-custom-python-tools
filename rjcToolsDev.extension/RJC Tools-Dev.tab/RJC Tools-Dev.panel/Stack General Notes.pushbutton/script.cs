using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;


/*
Author: Aric Crosson Bouwers
Date: 2019 October 24th

Thanks to Ali Tehami, Woods Bagot, Melbourne, Australia. for sharing this code open source.
Thanks to Joshua Lumley for his great tips that enabled this proof of concept in PyRevit.
Link to Joshua's youtube video explaning the method:
https://youtu.be/KHMwd4U_Lrs
more on Joshua's video from Jermey Tammik:
https://thebuildingcoder.typepad.com/blog/2018/09/five-secrets-of-revit-api-coding.html
 */

namespace PyRevitInvokeTesting
{
   [Autodesk.Revit.Attributes.Transaction (Autodesk.Revit.Attributes.TransactionMode.Manual)]
   public class invokingTest : IExternalCommand
   {

      //PyRevit MetaData
      public const string __title__ = "Stack\nGeneral Notes";
      public const string __doc__ = "Re-organize General Notes automatically by giving the extents of the sheet boundary";
      public const string __author__ = "PyRevit Implementation by: Aric Crosson Bouwers\nStack General Notes C# Code written by: Chris Febbraro\n Original PyRevit Implementation code by Ali Tehami \n+ \nOriginal code by Joshua Lumley";
      // Original script made by Ali Tehami
      public const string __helpurl__ = @"https://github.com/alitehami/pyRevitBetaIdeas_Public/tree/master/aliTehami.extension/BetaConcepts.tab/invoking%20Assemblies.panel/invoke.pushbutton";
      public const string __min_revit_ver__ = "2016";
      public const string __max_revit_ver__ = "2019";
      public const bool __beta__ = false;

      public Result Execute (ExternalCommandData commandData, ref string message, ElementSet elements)
      {
         //path to the assembly. (can be automated if there is a way to access pyrevit's 'command_path' (so assemblies can live under a lib\ folder with script.cs)
         string path = @"C:\Users\acrossonbouwers\Documents\GitHub\rjc-development\rjcCustomPythonTools\rjcToolsDev.extension\RJC Tools-Dev.tab\RJC Tools-Dev.panel\Stack General Notes.pushbutton\";
         String exeConfigPath = Path.GetDirectoryName (path)  + "\\Assembly\\GeneralNotesAutomation.dll";
         String exeConfigPath2 = Path.GetDirectoryName (path) + "\\Assembly";

         //name of the class for the command to Execute
         string strCommandName = "StackGeneralNotes";

         byte[ ] assemblyBytes = File.ReadAllBytes (exeConfigPath);
         Assembly objAssembly = Assembly.Load (assemblyBytes);
         IEnumerable<Type> myIEnumerableType = GetTypesSafely (objAssembly);
         foreach (Type objType in myIEnumerableType)
         {
            if (objType.IsClass)
            {
               if (objType.Name.ToLower ( ) == strCommandName.ToLower ( ))
               {
                  object ibaseObject = Activator.CreateInstance (objType);
                  object[ ] arguments = new object[ ]
                  {
                     commandData,
                     exeConfigPath2,
                     elements
                  };
                  object result = null;
                  result = objType.InvokeMember ("Execute", BindingFlags.Default | BindingFlags.InvokeMethod, null, ibaseObject, arguments);
                  break;
               }
            }
         }
         return Result.Succeeded;
      }

      
      private static IEnumerable<Type> GetTypesSafely (Assembly assembly)
      {
         try
         {
            return assembly.GetTypes ( );
         }
         catch (ReflectionTypeLoadException ex)
         {
            return ex.Types.Where (x => x != null);
         }
      }

   }
   
}