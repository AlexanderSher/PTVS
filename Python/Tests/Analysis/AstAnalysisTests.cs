// Python Tools for Visual Studio
// Copyright(c) Microsoft Corporation
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABLITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Python.Tests.Utilities;
using Microsoft.Python.Tests.Utilities.FluentAssertions;
using Microsoft.PythonTools;
using Microsoft.PythonTools.Analysis;
using Microsoft.PythonTools.Analysis.FluentAssertions;
using Microsoft.PythonTools.Analysis.LanguageServer;
using Microsoft.PythonTools.Analysis.Values;
using Microsoft.PythonTools.Interpreter;
using Microsoft.PythonTools.Interpreter.Ast;
using Microsoft.PythonTools.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestUtilities;
using Ast = Microsoft.PythonTools.Parsing.Ast;

namespace AnalysisTests {
    [TestClass]
    public class AstAnalysisTests {
        [TestInitialize]
        public void TestInitialize() => TestEnvironmentImpl.TestInitialize();

        public TestContext TestContext { get; set; }

        private string _analysisLog;
        private string _moduleCache;

        public AstAnalysisTests() {
            _moduleCache = null;
        }

        [TestCleanup]
        public void Cleanup() {
            if (TestContext.CurrentTestOutcome != UnitTestOutcome.Passed) {
                if (_analysisLog != null) {
                    Console.WriteLine("Analysis log:");
                    Console.WriteLine(_analysisLog);
                }

                if (_moduleCache != null) {
                    Console.WriteLine("Module cache:");
                    Console.WriteLine(_moduleCache);
                }
            }

            TestEnvironmentImpl.TestCleanup();
        }
        
        private static AstPythonInterpreterFactory CreateInterpreterFactory(InterpreterConfiguration configuration) {
            configuration.AssertInstalled();
            var opts = new InterpreterFactoryCreationOptions {
                DatabasePath = TestData.GetTempPath("AstAnalysisCache"),
                UseExistingCache = false
            };

            Trace.TraceInformation("Cache Path: " + opts.DatabasePath);

            return new AstPythonInterpreterFactory(configuration, opts);
        }

        private static Task<Server> CreateServerAsync(InterpreterConfiguration configuration = null, string searchPath = null) 
            => new Server().InitializeAsync(
                configuration ?? PythonVersions.LatestAvailable,
                searchPath ?? TestData.GetPath(@"TestData\AstAnalysis"));

        private static AstPythonInterpreterFactory CreateInterpreterFactory() => CreateInterpreterFactory(PythonVersions.LatestAvailable);

        private static readonly Lazy<string> _typeShedPath = new Lazy<string>(FindTypeShedForTest);
        private static string TypeShedPath => _typeShedPath.Value;
        private static string FindTypeShedForTest() {
            var candidate = Path.Combine(PathUtils.GetParent(typeof(AstPythonInterpreterFactory).Assembly.Location), "Typeshed");
            if (Directory.Exists(candidate) && Directory.Exists(Path.Combine(candidate, "stdlib", "2and3"))) {
                return candidate;
            }

            var root = TestData.GetPath();

            for (string previousRoot = null; root != previousRoot; previousRoot = root, root = PathUtils.GetParent(root)) {
                candidate = Path.Combine(root, "typeshed");
                if (Directory.Exists(Path.Combine(candidate, "stdlib", "2and3"))) {
                    return candidate;
                }
            }

            return null;
        }


        #region Test cases

        [TestMethod, Priority(0)]
        public void AstClasses() {
            var mod = Parse("Classes.py", PythonLanguageVersion.V35);
            mod.GetMemberNames(null).Should().OnlyContain("C1", "C2", "C3", "C4", "C5",
                "D", "E",
                "F1",
                "f"
            );

            mod.GetMember(null, "C1").Should().BeOfType<AstPythonType>()
                .Which.Documentation.Should().Be("C1");
            mod.GetMember(null, "C2").Should().BeOfType<AstPythonType>();
            mod.GetMember(null, "C3").Should().BeOfType<AstPythonType>();
            mod.GetMember(null, "C4").Should().BeOfType<AstPythonType>();
            mod.GetMember(null, "C5").Should().BeOfType<AstPythonType>()
                .Which.Documentation.Should().Be("C1");
            mod.GetMember(null, "D").Should().BeOfType<AstPythonType>();
            mod.GetMember(null, "E").Should().BeOfType<AstPythonType>();
            mod.GetMember(null, "f").Should().BeOfType<AstPythonFunction>();

            var f1 = mod.GetMember(null, "F1").Should().BeOfType<AstPythonType>().Which;
            f1.GetMemberNames(null).Should().OnlyContain("F2", "F3", "F6", "__class__", "__bases__");
            f1.GetMember(null, "F6").Should().BeOfType<AstPythonType>()
                .Which.Documentation.Should().Be("C1");
            f1.GetMember(null, "F2").Should().BeOfType<AstPythonType>();
            f1.GetMember(null, "F3").Should().BeOfType<AstPythonType>();
            f1.GetMember(null, "__class__").Should().BeOfType<AstPythonType>();
            f1.GetMember(null, "__bases__").Should().BeOfType<AstPythonSequence>();
        }

        [TestMethod, Priority(0)]
        public void AstFunctions() {
            var mod = Parse("Functions.py", PythonLanguageVersion.V35);
            mod.GetMemberNames(null).Should().OnlyContain("f", "f2", "g", "h", "C");

            mod.GetMember(null, "f").Should().BeOfType<AstPythonFunction>()
                .Which.Documentation.Should().Be("f");

            mod.GetMember(null, "f2").Should().BeOfType<AstPythonFunction>()
                .Which.Documentation.Should().Be("f");

            mod.GetMember(null, "g").Should().BeOfType<AstPythonFunction>();
            mod.GetMember(null, "h").Should().BeOfType<AstPythonFunction>();

            var c = mod.GetMember(null, "C").Should().BeOfType<AstPythonType>().Which;
            c.GetMemberNames(null).Should().OnlyContain("i", "j", "C2", "__class__", "__bases__");
            c.GetMember(null, "i").Should().BeOfType<AstPythonFunction>();
            c.GetMember(null, "j").Should().BeOfType<AstPythonFunction>();
            c.GetMember(null, "__class__").Should().BeOfType<AstPythonType>();
            c.GetMember(null, "__bases__").Should().BeOfType<AstPythonSequence>();

            var c2 = c.GetMember(null, "C2").Should().BeOfType<AstPythonType>().Which;
            c2.GetMemberNames(null).Should().OnlyContain("k", "__class__", "__bases__");
            c2.GetMember(null, "k").Should().BeOfType<AstPythonFunction>();
            c2.GetMember(null, "__class__").Should().BeOfType<AstPythonType>();
            c2.GetMember(null, "__bases__").Should().BeOfType<AstPythonSequence>();
        }

