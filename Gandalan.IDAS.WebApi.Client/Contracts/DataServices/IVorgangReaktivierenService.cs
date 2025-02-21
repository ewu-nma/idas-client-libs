﻿using Gandalan.IDAS.WebApi.DTO;
using System;
using System.Threading.Tasks;

namespace Gandalan.Client.Contracts.DataServices
{
    public interface IVorgangReaktivierenService
    {
        Task<VorgangDTO> VorgangReaktivierenAsync(BelegartWechselDTO dto);   
        VorgangDTO VorgangReaktivieren(BelegartWechselDTO dto);   
    }
}

