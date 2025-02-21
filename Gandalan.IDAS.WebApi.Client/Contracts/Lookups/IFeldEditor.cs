﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gandalan.IDAS.WebApi.DTO;

namespace Gandalan.Client.Contracts.Lookups
{
    public interface IFeldEditor
    {
        bool CanHandle(string tag, bool initGroup = false);
        Task<Dictionary<string, string>> ExecuteAsync(BelegPositionDTO data);
    }
}
