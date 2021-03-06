﻿// Visual Studio Shared Project
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
// MERCHANTABILITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System.Windows.Automation;

namespace TestUtilities.UI {
    public class ExceptionHelperAdornment : AutomationWrapper {
        public ExceptionHelperAdornment(AutomationElement element)
            : base(element) {
        }

        public string Title {
            get {
                // There are no automation name or id we can use to locate the adorner title
                // Don't be surprised if this breaks when they rearrange the controls
                var title = FindFirstByControlType(ControlType.Text);
                return title.GetCurrentPropertyValue(AutomationElement.NameProperty) as string;
            }
        }

        public string Description {
            get {
                var desc = FindByAutomationId("ExceptionDescription");
                return (((TextPattern)desc.GetCurrentPattern(TextPattern.Pattern)).DocumentRange.GetText(-1).ToString());
            }
        }
    }
}
