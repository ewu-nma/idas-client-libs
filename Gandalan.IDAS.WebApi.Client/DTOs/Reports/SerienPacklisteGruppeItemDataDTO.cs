﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gandalan.IDAS.WebApi.Data.DTOs.Reports
{
    public class SerienPacklisteGruppeItemDataDTO
    {
        public int Anzahl { get; set; }
        public string Einheit { get; set; }
        public string Katalognummer { get; set; }
        public string Bezeichnung { get; set; }
        public string FarbKuerzel { get; set; }
        public string FarbCode { get; set; }
        public string FarbBezeichnung { get; set; }
        public float Laenge { get; set; }
        public string Winkel { get; set; }
        public bool IstZuschnitt { get; set; }

    }
}
