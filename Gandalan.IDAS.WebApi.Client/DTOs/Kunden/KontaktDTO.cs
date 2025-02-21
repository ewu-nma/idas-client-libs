﻿using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Gandalan.IDAS.WebApi.Util;
using PropertyChanged;

namespace Gandalan.IDAS.WebApi.DTO
{

    public class KontaktDTO : IDTOWithApplicationSpecificProperties, INotifyPropertyChanged
    {
        /// <summary>
        /// Eindeutige GUID
        /// </summary>
        public Guid KontaktGuid { get; set; }
        /// <summary>
        /// Nachname (für Endkunden)
        /// </summary>
        public string Nachname { get; set; }
        /// <summary>
        /// Vorname(n) (für Endkunden)
        /// </summary>
        public string Vorname { get; set; }
        /// <summary>
        /// Firmierung (für jur. Personen)
        /// </summary>
        public string Firmenname { get; set; }
        /// <summary>
        /// Kundennummer (alphanummerisch)
        /// </summary>
        public string KundenNummer { get; set; }
        /// <summary>
        /// Datum des ersten Kontakts
        /// </summary>
        public DateTime? Erstkontakt { get; set; }
        /// <summary>
        /// Datum des letzten Kontakts
        /// </summary>
        public DateTime? LetzterKontakt { get; set; }
        /// <summary>
        /// Endkunde oder Firmenkunde
        /// </summary>
        public bool IstEndkunde { get; set; }
        /// <summary>
        /// allg. Informationen
        /// </summary>
        public string Kommentar { get; set; }
        /// <summary>
        /// Status Kontakt oder Interessent
        /// </summary>
        public bool IstKunde { get; set; }
        /// <summary>
        /// Gesperrt/Wird nicht beliefert
        /// </summary>
        public bool IstGesperrt { get; set; }
        /// <summary>
        /// Holt Ware normalerweise ab
        /// </summary>
        public bool IstSelbstabholer { get; set; }
        /// <summary>
        /// Kunde muss in Vorkasse gehen.
        /// </summary>
        public bool IstVorkasse { get; set; }

        public bool IstUmsatzsteuerPflichtig { get; set; } = true;
        /// <summary>
        /// Zugeordnete Personen
        /// </summary>
        public virtual IList<PersonDTO> Personen { get; set; }
        /// <summary>
        /// Zugeordnete Standard-Adressen
        /// </summary>
        public virtual IList<ZusatzanschriftDTO> Zusatzanschriften { get; set; }
        public bool IstDummyKunde { get; set; }
        public decimal ArtikelRabattVorgabe { get; set; }
        public decimal ElementRabattVorgabe { get; set; }
        public string UmsatzSteuerId { get; set; }
        public string Sonderetikett { get; set; }
        public string Branche { get; set; }
        public string Briefanrede { get; set; }
        public string Titel { get; set; }
        /// <summary>
        /// Namensanrede, z.B. "Herr"/"Frau"/"Firma"
        /// </summary>
        public string Anrede { get; set; }
        /// <summary>
        /// Adresszusatz, z.B. "c/o" (belegbezogen)
        /// </summary>
        public string AdressZusatz1 { get; set; }
        /// <summary>
        /// Adresszusatz, z.B. "c/o" (belegbezogen)
        /// </summary>
        public string AdressZusatz2 { get; set; }
        /// <summary>
        /// Postalische Straße
        /// </summary>
        public string Strasse { get; set; }
        /// <summary>
        /// Postalische Hausnummer, ggf. mit Suffix/Prafix z.B. "16a" oder "77 (Hinterhaus)"
        /// </summary>
        public string Hausnummer { get; set; }
        /// <summary>
        /// Postfach (ersetzt Straße/Hausnummer in der Ausgabe)
        /// </summary>
        public string Postfach { get; set; }
        /// <summary>
        /// Postalische Postleitzahl bezogen auf die Straßen- oder Postfachadresse
        /// </summary>
        public string Postleitzahl { get; set; }
        /// <summary>
        /// Postalische Ortsangabe
        /// </summary>
        public string Ort { get; set; }
        /// <summary>
        /// Nicht-postalische Angabe zum Ortsteil
        /// </summary>
        public string Ortsteil { get; set; }
        /// <summary>
        /// Land als Textkürzel
        /// </summary>
        public string Land { get; set; }
        /// <summary>
        /// E-Mail-Adresse
        /// </summary>
        public string Mailadresse { get; set; }
        /// <summary>
        /// Telefon: Ländervorwahl (kanonisch [+49] oder landesspezifisch [0049])
        /// </summary>
        public string Landesvorwahl { get; set; }
        /// <summary>
        /// Ortskennzahl mit führender "0" in Deutschland
        /// </summary>
        public string Vorwahl { get; set; }
        /// <summary>
        /// Anschlussrufnummer
        /// </summary>
        public string Telefonnummer { get; set; }
        /// <summary>
        /// Durchwahlzusatz
        /// </summary>
        public string Durchwahl { get; set; }
        /// <summary>
        /// Internet-/Web-URL mit Protokoll-Präfix (https://...)
        /// </summary>
        public string Webadresse { get; set; }
        /// <summary>
        /// Transportkosten für den Kunden
        /// </summary>
        public decimal? Transportkosten { get; set; }
        /// <summary>
        /// Transportkosten für den Kunden
        /// </summary>
        public bool AbweichendeTransportkosten { get; set; }
        /// <summary>
        /// Intern
        /// </summary>
        public Dictionary<string, PropertyValueCollection> ApplicationSpecificProperties { get; set; }
        public Guid KontaktMandantGuid { get; set; }
        public bool KontaktMandantIstAktiv { get; set; }
        public long Version { get; set; }
        public DateTime ChangedDate { get; set; }
        /// <summary>
        /// Zahleneintrag für Tage
        /// </summary>
        public int NettoTage { get; set; }
        /// <summary>
        /// Zahleneintrag für Tage sowie Skonto in %
        /// </summary>
        public decimal Skonto { get; set; }
        /// <summary>
        /// Freitextfeld bei Vorkasse Kunden "Hinweis auf Zahlung" (Angebot + AB)
        /// </summary>
        public string SchlussTextAngebotAB { get; set; }
        /// <summary>
        /// Freitextfeld bei Vorkasse Kunden "Hinweis auf Zahlung" (Rechnung)
        /// </summary>
        public string SchlussTextRechnung { get; set; }
        /// <summary>
        /// Freitextfeld für die Zahlungsbedingung
        /// </summary>
        public string Zahlungsbedingung { get; set; }
        /// <summary>
        /// Kunde hat Winterrabatt ja/nein
        /// </summary>
        public bool HatWinterrabatt { get; set; }
        public bool KeineAutofreigabe { get; set; }
        public bool ErbtAuswahlOhneSprosse { get; set; } = false;
        public bool DigitalerRechnungsversand { get; set; }
        public bool IstSammelrechnungsKunde { get; set; }

        public KontaktDTO()
        {
            Zusatzanschriften = new ObservableCollection<ZusatzanschriftDTO>();
            Personen = new ObservableCollection<PersonDTO>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return AnzeigeName;
        }

        /// <summary>
        /// Erzeugt einen Text aus den Namensfeldern für die Anzeige in 
        /// Überschriften, Anschriftenfeldern usw. 
        /// </summary>
        public string AnzeigeName
        {
            get
            {
                return string.IsNullOrEmpty(Firmenname) ? $"{Anrede} {Titel} {Vorname} {Nachname}".Trim() : Firmenname;
            }
        }
    }
}