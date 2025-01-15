using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MEDCHGClassLibrary.Model
{
    public class Patient:BaseEntity
    {

        [Required(ErrorMessage = "MRN is required")]
        [StringLength(120, MinimumLength = 3, ErrorMessage = "MRN Name must be between {2} and {1} characters long.")]
        public string MRN { get; set; }

        private string firstName;
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(120, MinimumLength = 3, ErrorMessage = "First Name must be between {2} and {1} characters long.")]
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        private string middleName;
        public string? MiddleName
        {
            get { return middleName; }
            set { middleName = value; }
        }
        public DateTime? PatientAddmissionDateTime { get; set; }
        private string lastName;
        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(120, MinimumLength = 3, ErrorMessage = "Last Name must be between {2} and {1} characters long.")]
        public string? LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public string? Name
        {
            get
            {
                // Concatenate the first, middle, and last names, trimming any extra spaces.
                return $"{firstName} {middleName} {lastName}".Trim();
            }
            set
            {
                if (value != null)
                {
                    // Split the full name into parts.
                    string[] parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 1)
                    {
                        firstName = parts[0];

                    }

                    else if (parts.Length == 2)

                    {
                        firstName = parts.First();
                        lastName = parts.Last();

                    }

                    else if (parts.Length == 3)

                    {
                        firstName = parts.First();
                        lastName = parts.Last();
                        middleName = parts[1];
                    }

                    else
                    {
                        lastName = string.Join(" ", parts.Skip(2));
                    }
                }
            }
        }


        public string? Salutation { get; set; }


        [Required(ErrorMessage = "Gender is required")]
        [Column("Gender", Order = 7, TypeName = "int")]
        public Gender Gender { get; set; }

        public DateOnly DateOfBirth
        {
            get
            {
                try
                {
                    if (Year != null && Month != null & Day != null)
                        return new DateOnly((int)Year, (int)Month, (int)Day);
                    else
                    {
                        return DateOnly.MaxValue;
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    return DateOnly.MaxValue;
                }
            }
            set
            {
                Day = value.Day;
                Month = value.Month;
                Year = value.Year;
            }
        }
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        [NotMapped]
        public PatientVisitHistory? PatientVisitHistories { get; set; } = new ();
    }

    public enum Gender
    {
        Male = 1,
        Female = 2
    }
}
