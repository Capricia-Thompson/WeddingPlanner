#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace WeddingPlanner.Models;
public class Wedding
{
    [Key]
    public int WeddingId { get; set; }

    [Required]
    public string WedderOne { get; set; }
    [Required]
    public string WedderTwo { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [ValidWeddingDate]
    public DateTime WeddingDate { get; set; }

    [Required]
    public string WeddingAddress { get; set; }

    public int UserId { get; set; }

    public User? Planner { get; set; }

    public List<Attendee> Guests { get; set; } = new List<Attendee>();

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

public class ValidWeddingDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime date)
        {
            if (date < DateTime.Now)
            {
                return new ValidationResult("Date must be in the future.");
            }
        }

        return ValidationResult.Success;
    }
}