﻿// Copyright © 2009,2010 Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Collections.Generic;
using fitSharp.Machine.Engine;
using fitSharp.Machine.Model;
using fitSharp.Slim.Operators;

namespace fitSharp.Slim.Service {
    public class Service: ProcessorBase<string, SlimProcessor>, SlimProcessor {
        private readonly SlimOperators operators;
        private readonly Stack<TypedValue> libraryInstances = new Stack<TypedValue>();

        public Service(): this(new Configuration()) {}

        public Service(Configuration configuration) : base(configuration) {
            operators = configuration.GetItem<SlimOperators>();
            operators.Processor = this;

            AddMemory<SavedInstance>();
            AddMemory<Symbol>();
        }

        public void PushLibraryInstance(TypedValue instance) {
            libraryInstances.Push(instance);
        }

        public IEnumerable<TypedValue> LibraryInstances { get { return libraryInstances; } }

        protected override Operators<string, SlimProcessor> Operators {
            get { return operators; }
        }
    }
}