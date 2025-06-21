using FluentValidation;

namespace CityInfo.API.Models
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
    }

   
public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required.");

            RuleFor(x => x.Age)
                .InclusiveBetween(18, 60)
                .WithMessage("Age must be between 18 and 60.");

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("A valid email address is required.");
        }
    }
}
