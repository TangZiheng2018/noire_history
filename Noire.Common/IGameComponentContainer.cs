﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noire.Common {
    public interface IGameComponentContainer {

        GameComponentCollection ChildComponents { get; }

    }
}
