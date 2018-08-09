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
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.PythonTools.Analysis.LanguageServer;
using Microsoft.PythonTools.Interpreter;
using Microsoft.PythonTools.Interpreter.Ast;
using TestUtilities;

namespace Microsoft.PythonTools.Analysis {
    public static class ServerExtensions {
        public static async Task<Server> InitializeAsync(this Server server, InterpreterConfiguration configuration) {
            configuration.AssertInstalled();

            server.OnLogMessage += Server_OnLogMessage;
            var properties = new InterpreterFactoryCreationOptions {
                TraceLevel = System.Diagnostics.TraceLevel.Verbose,
                DatabasePath = TestData.GetTempPath($"AstAnalysisCache{configuration.Version}")
            }.ToDictionary();

            configuration.WriteToDictionary(properties);

            await server.Initialize(new InitializeParams {
                initializationOptions = new PythonInitializationOptions {
                    interpreter = new PythonInitializationOptions.Interpreter {
                        assembly = typeof(AstPythonInterpreterFactory).Assembly.Location,
                        typeName = typeof(AstPythonInterpreterFactory).FullName,
                        properties = properties
                    },
                    analysisUpdates = true,
                    traceLogging = true,
                },
                capabilities = new ClientCapabilities {
                    python = new PythonClientCapabilities {
                        liveLinting = true,
                    }
                }
            });
            
            return server;
        }

        public static void SendDidChangeTextDocument(this Server server, Uri uri) {
            server.DidChangeTextDocument(new DidChangeTextDocumentParams {
                textDocument = new VersionedTextDocumentIdentifier {

                }
            });
        }

        private static void Server_OnLogMessage(object sender, LogMessageEventArgs e) {
            switch (e.type) {
                case MessageType.Error: Trace.TraceError(e.message); break;
                case MessageType.Warning: Trace.TraceWarning(e.message); break;
                case MessageType.Info: Trace.TraceInformation(e.message); break;
                case MessageType.Log: Trace.TraceInformation("LOG: " + e.message); break;
            }
        }
    }
}
