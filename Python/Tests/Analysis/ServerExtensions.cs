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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.PythonTools.Analysis.LanguageServer;
using Microsoft.PythonTools.Intellisense;
using Microsoft.PythonTools.Interpreter;
using Microsoft.PythonTools.Interpreter.Ast;
using TestUtilities;

namespace Microsoft.PythonTools.Analysis {
    internal static class ServerExtensions {
        public static async Task<Server> InitializeAsync(this Server server, InterpreterConfiguration configuration, params string[] searchPaths) {
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
                    searchPaths = searchPaths,
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

        public static Task<ModuleAnalysis> GetAnalysisAsync(this Server server, Uri uri, int waitingTimeout = -1, CancellationToken cancellationToken = default(CancellationToken))
            => ((ProjectEntry)server.ProjectFiles.GetEntry(uri)).GetAnalysisAsync(waitingTimeout, cancellationToken);

        public static void EnqueueItem(this Server server, Uri uri) 
            => server.EnqueueItem((IDocument)server.ProjectFiles.GetEntry(uri));

        public static Task SendDidOpenTextDocument(this Server server, string path, string content, string languageId = null) 
            => server.SendDidOpenTextDocument(new Uri(path), content, languageId);

        public static Task SendDidOpenTextDocument(this Server server, Uri uri, string content, string languageId = null) {
            return server.DidOpenTextDocument(new DidOpenTextDocumentParams {
                textDocument = new TextDocumentItem {
                    uri = uri,
                    text = content,
                    languageId = languageId ?? "python"
                }
            });
        }

        private static void Server_OnLogMessage(object sender, LogMessageEventArgs e) {
            switch (e.type) {
                case MessageType.Error: Trace.TraceError($"[{TestEnvironmentImpl.Elapsed()}]: {e.message}"); break;
                case MessageType.Warning: Trace.TraceWarning($"[{TestEnvironmentImpl.Elapsed()}]: {e.message}"); break;
                case MessageType.Info: Trace.TraceInformation($"[{TestEnvironmentImpl.Elapsed()}]: {e.message}"); break;
                case MessageType.Log: Trace.TraceInformation($"[{TestEnvironmentImpl.Elapsed()}] LOG: {e.message}"); break;
            }
        }
    }
}
