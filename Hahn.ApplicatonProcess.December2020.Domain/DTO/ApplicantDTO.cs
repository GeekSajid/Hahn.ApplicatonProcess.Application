using FluentValidation;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Hahn.ApplicatonProcess.December2020.Domain.DTO
{
    public class ApplicantDTO
    {
        public ApplicantDTO()
        {
            Hired = false;
        }
        public int Id { get; set; }
        /// <summary>
        /// Applicant's Name
        /// </summary>
        /// <example>Shahriar</example>
        public string Name { get; set; }
        /// <summary>
        /// Applicant's Family Name or surname
        /// </summary>
        /// <example>Mahmud</example>
        public string FamilyName { get; set; }
        /// <summary>
        /// Applicant's Address
        /// </summary>
        /// <example>Karwan Bazar,Dhaka</example>
        public string Address { get; set; }
        /// <summary>
        /// Applicant's Birth Country
        /// </summary>
        /// <example>Bangladesh</example>
        public string CountryOfOrigin { get; set; }
        /// <summary>
        /// Applicant's Email Address
        /// </summary>
        /// <example>abc@gmail.com</example>
        public string EmailAddress { get; set; }
        /// <summary>
        /// Age in years
        /// </summary>
        /// <example>25</example>
        public int Age { get; set; }
        /// <example>false</example>
        public bool Hired { get; set; }
    }
    public class ApplicantValidator : AbstractValidator<ApplicantDTO>
    {
        public ApplicantValidator()
        {
            RuleFor(x => x.Name).NotNull().WithMessage("Name is required").MinimumLength(5).WithMessage("Name must be at least 5 Characters");
            RuleFor(x => x.FamilyName).NotNull().WithMessage("FamilyName is required").MinimumLength(5).WithMessage("FamilyName must be at least 5 Characters");
            RuleFor(x => x.Address).NotNull().WithMessage("Address is required").MinimumLength(10).WithMessage("Address must be at least 10 Characters");
            RuleFor(x => x.CountryOfOrigin).NotNull().WithMessage("CountryOfOrigin is required").Must(ValidCountry).WithMessage("CountryOfOrigin must be a valid country");
            RuleFor(x => x.EmailAddress).NotNull().WithMessage("EmailAddress is required").EmailAddress().WithMessage("EmailAddress must be a Valid email");
            RuleFor(x => x.Age).NotNull().WithMessage("Age is required").InclusiveBetween(20, 60).WithMessage("Age must be between 20 and 60");
        }

        private bool ValidCountry(string country)
        {

            var baseUrl = "https://restcountries.eu/rest/v2/name/";
            var client = new HttpClient { BaseAddress = new Uri(baseUrl) };
            var countryCheckUrl = country + "?fullText=true";
            var response = client.GetAsync(countryCheckUrl).Result;
            bool countryValid = response.IsSuccessStatusCode;
            return countryValid;
        }
    }
}
