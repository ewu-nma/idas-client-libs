﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gandalan.Client.Contracts.DataServices
{
    public interface IScriptService
    {
        Task<Dictionary<string, string>> GetAllAsync();
        Task Clean();
    }
}
