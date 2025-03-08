using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyManager.Logic.Entities
{
    /// <summary>
    /// Represents an employee entity.
    /// </summary>
    [Table("Employees")]
    [Index(nameof(FirstName), nameof(LastName), IsUnique = false)]
    [Index(nameof(Email), IsUnique = true)]
    public class Employee : EntityObject, Common.Contracts.IEmployee
    {
        #region properties
        /// <summary>
        /// Gets or sets the company ID.
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the first name of the employee.
        /// </summary>
        [MaxLength(64)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last name of the employee.
        /// </summary>
        [MaxLength(64)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email of the employee.
        /// </summary>
        [MaxLength(128)]
        public string Email { get; set; } = string.Empty;
        #endregion properties

        #region navigation properties
        /// <summary>
        /// Gets or sets the associated company.
        /// </summary>
        public Company? Company { get; set; }
        #endregion navigation properties

        #region methods
        /// <summary>
        /// Returns a string representation of the employee.
        /// </summary>
        /// <returns>A string representation of the employee.</returns>
        public override string ToString()
        {
            return $"Employee: {FirstName} {LastName} - {Email}";
        }
        #endregion methods
    }
}
