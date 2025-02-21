﻿using Gandalan.IDAS.Client.Contracts.Contracts;
using Gandalan.IDAS.WebApi.DTO;
using System.Threading.Tasks;

namespace Gandalan.IDAS.WebApi.Client.BusinessRoutinen
{
    public class ProduktionsfreigabeWebRoutinen : WebRoutinenBase
    {
        public ProduktionsfreigabeWebRoutinen(IWebApiConfig settings) : base(settings)
        {
        }

        public VorgangDTO AddProduktionsfreigabe(BelegartWechselDTO dto)
        {
            if (Login())
            {
                return Put<VorgangDTO>("Produktionsfreigabe", dto);
            }
            return null;
        }

        public async Task<VorgangDTO> AddProduktionsfreigabeAsync(BelegartWechselDTO dto)
        {
            return await Task.Run(() => AddProduktionsfreigabe(dto));
        }

        public string WebJob()
        {
            return Post("Produktionsfreigabe/WebJob", null);
        }
    }
}