        [TestMethod, Priority(0)]
        public async Task AstValues() {
            using (var server = await CreateServerAsync()) {
                var uri = TestData.GetTempPathUri("test-module.py");
                await server.SendDidOpenTextDocument(uri, "from Values import *");
                var analysis = await server.GetAnalysisAsync(uri);

                analysis.Should().HaveVariable("x").OfTypes(BuiltinTypeId.Int)
                    .And.HaveVariable("y").OfTypes(BuiltinTypeId.Str)
                    .And.HaveVariable("z").OfTypes(BuiltinTypeId.Bytes)
                    .And.HaveVariable("pi").OfTypes(BuiltinTypeId.Float)
                    .And.HaveVariable("l").OfTypes(BuiltinTypeId.List)
                    .And.HaveVariable("t").OfTypes(BuiltinTypeId.Tuple)
                    .And.HaveVariable("d").OfTypes(BuiltinTypeId.Dict)
                    .And.HaveVariable("s").OfTypes(BuiltinTypeId.Set)
                    .And.HaveVariable("X").OfTypes(BuiltinTypeId.Int)
                    .And.HaveVariable("Y").OfTypes(BuiltinTypeId.Str)
                    .And.HaveVariable("Z").OfTypes(BuiltinTypeId.Bytes)
                    .And.HaveVariable("PI").OfTypes(BuiltinTypeId.Float)
                    .And.HaveVariable("L").OfTypes(BuiltinTypeId.List)
                    .And.HaveVariable("T").OfTypes(BuiltinTypeId.Tuple)
                    .And.HaveVariable("D").OfTypes(BuiltinTypeId.Dict)
                    .And.HaveVariable("S").OfTypes(BuiltinTypeId.Set);
            }
        }

        [TestMethod, Priority(0)]
        public async Task AstMultiValues() {
            using (var server = await CreateServerAsync()) {
                var uri = TestData.GetTempPathUri("test-module.py");
                await server.SendDidOpenTextDocument(uri, "from MultiValues import *");
                var analysis = await server.GetAnalysisAsync(uri);

                analysis.Should().HaveVariable("x").OfTypes(BuiltinTypeId.Int)
                    .And.HaveVariable("y").OfTypes(BuiltinTypeId.Str)
                    .And.HaveVariable("z").OfTypes(BuiltinTypeId.Bytes)
                    .And.HaveVariable("l").OfTypes(BuiltinTypeId.List)
                    .And.HaveVariable("t").OfTypes(BuiltinTypeId.Tuple)
                    .And.HaveVariable("s").OfTypes(BuiltinTypeId.Set)
                    .And.HaveVariable("XY").OfTypes(BuiltinTypeId.Int, BuiltinTypeId.Str)
                    .And.HaveVariable("XYZ").OfTypes(BuiltinTypeId.Int, BuiltinTypeId.Str, BuiltinTypeId.Bytes)
                    .And.HaveVariable("D").OfTypes(BuiltinTypeId.List, BuiltinTypeId.Tuple, BuiltinTypeId.Dict, BuiltinTypeId.Set);
            }
        }

        [TestMethod, Priority(0)]
        public void AstImports() {
            var mod = Parse("Imports.py", PythonLanguageVersion.V35);
            mod.GetMemberNames(null).Should().OnlyContain("version_info", "a_made_up_module");
        }

