//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NorthernLightsHospital
{
    using System;
    using System.Collections.Generic;
    
    public partial class Admission
    {
        public string idAdmission { get; set; }
        public Nullable<bool> chirurgie { get; set; }
        public Nullable<System.DateTime> dateAdmission { get; set; }
        public Nullable<System.DateTime> dateChirurgie { get; set; }
        public Nullable<System.DateTime> dateConge { get; set; }
        public Nullable<bool> televiseur { get; set; }
        public Nullable<bool> telephone { get; set; }
        public string NSS { get; set; }
        public int numeroLit { get; set; }
        public string idMedecin { get; set; }
    
        public virtual Lit Lit { get; set; }
        public virtual Medecin Medecin { get; set; }
        public virtual Patient Patient { get; set; }
        public override string ToString()
        {
            return ($"Admission: {dateAdmission}\n Chirurgie: {dateChirurgie}\n Lit: {numeroLit} M�decin: {idMedecin}");
        }
    }
}
