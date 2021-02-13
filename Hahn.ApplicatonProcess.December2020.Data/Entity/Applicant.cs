using FluentValidation;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Hahn.ApplicatonProcess.December2020.Data.Entity
{
    public class Applicant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public string Address { get; set; }
        public string CountryOfOrigin { get; set; }
        public string EmailAddress { get; set; }
        public int Age { get; set; }
        public bool Hired { get; set; }
    }
    
}