        [TestMethod, Priority(0)]
        public async Task AstReturnTypes() {
            using (var server = await CreateServerAsync()) {
                var uri = TestData.GetTempPathUri("test-module.py");
                await server.SendDidOpenTextDocument(uri, @"from ReturnValues import *
R_str = r_str()
R_object = r_object()
R_A1 = A()
R_A2 = A().r_A()
R_A3 = R_A1.r_A()");
                var analysis = await server.GetAnalysisAsync(uri);
                analysis.Should().HaveFunctions("r_a", "r_b", "r_str", "r_object")
                    .And.HaveClasses("A")
                    .And.HaveVariable("R_str").OfTypes(BuiltinTypeId.Str)
                    .And.HaveVariable("R_object").OfTypes(BuiltinTypeId.Object)
                    .And.HaveVariable("R_A1").OfTypes("A").WithDescription("A")
                    .And.HaveVariable("R_A2").OfTypes("A").WithDescription("A")
                    .And.HaveVariable("R_A3").OfTypes("A").WithDescription("A");
            }
        }

        [TestMethod, Priority(0)]
        public async Task AstInstanceMembers() {
            using (var server = await CreateServerAsync()) {
                var uri = TestData.GetTempPathUri("test-module.py");
                await server.SendDidOpenTextDocument(uri, "from InstanceMethod import f1, f2");
                var analysis = await server.GetAnalysisAsync(uri);

                analysis.Should().HaveVariable("f1").WithValueOfType<BuiltinFunctionInfo>(BuiltinTypeId.BuiltinFunction)
                    .And.HaveVariable("f2").WithValueOfType<BoundBuiltinMethodInfo>(BuiltinTypeId.BuiltinMethodDescriptor);
            }
        }
        [TestMethod, Priority(0)]
        public async Task AstInstanceMembers_Random() {
            using (var server = await CreateServerAsync()) {
                var uri = TestData.GetTempPathUri("test-module.py");
                await server.SendDidOpenTextDocument(uri, "from random import *");
                var analysis = await server.GetAnalysisAsync(uri);

                foreach (var fnName in new[] { "seed", "randrange", "gauss" }) {
                    analysis.Should().HaveVariable(fnName)
                        .Which.Should().HaveType(BuiltinTypeId.BuiltinMethodDescriptor)
                        .And.HaveValue<BoundBuiltinMethodInfo>()
                        .Which.Should().HaveOverloadWithParametersAt(0);
                }
            }
        }

        [TestMethod, Priority(0)]
        public async Task AstLibraryMembers_Datetime() {
            using (var server = await CreateServerAsync()) {
                var uri = TestData.GetTempPathUri("test-module.py");
                await server.SendDidOpenTextDocument(uri, "import datetime");
                var analysis = await server.GetAnalysisAsync(uri);

                analysis.Should().HaveBuiltinModule("datetime")
                    .Which.InterpreterModule.Should().HaveMember<AstPythonType>("datetime");

                var dt = analysis.GetValues("datetime.datetime", SourceLocation.MinValue);
            }

            //using (var entry = CreateAnalysis()) {
            //    try {
            //        entry.AddModule("test-module", "import datetime");
            //        entry.WaitForAnalysis();

            //        var dtClass = entry.GetTypes("datetime.datetime").FirstOrDefault(t => t.MemberType == PythonMemberType.Class && t.Name == "datetime");
            //        Assert.IsNotNull(dtClass);

            //        var dayProperty = dtClass.GetMember(entry.ModuleContext, "day");
            //        Assert.IsNotNull(dayProperty);
            //        Assert.AreEqual(PythonMemberType.Property, dayProperty.MemberType);

            //        var prop = dayProperty as AstPythonProperty;
            //        Assert.IsTrue(prop.IsReadOnly);
            //        Assert.AreEqual(BuiltinTypeId.Int, prop.Type.TypeId);

            //        var nowMethod = dtClass.GetMember(entry.ModuleContext, "now");
            //        Assert.IsNotNull(nowMethod);
            //        Assert.AreEqual(PythonMemberType.Method, nowMethod.MemberType);

            //        var func = nowMethod as AstPythonFunction;
            //        Assert.IsTrue(func.IsClassMethod);

            //        Assert.AreEqual(1, func.Overloads.Count);
            //        var overload = func.Overloads[0];
            //        Assert.IsNotNull(overload);
            //        Assert.AreEqual(1, overload.ReturnType.Count);
            //        Assert.AreEqual("datetime", overload.ReturnType[0].Name);
            //    } finally {
            //        _analysisLog = entry.GetLogContent(CultureInfo.InvariantCulture);
            //    }
            //}
        }
        /*
        [TestMethod, Priority(0)]
        public void AstComparisonTypeInference() {
            using (var entry = CreateAnalysis()) {
                try {
                    var code = @"
class BankAccount(object):
    def __init__(self, initial_balance=0):
        self.balance = initial_balance
    def withdraw(self, amount):
        self.balance -= amount
    def overdrawn(self):
        return self.balance < 0
";
                    entry.AddModule("test-module", code);
                    entry.WaitForAnalysis();

                    var moduleEntry = entry.Modules.First().Value;

                    var varDef = moduleEntry.Analysis.Scope.AllVariables.First(x => x.Key == "BankAccount").Value;
                    var clsInfo = varDef.Types.First(x => x is ClassInfo).First() as ClassInfo;
                    var overdrawn = clsInfo.Scope.GetVariable("overdrawn").Types.First() as FunctionInfo;

                    Assert.AreEqual(1, overdrawn.Overloads.Count());
                    var overload = overdrawn.Overloads.First();
                    Assert.IsNotNull(overload);
                    Assert.AreEqual(1, overload.ReturnType.Count);
                    Assert.AreEqual("bool", overload.ReturnType[0]);
                } finally {
                    _analysisLog = entry.GetLogContent(CultureInfo.InvariantCulture);
                }
            }
        }

        [TestMethod, Priority(0)]
        public void AstSearchPathsThroughFactory() {
            using (var evt = new ManualResetEvent(false))
            using (var analysis = CreateAnalysis()) {
                try {
                    var fact = (AstPythonInterpreterFactory)analysis.Analyzer.InterpreterFactory;
                    var interp = (AstPythonInterpreter)analysis.Analyzer.Interpreter;

                    interp.ModuleNamesChanged += (s, e) => evt.Set();

                    fact.SetCurrentSearchPaths(new[] { TestData.GetPath("TestData\\AstAnalysis") });
                    Assert.IsTrue(evt.WaitOne(1000), "Timeout waiting for paths to update");
                    AssertUtil.ContainsAtLeast(interp.GetModuleNames(), "Values");
                    Assert.IsNotNull(interp.ImportModule("Values"), "Module was not available");

                    evt.Reset();
                    fact.SetCurrentSearchPaths(new string[0]);
                    Assert.IsTrue(evt.WaitOne(1000), "Timeout waiting for paths to update");
                    AssertUtil.DoesntContain(interp.GetModuleNames(), "Values");
                    Assert.IsNull(interp.ImportModule("Values"), "Module was not removed");
                } finally {
                    _analysisLog = analysis.GetLogContent(CultureInfo.InvariantCulture);
                }
            }
        }

        [TestMethod, Priority(0)]
        public void AstSearchPathsThroughAnalyzer() {
            using (var evt = new AutoResetEvent(false))
            using (var analysis = CreateAnalysis()) {
                try {
                    var fact = (AstPythonInterpreterFactory)analysis.Analyzer.InterpreterFactory;
                    var interp = (AstPythonInterpreter)analysis.Analyzer.Interpreter;

                    interp.ModuleNamesChanged += (s, e) => evt.SetIfNotDisposed();

                    analysis.Analyzer.SetSearchPaths(new[] { TestData.GetPath("TestData\\AstAnalysis") });
                    Assert.IsTrue(evt.WaitOne(1000), "Timeout waiting for paths to update");
                    AssertUtil.ContainsAtLeast(interp.GetModuleNames(), "Values");
                    Assert.IsNotNull(interp.ImportModule("Values"), "Module was not available");

                    analysis.Analyzer.SetSearchPaths(new string[0]);
                    Assert.IsTrue(evt.WaitOne(1000), "Timeout waiting for paths to update");
                    AssertUtil.DoesntContain(interp.GetModuleNames(), "Values");
                    Assert.IsNull(interp.ImportModule("Values"), "Module was not removed");
                } finally {
                    _analysisLog = analysis.GetLogContent(CultureInfo.InvariantCulture);
                }
            }
        }

        [TestMethod, Priority(0)]
        public void AstTypeStubPaths_NoStubs() {
            using (var analysis = CreateAnalysis()) {
                try {
                    analysis.SetSearchPaths(TestData.GetPath("TestData\\AstAnalysis"));
                    analysis.AddModule("test-module", "import Package.Module\n\nc = Package.Module.Class()");

                    analysis.SetLimits(new AnalysisLimits() { UseTypeStubPackages = false });
                    analysis.SetTypeStubSearchPath();
                    analysis.ReanalyzeAll();
                    analysis.AssertHasAttr("c", "untyped_method", "inferred_method");
                    analysis.AssertNotHasAttr("c", "typed_method", "typed_method_2");
                } finally {
                    _analysisLog = analysis.GetLogContent(CultureInfo.InvariantCulture);
                }
            }
        }

        [TestMethod, Priority(0)]
        public void AstTypeStubPaths_MergeStubs() {
            using (var analysis = CreateAnalysis()) {
                try {
                    analysis.SetSearchPaths(TestData.GetPath("TestData\\AstAnalysis"));
                    analysis.AddModule("test-module", "import Package.Module\n\nc = Package.Module.Class()");

                    analysis.SetLimits(new AnalysisLimits() { UseTypeStubPackages = true, UseTypeStubPackagesExclusively = false });
                    analysis.ReanalyzeAll();
                    analysis.AssertHasAttr("c", "untyped_method", "inferred_method", "typed_method");
                    analysis.AssertNotHasAttr("c", "typed_method_2");
                } finally {
                    _analysisLog = analysis.GetLogContent(CultureInfo.InvariantCulture);
                }
            }
        }

        [TestMethod, Priority(0)]
        public void AstTypeStubPaths_MergeStubsPath() {
            using (var analysis = CreateAnalysis()) {
                try {
                    analysis.SetSearchPaths(TestData.GetPath("TestData\\AstAnalysis"));
                    analysis.AddModule("test-module", "import Package.Module\n\nc = Package.Module.Class()");

                    analysis.SetTypeStubSearchPath(TestData.GetPath("TestData\\AstAnalysis\\Stubs"));
                    analysis.ReanalyzeAll();
                    analysis.AssertHasAttr("c", "untyped_method", "inferred_method", "typed_method_2");
                    analysis.AssertNotHasAttr("c", "typed_method");
                } finally {
                    _analysisLog = analysis.GetLogContent(CultureInfo.InvariantCulture);
                }
            }
        }

        [TestMethod, Priority(0)]
        public void AstTypeStubPaths_ExclusiveStubs() {
            using (var analysis = CreateAnalysis()) {
                try {
                    analysis.SetSearchPaths(TestData.GetPath("TestData\\AstAnalysis"));
                    analysis.AddModule("test-module", "import Package.Module\n\nc = Package.Module.Class()");

                    analysis.SetLimits(new AnalysisLimits() { UseTypeStubPackages = true, UseTypeStubPackagesExclusively = true });
                    analysis.SetTypeStubSearchPath(TestData.GetPath("TestData\\AstAnalysis\\Stubs"));
                    analysis.ReanalyzeAll();
                    analysis.AssertHasAttr("c", "typed_method_2");
                    analysis.AssertNotHasAttr("c", "untyped_method", "inferred_method", "typed_method");
                } finally {
                    _analysisLog = analysis.GetLogContent(CultureInfo.InvariantCulture);
                }
            }
        }
        */
        [TestMethod, Priority(0)]
        public void AstMro() {
            var O = new AstPythonType("O");
            var A = new AstPythonType("A");
            var B = new AstPythonType("B");
            var C = new AstPythonType("C");
            var D = new AstPythonType("D");
            var E = new AstPythonType("E");
            var F = new AstPythonType("F");

            F.SetBases(null, new[] { O });
            E.SetBases(null, new[] { O });
            D.SetBases(null, new[] { O });
            C.SetBases(null, new[] { D, F });
            B.SetBases(null, new[] { D, E });
            A.SetBases(null, new[] { B, C });

            AstPythonType.CalculateMro(A).Should().Equal(new []{ "A", "B", "C", "D", "E", "F", "O" }, (p, n) => p.Name == n);
            AstPythonType.CalculateMro(B).Should().Equal(new []{ "B", "D", "E", "O" }, (p, n) => p.Name == n);
            AstPythonType.CalculateMro(C).Should().Equal(new []{ "C", "D", "F", "O" }, (p, n) => p.Name == n);
        }

        private static IPythonModule Parse(string path, PythonLanguageVersion version) {
            var interpreter = InterpreterFactoryCreator.CreateAnalysisInterpreterFactory(version.ToVersion()).CreateInterpreter();
            if (!Path.IsPathRooted(path)) {
                path = TestData.GetPath(Path.Combine("TestData", "AstAnalysis", path));
            }
            return PythonModuleLoader.FromFile(interpreter, path, version);
        }

        [TestMethod, Priority(0)]
        public async Task ScrapedTypeWithWrongModule() {
            var version = PythonVersions.Versions
                .Concat(PythonVersions.AnacondaVersions)
                .LastOrDefault(v => Directory.Exists(Path.Combine(v.PrefixPath, "Lib", "site-packages", "numpy")));
            version.AssertInstalled();

            Console.WriteLine("Using {0}", version.PrefixPath);
            using (var server = await CreateServerAsync()) {
                var uri = TestData.GetTempPathUri("test-module.py");
                await server.SendDidOpenTextDocument(uri, "import numpy.core.numeric as NP; ndarray = NP.ndarray");
                var analysis = await server.GetAnalysisAsync(uri, 15000);

                analysis.Should().HaveVariable("ndarray")
                    .Which.Should().HaveValue<BuiltinClassInfo>();
            }
        }

        [TestMethod, Priority(0)]
        public async Task ScrapedSpecialFloats() {
            using (var server = await CreateServerAsync()) {
                var uri = TestData.GetTempPathUri("test-module.py");
                await server.SendDidOpenTextDocument(uri, "import math; inf = math.inf; nan = math.nan");
                var analysis = await server.GetAnalysisAsync(uri, 15000);

                analysis.Should().HaveVariable("math")
                    .And.HaveVariable("inf").WithValueOfType<NumericInstanceInfo>(BuiltinTypeId.Float)
                    .And.HaveVariable("nan").WithValueOfType<NumericInstanceInfo>(BuiltinTypeId.Float);
            }
        }

        #endregion

        #region Black-box sanity tests
        // "Do we crash?"

        [TestMethod, Priority(0)]
        public void AstBuiltinScrapeV37() => AstBuiltinScrape(PythonVersions.Python37_x64 ?? PythonVersions.Python37);

        [TestMethod, Priority(0)]
        public void AstBuiltinScrapeV36() => AstBuiltinScrape(PythonVersions.Python36_x64 ?? PythonVersions.Python36);

        [TestMethod, Priority(0)]
        public void AstBuiltinScrapeV35() => AstBuiltinScrape(PythonVersions.Python35_x64 ?? PythonVersions.Python35);

        [TestMethod, Priority(0)]
        public void AstBuiltinScrapeV34() => AstBuiltinScrape(PythonVersions.Python34_x64 ?? PythonVersions.Python34);

        [TestMethod, Priority(0)]
        public void AstBuiltinScrapeV33() => AstBuiltinScrape(PythonVersions.Python33_x64 ?? PythonVersions.Python33);

        [TestMethod, Priority(0)]
        public void AstBuiltinScrapeV32() => AstBuiltinScrape(PythonVersions.Python32_x64 ?? PythonVersions.Python32);

        [TestMethod, Priority(0)]
        public void AstBuiltinScrapeV31() => AstBuiltinScrape(PythonVersions.Python31_x64 ?? PythonVersions.Python31);

        [TestMethod, Priority(0)]
        public void AstBuiltinScrapeV27() => AstBuiltinScrape(PythonVersions.Python27_x64 ?? PythonVersions.Python27);

        [TestMethod, Priority(0)]
        public void AstBuiltinScrapeV26() => AstBuiltinScrape(PythonVersions.Python26_x64 ?? PythonVersions.Python26);

        [TestMethod, Priority(0)]
        public void AstBuiltinScrapeIPy27() => AstBuiltinScrape(PythonVersions.IronPython27_x64 ?? PythonVersions.IronPython27);


        private void AstBuiltinScrape(InterpreterConfiguration configuration) {
            AstScrapedPythonModule.KeepAst = true;
            configuration.AssertInstalled();
            using (var factory = CreateInterpreterFactory(configuration))
            using (var analyzer = PythonAnalyzer.CreateSynchronously(factory)) {
                try {
                    var interp = (AstPythonInterpreter)analyzer.Interpreter;
                    var ctxt = interp.CreateModuleContext();

                    var mod = interp.ImportModule(interp.BuiltinModuleName);
                    Assert.IsInstanceOfType(mod, typeof(AstBuiltinsPythonModule));
                    mod.Imported(ctxt);

                    var modPath = factory.GetCacheFilePath(factory.Configuration.InterpreterPath);
                    if (File.Exists(modPath)) {
                        _moduleCache = File.ReadAllText(modPath);
                    }

                    var errors = ((AstScrapedPythonModule)mod).ParseErrors ?? Enumerable.Empty<string>();
                    foreach (var err in errors) {
                        Console.WriteLine(err);
                    }
                    Assert.AreEqual(0, errors.Count(), "Parse errors occurred");

                    var seen = new HashSet<string>();
                    foreach (var stmt in ((Ast.SuiteStatement)((AstScrapedPythonModule)mod).Ast.Body).Statements) {
                        if (stmt is Ast.ClassDefinition cd) {
                            Assert.IsTrue(seen.Add(cd.Name), $"Repeated use of {cd.Name} at index {cd.StartIndex} in {modPath}");
                        } else if (stmt is Ast.FunctionDefinition fd) {
                            Assert.IsTrue(seen.Add(fd.Name), $"Repeated use of {fd.Name} at index {fd.StartIndex} in {modPath}");
                        } else if (stmt is Ast.AssignmentStatement assign && assign.Left.FirstOrDefault() is Ast.NameExpression n) {
                            Assert.IsTrue(seen.Add(n.Name), $"Repeated use of {n.Name} at index {n.StartIndex} in {modPath}");
                        }
                    }

                    // Ensure we can get all the builtin types
                    foreach (BuiltinTypeId v in Enum.GetValues(typeof(BuiltinTypeId))) {
                        var type = interp.GetBuiltinType(v);
                        type.Should().NotBeNull().And.BeOfType<AstPythonBuiltinType>($"Did not find {v}");
                    }

                    // Ensure we cannot see or get builtin types directly
                    mod.GetMemberNames(null).Should().NotContain(Enum.GetNames(typeof(BuiltinTypeId)).Select(n => $"__{n}"));

                    foreach (var id in Enum.GetNames(typeof(BuiltinTypeId))) {
                        mod.GetMember(null, $"__{id}").Should().BeNull(id);
                    }
                } finally {
                    _analysisLog = factory.GetAnalysisLogContent(CultureInfo.InvariantCulture);
                }
            }
        }

        [TestMethod, Priority(0)]
        public void AstNativeBuiltinScrapeV37x64() => AstNativeBuiltinScrape(PythonVersions.Python37_x64);

        [TestMethod, Priority(0)]
        public void AstNativeBuiltinScrapeV36x64() => AstNativeBuiltinScrape(PythonVersions.Python36_x64);

        [TestMethod, Priority(0)]
        public void AstNativeBuiltinScrapeV35x64() => AstNativeBuiltinScrape(PythonVersions.Python35_x64);

        [TestMethod, Priority(0)]
        public void AstNativeBuiltinScrapeV34x64() => AstNativeBuiltinScrape(PythonVersions.Python34_x64);

        [TestMethod, Priority(0)]
        public void AstNativeBuiltinScrapeV33x64() => AstNativeBuiltinScrape(PythonVersions.Python33_x64);

        [TestMethod, Priority(0)]
        public void AstNativeBuiltinScrapeV32x64() => AstNativeBuiltinScrape(PythonVersions.Python32_x64);

        [TestMethod, Priority(0)]
        public void AstNativeBuiltinScrapeV31x64() => AstNativeBuiltinScrape(PythonVersions.Python31_x64);

        [TestMethod, Priority(0)]
        public void AstNativeBuiltinScrapeV27x64() => AstNativeBuiltinScrape(PythonVersions.Python27_x64);

        [TestMethod, Priority(0)]
        public void AstNativeBuiltinScrapeV26x64() => AstNativeBuiltinScrape(PythonVersions.Python26_x64);

        [TestMethod, Priority(0)]
        public void AstNativeBuiltinScrapeV37x86() => AstNativeBuiltinScrape(PythonVersions.Python37);

        [TestMethod, Priority(0)]
        public void AstNativeBuiltinScrapeV36x86() => AstNativeBuiltinScrape(PythonVersions.Python36);

        [TestMethod, Priority(0)]
        public void AstNativeBuiltinScrapeV35x86() => AstNativeBuiltinScrape(PythonVersions.Python35);

        [TestMethod, Priority(0)]
        public void AstNativeBuiltinScrapeV34x86() => AstNativeBuiltinScrape(PythonVersions.Python34);

        [TestMethod, Priority(0)]
        public void AstNativeBuiltinScrapeV33x86() => AstNativeBuiltinScrape(PythonVersions.Python33);

        [TestMethod, Priority(0)]
        public void AstNativeBuiltinScrapeV32x86() => AstNativeBuiltinScrape(PythonVersions.Python32);

        [TestMethod, Priority(0)]
        public void AstNativeBuiltinScrapeV31x86() => AstNativeBuiltinScrape(PythonVersions.Python31);

        [TestMethod, Priority(0)]
        public void AstNativeBuiltinScrapeV27x86() => AstNativeBuiltinScrape(PythonVersions.Python27);

        [TestMethod, Priority(0)]
        public void AstNativeBuiltinScrapeV26x86() => AstNativeBuiltinScrape(PythonVersions.Python26);


        private void AstNativeBuiltinScrape(InterpreterConfiguration configuration) {
            AstScrapedPythonModule.KeepAst = true;
            configuration.AssertInstalled();
            using (var factory = CreateInterpreterFactory(configuration))
            using (var analyzer = PythonAnalyzer.CreateSynchronously(factory)) {
                try {
                    var interpreter = (AstPythonInterpreter)analyzer.Interpreter;
                    var ctxt = interpreter.CreateModuleContext();

                    var dllsDir = PathUtils.GetAbsoluteDirectoryPath(factory.Configuration.PrefixPath, "DLLs");
                    if (!Directory.Exists(dllsDir)) {
                        Assert.Inconclusive("Configuration does not have DLLs");
                    }

                    var report = new List<string>();
                    var permittedImports = factory.GetLanguageVersion().Is2x() ?
                        new[] { interpreter.BuiltinModuleName, "exceptions" } :
                        new[] { interpreter.BuiltinModuleName };

                    foreach (var pyd in Microsoft.PythonTools.Analysis.Infrastructure.PathUtils.EnumerateFiles(dllsDir, "*", recurse: false).Where(ModulePath.IsPythonFile)) {
                        var mp = ModulePath.FromFullPath(pyd);
                        if (mp.IsDebug) {
                            continue;
                        }

                        Console.WriteLine("Importing {0} from {1}", mp.ModuleName, mp.SourceFile);
                        var mod = interpreter.ImportModule(mp.ModuleName);
                        Assert.IsInstanceOfType(mod, typeof(AstScrapedPythonModule));
                        mod.Imported(ctxt);

                        var modPath = factory.GetCacheFilePath(pyd);
                        Assert.IsTrue(File.Exists(modPath), "No cache file created");
                        _moduleCache = File.ReadAllText(modPath);

                        var errors = ((AstScrapedPythonModule)mod).ParseErrors ?? Enumerable.Empty<string>();
                        foreach (var err in errors) {
                            Console.WriteLine(err);
                        }
                        Assert.AreEqual(0, errors.Count(), "Parse errors occurred");

                        var ast = ((AstScrapedPythonModule)mod).Ast;

                        
                        var imports = ((Ast.SuiteStatement)ast.Body).Statements
                            .OfType<Ast.ImportStatement>()
                            .SelectMany(s => s.Names)
                            .Select(n => n.MakeString())
                            .Except(permittedImports)
                            .ToArray();

                        // We expect no imports (after excluding builtins)
                        report.AddRange(imports.Select(n => $"{mp.ModuleName} imported {n}"));

                        _moduleCache = null;
                    }

                    report.Should().BeEmpty();
                } finally {
                    _analysisLog = factory.GetAnalysisLogContent(CultureInfo.InvariantCulture);
                }
            }
        }

        [TestMethod, TestCategory("60s"), Priority(0)]
        public async Task FullStdLibV37() {
            var v = PythonVersions.Python37 ?? PythonVersions.Python37_x64;
            await FullStdLibTest(v);
        }


        [TestMethod, TestCategory("60s"), Priority(0)]
        public async Task FullStdLibV36() {
            var v = PythonVersions.Python36 ?? PythonVersions.Python36_x64;
            await FullStdLibTest(v);
        }


        [TestMethod, TestCategory("60s"), Priority(0)]
        public async Task FullStdLibV35() {
            var v = PythonVersions.Python35 ?? PythonVersions.Python35_x64;
            await FullStdLibTest(v);
        }

        [TestMethod, TestCategory("60s"), Priority(0)]
        public async Task FullStdLibV34() {
            var v = PythonVersions.Python34 ?? PythonVersions.Python34_x64;
            await FullStdLibTest(v);
        }

        [TestMethod, TestCategory("60s"), Priority(0)]
        public async Task FullStdLibV33() {
            var v = PythonVersions.Python33 ?? PythonVersions.Python33_x64;
            await FullStdLibTest(v);
        }

        [TestMethod, TestCategory("60s"), Priority(0)]
        public async Task FullStdLibV32() {
            var v = PythonVersions.Python31 ?? PythonVersions.Python32_x64;
            await FullStdLibTest(v);
        }

        [TestMethod, TestCategory("60s"), Priority(0)]
        public async Task FullStdLibV31() {
            var v = PythonVersions.Python31 ?? PythonVersions.Python32_x64;
            await FullStdLibTest(v);
        }

        [TestMethod, TestCategory("60s"), Priority(0)]
        public async Task FullStdLibV27() {
            var v = PythonVersions.Python27 ?? PythonVersions.Python27_x64;
            await FullStdLibTest(v);
        }

        [TestMethod, TestCategory("60s"), Priority(0)]
        public async Task FullStdLibV26() {
            var v = PythonVersions.Python26 ?? PythonVersions.Python26_x64;
            await FullStdLibTest(v);
        }

        [TestMethod, TestCategory("60s"), Priority(1)]
        [Timeout(10 * 60 * 1000)]
        public async Task FullStdLibAnaconda3() {
            var v = PythonVersions.Anaconda36_x64 ?? PythonVersions.Anaconda36;
            await FullStdLibTest(v,
                // Crashes Python on import
                "sklearn.linear_model.cd_fast",
                // Crashes Python on import
                "sklearn.cluster._k_means_elkan"
            );
        }

        [TestMethod, TestCategory("60s"), Priority(1)]
        [Timeout(10 * 60 * 1000)]
        public async Task FullStdLibAnaconda2() {
            var v = PythonVersions.Anaconda27_x64 ?? PythonVersions.Anaconda27;
            await FullStdLibTest(v,
                // Fails to import due to SxS manifest issues
                "dde",
                "win32ui"
            );
        }

        [TestMethod, TestCategory("60s"), Priority(0)]
        public async Task FullStdLibIPy27() {
            var v = PythonVersions.IronPython27 ?? PythonVersions.IronPython27_x64;
            await FullStdLibTest(v);
        }


        private async Task FullStdLibTest(InterpreterConfiguration configuration, params string[] skipModules) {
            configuration.AssertInstalled();
            var factory = new AstPythonInterpreterFactory(configuration, new InterpreterFactoryCreationOptions {
                DatabasePath = TestData.GetTempPath(),
                UseExistingCache = false
            });
            var modules = ModulePath.GetModulesInLib(configuration.PrefixPath).ToList();

            var skip = new HashSet<string>(skipModules);
            skip.UnionWith(new[] {
                "matplotlib.backends._backend_gdk",
                "matplotlib.backends._backend_gtkagg",
                "matplotlib.backends._gtkagg",
                "test.test_pep3131",
                "test.test_unicode_identifiers"
            });
            skip.UnionWith(modules.Select(m => m.FullName).Where(n => n.StartsWith("test.badsyntax") || n.StartsWith("test.bad_coding")));

            var anySuccess = false;
            var anyExtensionSuccess = false;
            var anyExtensionSeen = false;
            var anyParseError = false;

            using (var analyzer = await PythonAnalyzer.CreateAsync(factory)) {
                try {
                    PythonModuleLoader.KeepParseErrors = true;
                    var tasks = new List<Task<Tuple<ModulePath, IPythonModule>>>();
                    var interp = (AstPythonInterpreter)analyzer.Interpreter;
                    foreach (var m in skip) {
                        interp.AddUnimportableModule(m);
                    }

                    foreach (var r in modules
                        .Where(m => !skip.Contains(m.ModuleName))
                        .GroupBy(m => {
                            int i = m.FullName.IndexOf('.');
                            return i <= 0 ? m.FullName : m.FullName.Remove(i);
                        })
                        .AsParallel()
                        .SelectMany(g => g.Select(m => Tuple.Create(m, interp.ImportModule(m.ModuleName))))
                    ) {
                        var modName = r.Item1;
                        var mod = r.Item2;

                        anyExtensionSeen |= modName.IsNativeExtension;
                        if (mod == null) {
                            Trace.TraceWarning("failed to import {0} from {1}", modName.ModuleName, modName.SourceFile);
                        } else if (mod is AstScrapedPythonModule aspm) {
                            var errors = aspm.ParseErrors.ToArray();
                            if (errors.Any()) {
                                anyParseError = true;
                                Trace.TraceError("Parse errors in {0}", modName.SourceFile);
                                foreach (var e in errors) {
                                    Trace.TraceError(e);
                                }
                            } else {
                                anySuccess = true;
                                anyExtensionSuccess |= modName.IsNativeExtension;
                                mod.GetMemberNames(null).ToList();
                            }
                        } else if (mod is AstPythonModule apm) {
                            var filteredErrors = apm.ParseErrors.Where(e => !e.Contains("encoding problem")).ToArray();
                            if (filteredErrors.Any()) {
                                // Do not fail due to errors in installed packages
                                if (!apm.FilePath.Contains("site-packages")) {
                                    anyParseError = true;
                                }
                                Trace.TraceError("Parse errors in {0}", modName.SourceFile);
                                foreach (var e in filteredErrors) {
                                    Trace.TraceError(e);
                                }
                            } else {
                                anySuccess = true;
                                anyExtensionSuccess |= modName.IsNativeExtension;
                                mod.GetMemberNames(null).ToList();
                            }
                        } else {
                            Trace.TraceError("imported {0} as type {1}", modName.ModuleName, mod.GetType().FullName);
                        }
                    }
                } finally {
                    _analysisLog = factory.GetAnalysisLogContent(CultureInfo.InvariantCulture);
                    PythonModuleLoader.KeepParseErrors = false;
                }
            }
            Assert.IsTrue(anySuccess, "failed to import any modules at all");
            Assert.IsTrue(anyExtensionSuccess || !anyExtensionSeen, "failed to import all extension modules");
            Assert.IsFalse(anyParseError, "parse errors occurred");
        }

        #endregion
        /*
        #region Type Annotation tests
        [TestMethod, Priority(0)]
        public async Task AstTypeAnnotationConversion() {
            using (var factory = CreateInterpreterFactory())
            using (var analyzer = await PythonAnalyzer.CreateAsync(factory)) {
                try {
                    analyzer.SetSearchPaths(new [] { TestData.GetPath(@"TestData\AstAnalysis") });
                    analysis.AddModule("test-module", @"from ReturnAnnotations import *
x = f()
y = g()");
                    analysis.WaitForAnalysis();

                    analysis.AssertIsInstance("x", BuiltinTypeId.Int);
                    analysis.AssertIsInstance("y", BuiltinTypeId.Str);
                    var sigs = analysis.GetSignatures("f").ToArray();
                    Assert.AreEqual(1, sigs.Length);
                    Assert.AreEqual(1, sigs[0].Parameters.Length);
                    var p = sigs[0].Parameters[0];
                    Assert.AreEqual("p", p.Name);
                    Assert.AreEqual("int", p.Type);
                    Assert.AreEqual("", p.DefaultValue ?? "");
                } finally {
                    _analysisLog = factory.GetAnalysisLogContent(CultureInfo.InvariantCulture);
                }
            }
        }
        #endregion

        #region Type Shed tests

        [TestMethod, Priority(0)]
        public void TypeShedElementTree() {
            using (var analysis = CreateAnalysis()) {
                analysis.SetTypeStubSearchPath(TypeShedPath);
                try {
                    var entry = analysis.AddModule("test-module", @"import xml.etree.ElementTree as ET

e = ET.Element()
e2 = e.makeelement()
iterfind = e.iterfind
l = iterfind()");
                    analysis.WaitForAnalysis();

                    analysis.AssertHasParameters("ET.Element", "tag", "attrib", "**extra");
                    analysis.AssertHasParameters("e.makeelement", "tag", "attrib");
                    analysis.AssertHasParameters("iterfind", "path", "namespaces");
                    analysis.AssertIsInstance("l", BuiltinTypeId.List);
                } finally {
                    _analysisLog = analysis.GetLogContent(CultureInfo.InvariantCulture);
                }
            }
        }

        [TestMethod, Priority(0)]
        public void TypeShedChildModules() {
            string[] expected;

            using (var analysis = CreateAnalysis()) {
                analysis.SetLimits(new AnalysisLimits() { UseTypeStubPackages = false });
                try {
                    var entry = analysis.AddModule("test-module", @"import urllib");
                    analysis.WaitForAnalysis();

                    expected = analysis.Analyzer.GetModuleMembers(entry.AnalysisContext, new[] { "urllib" }, false)
                        .Select(m => m.Name)
                        .OrderBy(n => n)
                        .ToArray();
                    Assert.AreNotEqual(0, expected.Length);
                    AssertUtil.ContainsAtLeast(expected, "parse", "request");
                } finally {
                    _analysisLog = analysis.GetLogContent(CultureInfo.InvariantCulture);
                }
            }

            using (var analysis = CreateAnalysis()) {
                analysis.SetTypeStubSearchPath(TypeShedPath);
                try {
                    var entry = analysis.AddModule("test-module", @"import urllib");
                    analysis.WaitForAnalysis();

                    var mods = analysis.Analyzer.GetModuleMembers(entry.AnalysisContext, new[] { "urllib" }, false)
                        .Select(m => m.Name)
                        .OrderBy(n => n)
                        .ToArray();
                    AssertUtil.ArrayEquals(expected, mods);
                } finally {
                    _analysisLog = analysis.GetLogContent(CultureInfo.InvariantCulture);
                }
            }
        }

        [TestMethod, Priority(0)]
        public void TypeShedSysExcInfo() {
            using (var analysis = CreateAnalysis()) {
                analysis.SetTypeStubSearchPath(TypeShedPath);
                try {
                    var entry = analysis.AddModule("test-module", @"import sys

e1, e2, e3 = sys.exc_info()");
                    analysis.WaitForAnalysis();

                    var funcs = analysis.GetValues("sys.exc_info").SelectMany(f => f.Overloads).ToArray();
                    AssertUtil.ContainsExactly(
                        funcs.Select(o => o.ToString()).Select(s => s.Remove(s.IndexOf("'''"))),
                        "exc_info()->[tuple[type, BaseException, None]]",
                        "exc_info()->[tuple]"
                    );

                    analysis.AssertIsInstance("e1", BuiltinTypeId.Type);
                    analysis.AssertIsInstance("e2", "BaseException");
                    analysis.AssertIsInstance("e3", BuiltinTypeId.NoneType);
                } finally {
                    _analysisLog = analysis.GetLogContent(CultureInfo.InvariantCulture);
                }
            }
        }

        [TestMethod, Priority(0)]
        public void TypeShedJsonMakeScanner() {
            using (var analysis = CreateAnalysis()) {
                analysis.SetTypeStubSearchPath(TypeShedPath);
                try {
                    var entry = analysis.AddModule("test-module", @"import _json

scanner = _json.make_scanner()");
                    analysis.WaitForAnalysis();

                    var overloads = analysis.GetSignatures("scanner");
                    AssertUtil.ContainsExactly(
                        overloads.Select(o => o.ToString()).Select(s => s.Remove(s.IndexOf("'''"))),
                        "__call__(string:str=,index:int=)->[tuple[None, int]]"
                    );
                } finally {
                    _analysisLog = analysis.GetLogContent(CultureInfo.InvariantCulture);
                }
            }
        }

        [TestMethod, Priority(0)]
        public void TypeShedSysInfo() {
            using (var analysis = CreateAnalysis()) {
                analysis.SetTypeStubSearchPath(TypeShedPath);
                analysis.SetLimits(new AnalysisLimits { UseTypeStubPackages = true, UseTypeStubPackagesExclusively = true });
                try {
                    var entry = analysis.AddModule("test-module", @"import sys

l_1 = sys.argv

s_1 = sys.argv[0]
s_2 = next(iter(sys.argv))
s_3 = sys.stdout.encoding

f_1 = sys.stdout.write
f_2 = sys.__stdin__.read

i_1 = sys.flags.debug
i_2 = sys.flags.quiet
i_3 = sys.implementation.version.major
i_4 = sys.getsizeof(None)
i_5 = sys.getwindowsversion().platform_version[0]
");
                    analysis.WaitForAnalysis();

                    analysis.AssertIsInstance("l_1", BuiltinTypeId.List);

                    analysis.AssertIsInstance("s_1", BuiltinTypeId.Str);
                    analysis.AssertIsInstance("s_2", BuiltinTypeId.Str);
                    analysis.AssertIsInstance("s_3", BuiltinTypeId.Str);

                    analysis.AssertIsInstance("f_1", BuiltinTypeId.BuiltinMethodDescriptor);
                    analysis.AssertIsInstance("f_2", BuiltinTypeId.BuiltinMethodDescriptor);

                    analysis.AssertIsInstance("i_1", BuiltinTypeId.Int);
                    analysis.AssertIsInstance("i_2", BuiltinTypeId.Int);
                    analysis.AssertIsInstance("i_3", BuiltinTypeId.Int);
                    analysis.AssertIsInstance("i_4", BuiltinTypeId.Int);
                    analysis.AssertIsInstance("i_5", BuiltinTypeId.Int);
                } finally {
                    _analysisLog = analysis.GetLogContent(CultureInfo.InvariantCulture);
                }
            }
        }

        [TestMethod, Priority(0)]
        public void TypeStubConditionalDefine() {
            var seen = new HashSet<Version>();

            var code = @"import sys

if sys.version_info < (2, 7):
    LT_2_7 : bool = ...
if sys.version_info <= (2, 7):
    LE_2_7 : bool = ...
if sys.version_info > (2, 7):
    GT_2_7 : bool = ...
if sys.version_info >= (2, 7):
    GE_2_7 : bool = ...

";

            var fullSet = new[] { "LT_2_7", "LE_2_7", "GT_2_7", "GE_2_7" };

            foreach (var ver in PythonVersions.Versions) {
                if (!seen.Add(ver.Version)) {
                    continue;
                }

                Console.WriteLine("Testing with {0}", ver.InterpreterPath);

                var interpreter = InterpreterFactoryCreator.CreateAnalysisInterpreterFactory(ver.Version).CreateInterpreter();
                var entry = PythonModuleLoader.FromStream(interpreter, new MemoryStream(Encoding.ASCII.GetBytes(code)), "testmodule.pyi", ver.Version);

                var expected = new List<string>();
                var pythonVersion = ver.Version.ToLanguageVersion();
                if (pythonVersion.Is3x()) {
                    expected.Add("GT_2_7");
                    expected.Add("GE_2_7");
                } else if (pythonVersion == PythonLanguageVersion.V27) {
                    expected.Add("GE_2_7");
                    expected.Add("LE_2_7");
                } else {
                    expected.Add("LT_2_7");
                    expected.Add("LE_2_7");
                }

                AssertUtil.CheckCollection(
                    entry.GetMemberNames(null).Where(n => n.EndsWithOrdinal("2_7")),
                    expected,
                    fullSet.Except(expected)
                );
            }
        }

        #endregion
    */
    }
}
