using System;
using System.Collections.Generic;

namespace BankingSystem.Persistance.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Branchid { get; set; }

    public string Role { get; set; } = null!;
}
