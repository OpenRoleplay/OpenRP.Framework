using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Models;

namespace OpenRP.Framework.Features.Banking.Models.Configurations
{
    public class BankModelConfiguration : IEntityTypeConfiguration<BankModel>
    {
        public void Configure(EntityTypeBuilder<BankModel> builder)
        {
            builder.Property(bank => bank.Name).HasMaxLength(32).IsRequired();
            builder.Property(bank => bank.Code).HasMaxLength(4).IsRequired();
            builder.Property(bank => bank.MonthlyFee).HasColumnType("decimal(18,2)");

            // The code is part of every OBAN of the bank.
            builder.HasIndex(bank => bank.Code).IsUnique();

            // A jurisdiction with banks can't be deleted out from under their accounts.
            builder.HasOne(bank => bank.Jurisdiction)
                   .WithMany()
                   .HasForeignKey(bank => bank.JurisdictionId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(bank => bank.RunByFaction)
                   .WithMany()
                   .HasForeignKey(bank => bank.RunByFactionId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class BankAccountModelConfiguration : IEntityTypeConfiguration<BankAccountModel>
    {
        public void Configure(EntityTypeBuilder<BankAccountModel> builder)
        {
            builder.Property(account => account.Oban).HasMaxLength(18).IsRequired();
            builder.Property(account => account.Name).HasMaxLength(32).IsRequired();
            builder.Property(account => account.Balance).HasColumnType("decimal(18,2)");

            builder.HasIndex(account => account.Oban).IsUnique();
            builder.HasIndex(account => new { account.OwnerType, account.OwnerId });
            // The fee timer looks for accounts that are due.
            builder.HasIndex(account => account.NextFeeAt);

            // Accounts and their history outlive nothing they belong to by accident: a bank with accounts can't be deleted.
            builder.HasOne(account => account.Bank)
                   .WithMany()
                   .HasForeignKey(account => account.BankId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(account => account.Currency)
                   .WithMany()
                   .HasForeignKey(account => account.CurrencyId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class BankAccountAccessModelConfiguration : IEntityTypeConfiguration<BankAccountAccessModel>
    {
        public void Configure(EntityTypeBuilder<BankAccountAccessModel> builder)
        {
            // A character has one access level per account.
            builder.HasIndex(access => new { access.BankAccountId, access.CharacterId }).IsUnique();

            builder.HasOne(access => access.BankAccount)
                   .WithMany()
                   .HasForeignKey(access => access.BankAccountId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(access => access.Character)
                   .WithMany()
                   .HasForeignKey(access => access.CharacterId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class BankTransactionModelConfiguration : IEntityTypeConfiguration<BankTransactionModel>
    {
        public void Configure(EntityTypeBuilder<BankTransactionModel> builder)
        {
            builder.Property(transaction => transaction.Amount).HasColumnType("decimal(18,2)");
            builder.Property(transaction => transaction.BalanceAfter).HasColumnType("decimal(18,2)");
            builder.Property(transaction => transaction.OtherOban).HasMaxLength(18);

            builder.HasOne(transaction => transaction.BankAccount)
                   .WithMany()
                   .HasForeignKey(transaction => transaction.BankAccountId)
                   .OnDelete(DeleteBehavior.Cascade);

            // The history stays when a character is deleted; only who did it is forgotten.
            builder.HasOne(transaction => transaction.Character)
                   .WithMany()
                   .HasForeignKey(transaction => transaction.CharacterId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class BankBranchModelConfiguration : IEntityTypeConfiguration<BankBranchModel>
    {
        public void Configure(EntityTypeBuilder<BankBranchModel> builder)
        {
            // A property is a branch of one bank at most.
            builder.HasIndex(branch => branch.PropertyId).IsUnique();

            builder.HasOne(branch => branch.Bank)
                   .WithMany()
                   .HasForeignKey(branch => branch.BankId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(branch => branch.Property)
                   .WithMany()
                   .HasForeignKey(branch => branch.PropertyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class BankTellerModelConfiguration : IEntityTypeConfiguration<BankTellerModel>
    {
        public void Configure(EntityTypeBuilder<BankTellerModel> builder)
        {
            // An actor works for one bank at most.
            builder.HasIndex(teller => teller.ActorId).IsUnique();

            builder.HasOne(teller => teller.Actor)
                   .WithMany()
                   .HasForeignKey(teller => teller.ActorId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(teller => teller.Bank)
                   .WithMany()
                   .HasForeignKey(teller => teller.BankId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
