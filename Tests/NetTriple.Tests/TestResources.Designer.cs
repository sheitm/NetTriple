﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NetTriple.Tests {
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
    internal class TestResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal TestResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NetTriple.Tests.TestResources", typeof(TestResources).Assembly);
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
        ///   Looks up a localized string similar to &lt;http://netriple.com/unittesting/measurement/111&gt; &lt;http://www.w3.org/1999/02/22-rdf-syntax-ns#type&gt; &lt;http://netriple.com/unittesting/Measurement&gt; .
        ///&lt;http://netriple.com/unittesting/measurement/111&gt; &lt;http://netriple.com/unittesting/measurement/value&gt; 123 .
        ///&lt;http://netriple.com/unittesting/measurement/111&gt; &lt;http://netriple.com/unittesting/measurement/unit&gt; &quot;mH&quot; .
        ///&lt;http://netriple.com/unittesting/measurement/111&gt; &lt;http://netriple.com/unittesting/measurements_per_hour&gt; &lt;http://netriple.com/unittesting/hourme [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string NTriples {
            get {
                return ResourceManager.GetString("NTriples", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;http://psi.hafslund.no/sesam/quant/meterreading-day/707057500032064897_REL_ECR_kWh_2014-06-14&gt; &lt;http://www.w3.org/1999/02/22-rdf-syntax-ns#type&gt; &lt;http://psi.hafslund.no/sesam/quant/meterreading-day&gt; .
        ///&lt;http://psi.hafslund.no/sesam/quant/meterreading-day/707057500032064897_REL_ECR_kWh_2014-06-14&gt; &lt;http://psi.hafslund.no/sesam/quant/schema/day&gt; &quot;2014-06-14&quot; .
        ///&lt;http://psi.hafslund.no/sesam/quant/meterreading-day/707057500032064897_REL_ECR_kWh_2014-06-14&gt; &lt;http://psi.hafslund.no/sesam/quant/schema/mnoid&gt; &quot;61 [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string NTriples2 {
            get {
                return ResourceManager.GetString("NTriples2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;http://netriple.com/meter/936569&gt; &lt;http://www.w3.org/1999/02/22-rdf-syntax-ns#type&gt; &lt;http://psi.hafslund.no/sesam/quant/meter&gt; .
        ///&lt;http://netriple.com/meter/936569&gt; &lt;http://nettriple.com/schema/terminal&gt;  &quot;000D6F00003862F5&quot; ..
        /// </summary>
        internal static string TriplesDoubleSpace {
            get {
                return ResourceManager.GetString("TriplesDoubleSpace", resourceCulture);
            }
        }
    }
}
