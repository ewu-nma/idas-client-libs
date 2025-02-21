﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Gandalan.IDAS.WebApi.DTO
{
    public class BelegDruckDTO
    {
        public BelegDruckDTO(VorgangDTO vorgang, BelegDTO beleg)
        {
            CultureInfo culture = new CultureInfo("de-de");
            Salden = new ObservableCollection<BelegSaldoDruckDTO>();
            PositionsObjekte = new ObservableCollection<BelegPositionDruckDTO>();

            if (beleg != null && vorgang != null)
            {
                beleg.SetupObjekte(vorgang);

                this.BelegGuid = beleg.BelegGuid;
                this.VorgangGuid = vorgang.VorgangGuid;
                this.BelegArt = beleg.BelegArt;
                this.BelegNummer = vorgang.VorgangsNummer;
                this.VorgangsNummer = vorgang.VorgangsNummer;
                this.BelegDatum = beleg.BelegDatum;
                this.VorgangErstellDatum = vorgang.ErstellDatum.ToString("d", culture);
                this.AenderungsDatum = beleg.AenderungsDatum;
                this.BelegJahr = beleg.BelegJahr;
                this.Schlusstext = beleg.Schlusstext;
                this.Kommission = string.IsNullOrEmpty(vorgang.Kommission) ? String.Empty : "Kommission: " + vorgang.Kommission;
                this.Ausfuehrungsdatum = string.IsNullOrEmpty(beleg.AusfuehrungsDatum) ? String.Empty : "Ausführungsdatum: " + beleg.AusfuehrungsDatum;
                this.AnsprechpartnerKunde = beleg.AnsprechpartnerKunde ?? "";
                this.Ansprechpartner = ""; //??? _apiSettings?.AuthToken?.Benutzer?.Vorname + " " + _apiSettings?.AuthToken?.Benutzer?.Nachname;
                this.Telefonnummer = ""; //??? _apiSettings?.AuthToken?.Benutzer?.TelefonNummer ?? "";
                this.Bestelldatum = !String.IsNullOrEmpty(beleg.BelegDatum.ToString("d", culture)) ? beleg.BelegDatum.ToString("d", culture) : "";
                this.Lieferzeit = ""; //???
                this.IsEndkunde = vorgang.Kunde?.IstEndkunde != null ? vorgang.Kunde.IstEndkunde : false;
                this.IsRabatt = beleg.PositionsObjekte?.Any(i => !i.Equals(0m)) ?? false;
                this.IstSelbstabholer = beleg.IstSelbstabholer;

                if (string.IsNullOrEmpty(beleg.BelegTitelUeberschrift))
                {
                    switch (beleg.BelegArt)
                    {
                        case "AB":
                            BelegTitelUeberschrift = "Auftragsbestätigung";
                            break;
                        default:
                            BelegTitelUeberschrift = beleg.BelegArt;
                            break;
                    }

                    if (string.IsNullOrEmpty(beleg.BelegTitelZeile1))
                    {
                        switch (beleg.BelegArt)
                        {
                            case "Rechnung":
                                BelegTitelZeile1 =
                                    $"Nr. {beleg.BelegNummer} vom {beleg.BelegDatum.ToString(culture.DateTimeFormat.ShortDatePattern, culture)}";
                                BelegTitelZeile2 =
                                    $"Vorgang Nr. {vorgang.VorgangsNummer}";
                                break;
                            default:
                                BelegTitelZeile1 =
                                    $"Vorgang Nr. {vorgang.VorgangsNummer} vom " +
                                    beleg.BelegDatum.ToString(culture.DateTimeFormat.ShortDatePattern, culture);
                                break;
                        }
                    }
                }
                else
                {
                    BelegTitelUeberschrift = beleg.BelegTitelUeberschrift;
                    BelegTitelZeile1 = beleg.BelegTitelZeile1;
                    BelegTitelZeile2 = beleg.BelegTitelZeile2;
                }

                this.TextFuerAnschreiben = beleg.TextFuerAnschreiben;
                this.BelegAdresse = new AdresseDruckDTO(beleg.BelegAdresse);
                this.BelegAdresseString = BelegAdresse.ToString();
                this.VersandAdresse = new AdresseDruckDTO(beleg.VersandAdresse);
                this.VersandAdresseString = VersandAdresse.ToString();

                bool preiseAnzeigen = beleg.BelegArt != "Lieferschein" && beleg.BelegArt != "Bestellschein";

                if (beleg.PositionsObjekte.Any(p => p.IstSonderfarbPosition && p.Farbzuschlag == -1))
                {
                    preiseAnzeigen = false;
                }

                foreach (BelegPositionDTO dto in beleg.PositionsObjekte)
                {
                    if (!dto.IstAktiv && !dto.IstAlternativPosition)
                    {
                        continue;
                    }

                    this.PositionsObjekte.Add(new BelegPositionDruckDTO(dto, preiseAnzeigen));
                }

                if (preiseAnzeigen)
                {
                    var saldenSorted = beleg.Salden.OrderBy(i => i.Reihenfolge);
                    if (saldenSorted.Any())
                    {
                        var lastActivSalde = saldenSorted.Last(s => !s.IstInaktiv);
                        foreach (BelegSaldoDTO dto in saldenSorted)
                        {
                            if (dto.IstInaktiv)
                            {
                                continue;
                            }

                            this.Salden.Add(new BelegSaldoDruckDTO(dto) { IsLastElement = lastActivSalde != null && lastActivSalde == dto });
                        }
                    }
                }

                CountValuePositionen = PositionsObjekte.Count();
                CountValueSalden = Salden.Count();
            }
        }

        public Guid BelegGuid { get; set; }
        public Guid VorgangGuid { get; set; }
        public string Kopfzeile { get; set; }
        public string Fusszeile { get; set; }
        public string BelegArt { get; set; }
        public long BelegNummer { get; set; }
        public long VorgangsNummer { get; set; }
        public DateTime BelegDatum { get; set; }
        public string VorgangErstellDatum { get; set; }
        public DateTime AenderungsDatum { get; set; }
        public int BelegJahr { get; set; }
        public string Schlusstext { get; set; }
        public string BelegTitelUeberschrift { get; set; }
        public string BelegTitelZeile1 { get; set; }
        public string BelegTitelZeile2 { get; set; }
        public string TextFuerAnschreiben { get; set; }
        public string Kommission { get; set; }
        public string Ausfuehrungsdatum { get; set; }
        public string AnsprechpartnerKunde { get; set; }
        public string Ansprechpartner { get; set; }
        public string Telefonnummer { get; set; }
        public string Bestelldatum { get; set; }
        public AdresseDruckDTO BelegAdresse { get; set; }
        public string BelegAdresseString { get; set; }
        public AdresseDruckDTO VersandAdresse { get; set; }
        public string VersandAdresseString { get; set; }
        public ObservableCollection<BelegPositionDruckDTO> PositionsObjekte { get; set; }
        public IList<BelegSaldoDruckDTO> Salden { get; set; }
        public int CountValuePositionen { get; set; }
        public int CountValueSalden { get; set; }
        public string Lieferzeit { get; set; }
        public bool IsEndkunde { get; set; }
        public bool IsRabatt { get; set; } = false;
        public bool IstSelbstabholer { get; set; } = false;

        public void SetTextBausteine(object textBausteine)
        {
            if (textBausteine is ExpandoObject)
            {
                var settingsDict = textBausteine as IDictionary<string, object>;
                var keys = new List<string>(settingsDict.Keys);

                if (keys.Contains(BelegArt))
                {
                    dynamic existingValue = settingsDict[BelegArt];
                    Kopfzeile = existingValue.Kopfzeile;
                    Fusszeile = existingValue.Fusszeile;
                }
            }
        }
    }

    public class AdresseDruckDTO
    {
        public AdresseDruckDTO() { }

        public AdresseDruckDTO(BeleganschriftDTO beleganschrift)
        {
            if (beleganschrift != null)
            {
                this.Anrede = beleganschrift.Anrede;
                this.Nachname = beleganschrift.Nachname;
                this.Vorname = beleganschrift.Vorname;
                this.Firmenname = beleganschrift.Firmenname;
                this.Zusatz = beleganschrift.Zusatz;
                this.AdressZusatz1 = beleganschrift.AdressZusatz1;
                this.Strasse = beleganschrift.Strasse;
                this.Hausnummer = beleganschrift.Hausnummer;
                this.Postleitzahl = beleganschrift.Postleitzahl;
                this.Ort = beleganschrift.Ort;
                this.Ortsteil = beleganschrift.Ortsteil;
                this.Land = beleganschrift.Land;
                this.IstInland = beleganschrift.IstInland;
            }
        }

        public string Anrede { get; set; }
        public string Nachname { get; set; }
        public string Vorname { get; set; }
        public string Firmenname { get; set; }
        public string Zusatz { get; set; }
        public string AdressZusatz1 { get; set; }
        public string Strasse { get; set; }
        public string Hausnummer { get; set; }
        public string Postleitzahl { get; set; }
        public string Ort { get; set; }
        public string Ortsteil { get; set; }
        public string Land { get; set; }
        public bool IstInland { get; set; }

        public override string ToString()
        {
            StringBuilder adressText = new StringBuilder();
            {
                adressText.AppendLine(Anrede);
                adressText.AppendLine(buildAnschriftsName());
                if (!string.IsNullOrEmpty(AdressZusatz1))
                {
                    adressText.AppendLine(AdressZusatz1);
                }

                if (!string.IsNullOrEmpty(Ortsteil))
                {
                    adressText.AppendLine(Ortsteil);
                }

                adressText.AppendLine($"{Strasse} {Hausnummer}");
                adressText.Append($"{Postleitzahl} {Ort}");
                if (!IstInland)
                {
                    adressText.AppendLine().Append(Land?.ToUpper());
                }
            }
            return adressText.ToString();
        }

        private string buildAnschriftsName()
        {
            var adesszusatz = "";
            var name1 = string.IsNullOrEmpty(Vorname) ? Nachname : Vorname;
            var name2 = string.IsNullOrEmpty(Vorname) ? adesszusatz : Nachname;
            return (!string.IsNullOrEmpty(Firmenname) ? $"{Firmenname}" : ($"{name1} {name2}")).Trim();
        }
    }

    public class BelegSaldoDruckDTO
    {
        public BelegSaldoDruckDTO() { }

        public BelegSaldoDruckDTO(BelegSaldoDTO saldo)
        {
            CultureInfo culture = new CultureInfo("de-de");
            if (saldo != null)
            {
                this.Reihenfolge = saldo.Reihenfolge;
                this.Text = saldo.Text;
                var vorzeichen = saldo.Typ == "Abschlag" ? '-' : ' ';
                this.Betrag = vorzeichen + saldo.Betrag.ToString(culture);
            }
        }

        public int Reihenfolge { get; set; }
        public string Text { get; set; }
        public string Betrag { get; set; }
        public bool IsLastElement { get; set; } = false;
    }

    public class BelegPositionDruckDTO
    {
        public int LaufendeNummer { get; set; }
        public string ArtikelNummer { get; set; }
        public string Variante { get; set; }
        public bool IstAlternativPosition { get; set; }
        public bool IstAktiv { get; set; }
        public decimal Menge { get; set; }
        public string EinzelpreisOhneFarbzuschlag { get; set; }
        public string Einzelpreis { get; set; }
        public string Rabatt { get; set; }
        public string Gesamtpreis { get; set; }
        public string Farbzuschlag { get; set; }
        public string MengenEinheit { get; set; }
        public string Text { get; set; }
        public string AngebotsText { get; set; }
        public string SonderwunschText { get; set; }
        public string SonderwunschAngebotsText { get; set; }
        public string ProduktionZusatzInfo { get; set; }
        public bool ProduktionZusatzInfoPrintOnReport { get; set; }
        public bool ProduktionZusatzInfoPrintZusatzEtikett { get; set; }

        public BelegPositionDruckDTO()
        {
        }

        public BelegPositionDruckDTO(BelegPositionDTO position, bool preiseAnzeigen = true)
        {
            CultureInfo culture = new CultureInfo("de-de");
            if (position != null)
            {
                this.LaufendeNummer = position.LaufendeNummer;
                this.ArtikelNummer = position.ArtikelNummer;
                this.Variante = position.Variante;
                this.IstAlternativPosition = position.IstAlternativPosition;
                this.IstAktiv = position.IstAktiv;
                this.Menge = position.Menge;
                this.MengenEinheit = position.Daten.FirstOrDefault(d => d.KonfigName.Equals("Konfig.ZuschnittLaenge")) != null ? "Stk." : position.MengenEinheit;
                if (this.MengenEinheit == null || this.MengenEinheit.Equals("st", StringComparison.InvariantCultureIgnoreCase))
                {
                    this.MengenEinheit = "Stk.";
                }

                string einbauort = String.Empty;
                if (!string.IsNullOrWhiteSpace(position.Einbauort) && !position.Text.StartsWith("Einbauort"))
                {
                    einbauort = "Einbauort: " + position.Einbauort + " - ";
                }

                this.Text = einbauort + position.Text;
                this.AngebotsText = einbauort + position.AngebotsText;
                this.SonderwunschText = position.SonderwunschText;
                this.SonderwunschAngebotsText = position.SonderwunschAngebotsText;
                this.ProduktionZusatzInfo = position.ProduktionZusatzInfo;
                this.ProduktionZusatzInfoPrintOnReport = position.ProduktionZusatzInfoPrintOnReport;
                this.ProduktionZusatzInfoPrintZusatzEtikett = position.ProduktionZusatzInfoPrintZusatzEtikett;
                if (preiseAnzeigen)
                {
                    this.Farbzuschlag = position.Farbzuschlag.ToString(culture);
                    this.EinzelpreisOhneFarbzuschlag = position.Einzelpreis.ToString(culture);
                    this.Rabatt = position.Rabatt.Equals(0m) ? String.Empty : position.Rabatt.ToString(culture);
                    this.Gesamtpreis = position.Gesamtpreis.ToString(culture);
                    this.Einzelpreis = (position.Einzelpreis + position.Farbzuschlag).ToString(culture);
                }
            }
        }
    }
}
