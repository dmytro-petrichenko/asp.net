using E_Commerce.Payment.Gateway.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Payment.Gateway.DataBase;

public class Database : DbContext
{
    public DbSet<CardDetails> CardDetails { get; set; } = default!;
    public DbSet<PaymentDetails> PaymentDetails { get; set; } = default!;

    public Database(DbContextOptions<Database> options)
        : base(options)
    {
    }
}