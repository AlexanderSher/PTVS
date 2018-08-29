﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.PythonTools.Profiling {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.PythonTools.Profiling.ExternalProfilerDriver.Strings", typeof(Strings).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Report callstacks line is {0} {1}.
        /// </summary>
        public static string CallstackReportCmdLineDump {
            get {
                return ResourceManager.GetString("CallstackReportCmdLineDump", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Collect command line is {0} {1}.
        /// </summary>
        public static string CollectCmdLineDump {
            get {
                return ResourceManager.GetString("CollectCmdLineDump", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Couldn&apos;t create specified directory {0}, error: {1}.
        /// </summary>
        public static string DirCreationFailed {
            get {
                return ResourceManager.GetString("DirCreationFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Couldn&apos;t build the module/function dictionary.
        /// </summary>
        public static string ErrorMsgCannotBuildModuleFunctionDict {
            get {
                return ResourceManager.GetString("ErrorMsgCannotBuildModuleFunctionDict", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot find specified CPU utilization report at path {0}.
        /// </summary>
        public static string ErrorMsgCannotFindCPUUtilizationReport {
            get {
                return ResourceManager.GetString("ErrorMsgCannotFindCPUUtilizationReport", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot find specified directory {0}.
        /// </summary>
        public static string ErrorMsgDirectoryDoesNotExist {
            get {
                return ResourceManager.GetString("ErrorMsgDirectoryDoesNotExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specified file {0}/{1} does not exist.
        /// </summary>
        public static string ErrorMsgFileDoesNotExist {
            get {
                return ResourceManager.GetString("ErrorMsgFileDoesNotExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only Linux and Windows are supported at this time.
        /// </summary>
        public static string ErrorMsgOSNotSupported {
            get {
                return ResourceManager.GetString("ErrorMsgOSNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specified path {0} does not exist.
        /// </summary>
        public static string ErrorMsgPathDoesNotExist {
            get {
                return ResourceManager.GetString("ErrorMsgPathDoesNotExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parsing exception caught: {0}.
        /// </summary>
        public static string ErrorMsgUnexpectedInputWhileParsing {
            get {
                return ResourceManager.GetString("ErrorMsgUnexpectedInputWhileParsing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please check you have installed VTune; expected file {0} not found.
        /// </summary>
        public static string ErrorMsgVTuneExpectedFileNotFound {
            get {
                return ResourceManager.GetString("ErrorMsgVTuneExpectedFileNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid runtime specification in CPU utilization report.
        /// </summary>
        public static string ErrorMsgWrongTimeSpecified {
            get {
                return ResourceManager.GetString("ErrorMsgWrongTimeSpecified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Incorrect command line.
        /// </summary>
        public static string IncorrectCommandLine {
            get {
                return ResourceManager.GetString("IncorrectCommandLine", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only VTune 2017 or 2018 supported, see https://software.intel.com/intel-vtune-amplifier-xe.
        /// </summary>
        public static string InstallVTuneFrom {
            get {
                return ResourceManager.GetString("InstallVTuneFrom", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Need an output directory unless in dry run.
        /// </summary>
        public static string OutputDirRequired {
            get {
                return ResourceManager.GetString("OutputDirRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Encountered error while running inferior process: {0}.
        /// </summary>
        public static string ProcessRunnerErrorWhenRunningInferior {
            get {
                return ResourceManager.GetString("ProcessRunnerErrorWhenRunningInferior", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Operation was cancelled with message : {0}.
        /// </summary>
        public static string ProcessRunnerInferiorCancelledWithMsg {
            get {
                return ResourceManager.GetString("ProcessRunnerInferiorCancelledWithMsg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to From process: {0}.
        /// </summary>
        public static string ProcessRunnerInferiorOutputPrefix {
            get {
                return ResourceManager.GetString("ProcessRunnerInferiorOutputPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Symbol path has been specified.
        /// </summary>
        public static string SymbolPathSpecifiedNotification {
            get {
                return ResourceManager.GetString("SymbolPathSpecifiedNotification", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Report timing line {0} {1}.
        /// </summary>
        public static string TimingReportCmdLineDump {
            get {
                return ResourceManager.GetString("TimingReportCmdLineDump", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The path of VTune is : {0}.
        /// </summary>
        public static string VTuneFoundInPath {
            get {
                return ResourceManager.GetString("VTuneFoundInPath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to VTune not found in expected path : {0}.
        /// </summary>
        public static string VTuneNotFoundInExpectedPath {
            get {
                return ResourceManager.GetString("VTuneNotFoundInExpectedPath", resourceCulture);
            }
        }
    }
}
