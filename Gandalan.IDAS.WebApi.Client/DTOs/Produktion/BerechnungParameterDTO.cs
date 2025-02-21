﻿using Gandalan.IDAS.WebApi.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gandalan.IDAS.WebApi.Data.DTOs.Produktion
{
    public class BerechnungParameterDTO : ICloneable
    {
        public Guid MandantGuid { get; set; } = Guid.Empty;
        public bool SaveResultData { get; set; } = true;
        public BelegPositionAVDTO BelegPositionAVDTO { get; set; }
        public long VorgangsNummer { get; set; } = -999;
        public long BelegNummer { get; set; } = -999;
        public bool IgnoreSonderwuensche { get; set; }
        public bool ReturnRawDataFile { get; set; }
        public string RawDataFileContent { get; set; }

        public object Clone()
        {
            var jsonString = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<BerechnungParameterDTO>(jsonString);
        }
    }

    public class BerechnungResultDTO
    {
        public BelegPositionDTO OriginalBelegPosition { get; set; }
        public Guid BelegPositionGuid { get; set; }
        public string RawDataFileContent { get; set; }
        public ProduktionsDatenDTO ProduktionsDaten { get; set; }
    }
}
