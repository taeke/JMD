﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18033
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DrawMap {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DrawMap.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided number for the BorderEndPoint is not found..
        /// </summary>
        internal static string BORDERENDPOINT_DOES_NOT_EXCIST {
            get {
                return ResourceManager.GetString("BORDERENDPOINT_DOES_NOT_EXCIST", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You can&apos;t call open if the map allready changed. Create a new instance to open..
        /// </summary>
        internal static string CANT_OPEN_IF_CHANGED {
            get {
                return ResourceManager.GetString("CANT_OPEN_IF_CHANGED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There are changes but IgnoreChanges is false..
        /// </summary>
        internal static string CHANGES_BUT_NOT_IGNORE {
            get {
                return ResourceManager.GetString("CHANGES_BUT_NOT_IGNORE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The file does not excist..
        /// </summary>
        internal static string FILE__DOES_NOT_EXCIST {
            get {
                return ResourceManager.GetString("FILE__DOES_NOT_EXCIST", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The filename can not be empty..
        /// </summary>
        internal static string FILENAME_EMPTY {
            get {
                return ResourceManager.GetString("FILENAME_EMPTY", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The filenam excists but MayOverwriteExcisting is false..
        /// </summary>
        internal static string FILENAME_EXCIST_MAY_NOT_OVERWRITE {
            get {
                return ResourceManager.GetString("FILENAME_EXCIST_MAY_NOT_OVERWRITE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The filename is not filled..
        /// </summary>
        internal static string FILENAME_NOT_FILLED {
            get {
                return ResourceManager.GetString("FILENAME_NOT_FILLED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The xml file does not seem to be a JMD.xml file..
        /// </summary>
        internal static string MALFORMED_XML {
            get {
                return ResourceManager.GetString("MALFORMED_XML", resourceCulture);
            }
        }
    }
}
